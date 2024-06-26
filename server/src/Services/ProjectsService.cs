using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using Microsoft.Data.SqlClient;
using ReleaseMonkey.src.Repositories;
using System.Collections.Generic;
using System.Data;

namespace ReleaseMonkey.Server.Services
{
  public class ProjectsService(ProjectsRepository projects, UserProjectsRepository userProjects, Db db)
  {
    public Task<Project> CreateProject(int userId, string projectName, string githubRepo, string token, bool publicProject)
    {
      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          Project project = projects.InsertProject(transaction, db, projectName, githubRepo, token, publicProject);
          userProjects.InsertUserProject(transaction, db, userId, project.Id, 0);
          transaction.Commit();
          return Task.FromResult(project);
        }
        catch (Exception)
        {
          transaction.Rollback();
          throw;
        }
      }
    }

    public Task<Project> UpdateProject(int projectId, string projectName, string githubRepo, string token, bool publicProject)
    {
      Project project = projects.UpdateProject(db, projectId, projectName, githubRepo, token, publicProject);
      return Task.FromResult(project);
    }

    public Task<List<Project>> GetAllProjects()
    {
      return Task.FromResult(projects.GetProjects(db));
    }

    public Task<List<Project>> GetProjectsByUserId(int userId)
    {
      return Task.FromResult(projects.GetProjectsByUserId(db, userId));
    }

    public Task<Project> GetProjectById(int projectId)
    {
      return Task.FromResult(projects.GetProjectById(db, projectId));
    }

    public Task<PublicProject> GetPublicProjectById(int projectId)
    {
      return Task.FromResult(projects.GetPublicProjectById(db, projectId));
    }

    public Task<List<PublicProject>> GetPublicProjects()
    {
      return Task.FromResult(projects.GetPublicProjects(db));
    }

    public List<int> GetReleaseMakerUserIds(int projectId)
    {
      return userProjects.GetUserIdsWithRole(db, 0, projectId);
    }
  }
}