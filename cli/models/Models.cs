using System.Text.Json;

namespace cli.models
{
  internal record User(int Id, string Email, string Name, string Token);

  internal record Project(int Id, string Name, string Repo, string Token, bool PublicProject);

  internal record Release(int Id, string ReleaseName, int ProjectId);

  internal record ReleaseTester(int Id, int ReleaseId, int TesterId, int State, string Comment);

  internal record UpdateReleaseTesterModel(int Id, int State, string Comment);

  internal record PendingRelease(int ReleaseTesterId, string ReleaseName, string ProjectName, string DownloadLink);

  internal record ReleaseKey(User User, Project Project);

  internal record UserProject(int Id, int UserId, int ProjectId, int Role);
}
