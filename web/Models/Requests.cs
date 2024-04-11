namespace ReleaseMonkeyWeb.Requests
{
  public record ReleaseTester(int Id, int ReleaseId, int TesterId, int State, string Comment);
  public record Tester(string Email, int ProjectId, int Role);
}