using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.Server.Types;
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
                    userProjects.InsertUserProject(transaction, db, userId, project.id, 1);
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

        public Task<List<Project>> FetchProjects(Modifier modifier)
        {
            return Task.FromResult(projects.SelectProjects(modifier));
        }
    }
}