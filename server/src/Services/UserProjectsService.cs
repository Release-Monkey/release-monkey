using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Common;
using ReleaseMonkey.src.Repositories;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
  public class UserProjectsService(UserProjectsRepository userProjects, UsersRepository users, Db db)
  {

    public Task<UserProject> GetUserProjectById(int userProjectId)
    {
      return Task.FromResult(userProjects.GetUserProjectById(db, userProjectId));
    }

    public Task<UserProject> InsertUserProjectByUserID(int userId, int projectId, int role)
    {
      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          UserProject userProject = userProjects.InsertUserProject(transaction, db, userId, projectId, role);
          transaction.Commit();
          return Task.FromResult(userProject);
        }
        catch (Exception)
        {
          transaction.Rollback();
          throw;
        }
      }
    }

    public Task<UserProject> InsertUserProjectByEmail(string email, int projectId, int role)
    {

      User user;
      try
      {
        user = users.FindByEmail(email);
      }
      catch (System.Exception)
      {
        user = users.InsertOrUpdateUser("", email);
      }

      using (SqlTransaction transaction = db.Connection.BeginTransaction())
      {
        try
        {
          UserProject userProject = userProjects.InsertUserProject(transaction, db, user.Id, projectId, role);
          transaction.Commit();
          return Task.FromResult(userProject);
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
