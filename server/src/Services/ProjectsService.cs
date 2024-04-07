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
    public Task<Project> CreateProject(int userId, string projectName, string githubRepo, string token)
    {
      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          Project project = projects.InsertProject(transaction, db, projectName, githubRepo, token);
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

    public Task<Project> GetProjectById(int projectId)
    {
      return Task.FromResult(projects.GetProjectById(db, projectId));
    }

    public List<int> GetReleaseMakerUserIds(int projectId)
    {
      return userProjects.GetUserIdsWithRole(db, 1, projectId);
    }
  }
}