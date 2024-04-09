using System.Text.Json;

namespace cli.models
{
  internal record User(int Id, string Email, string Name, string Token);

  internal record Project(int Id, string Name, string Repo, string Token, bool PublicProject);

  internal record Release(int Id, string ReleaseName, int ProjectId);

  internal record ReleaseKey(User User, Project Project);

  internal record UserProject(int Id, int UserId, int ProjectId, int Role);
}
