using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Repositories;
using ReleaseMonkey.src.Common;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
  public class ReleasesService(ReleasesRepository releases, ReleaseTestersRepository releaseTesters, UserProjectsRepository userProjects, ProjectsRepository projects, UsersRepository users, Db db)
  {
    public Task<List<Release>> GetAllReleases(int userId)
    {
      return Task.FromResult(releases.SelectReleasesByUserId(userId));
    }

    public Task<List<Release>> GetReleasesByProjectId(int projectId)
    {
      return Task.FromResult(releases.GetReleasesByProjectId(db, projectId));
    }

    public Task<List<PendingRelease>> GetPendingReleasesByUserId(int userId)
    {
      return Task.FromResult(releases.GetPendingReleasesByUserId(db, userId));
    }

    public Task<Release> GetReleaseById(int releaseId)
    {
      return Task.FromResult(releases.GetReleaseById(db, releaseId));
    }

    public Task<Release> CreateRelease(string releaseName, int projectId, string downloadLink)
    {
      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          Release release = releases.InsertRelease(transaction, db, releaseName, projectId, downloadLink);
          List<UserProject> userProjectList = userProjects.GetTestersForProject(transaction, db, projectId);
          List<UserProject> testerList = userProjectList.Where(u => u.Role == 1).ToList();
          foreach (UserProject userProject in testerList)
          {
            releaseTesters.InsertReleaseTester(transaction, db, release.Id, userProject.UserId);
          }

          Project project = projects.GetProjectById(transaction, db, release.ProjectId);

          var emails = from u in userProjectList
                       select u.UserId;

          Email.sendEmail(users.GetUserEmailsByIds(transaction, db, emails.ToList()), releaseName, project.Name, downloadLink, Email.BeginRelease);

          transaction.Commit();
          return Task.FromResult(release);
        }
        catch (Exception)
        {
          transaction.Rollback();
          throw;
        }
      }

    }
  }
}