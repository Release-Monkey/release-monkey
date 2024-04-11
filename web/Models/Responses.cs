namespace ReleaseMonkeyWeb.Responses
{
    public record ReleaseTester(int Id, int ReleaseId, int TesterId, int State, string Comment);
    public record UserProject(int Id, int UserId, int ProjectId, int Role);
    public record Project(int Id, string Name, string Repo, string Token, Boolean PublicProject);
}