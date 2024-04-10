using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Common;
using ReleaseMonkey.src.Repositories;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
  public class UserProjectsService(UserProjectsRepository userProjects, UsersRepository users, ProjectsRepository projects, Db db)
  {

    public Task<UserProject> GetUserProjectById(int userProjectId)
    {
      return Task.FromResult(userProjects.GetUserProjectById(db, userProjectId));
    }

    public async Task<UserProject> InsertUserProjectByUserID(int userId, int projectId, int role)
    {
      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          UserProject userProject = userProjects.InsertUserProject(transaction, db, userId, projectId, role);
          var project = projects.GetProjectById(transaction, db, projectId);
          var user = users.GetUserById(transaction, userId);
          Email.sendEmail([user.Email], "releases", project.Name, "", Email.WelcomeNewBetaTester);
          transaction.Commit();
          return await Task.FromResult(userProject);
        }
        catch (Exception)
        {
          transaction.Rollback();
          throw;
        }
      }
    }

    public async Task<UserProject> InsertUserProjectByEmail(string email, int projectId, int role)
    {

      User user;
      try
      {
        user = users.FindByEmail(email);
      }
      catch (KeyNotFoundException)
      {
        user = users.InsertOrUpdateUser("", email);
      }

      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          UserProject userProject = userProjects.InsertUserProject(transaction, db, user.Id, projectId, role);
          var project = projects.GetProjectById(transaction, db, projectId);
          Email.sendEmail([user.Email], "releases", project.Name, "", Email.WelcomeNewPrimaryTester);
          transaction.Commit();
          return await Task.FromResult(userProject);
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
