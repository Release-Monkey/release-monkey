namespace ReleaseMonkeyWeb.Models
{
    public record User(int Id, string Email, string Name, string Token);
    public record ReleaseTester(int Id, Release Release, int TesterId, int State, string Comment);
    public record Release(int Id,  string ReleaseName, int ProjectId);
    public record PublicProject(int Id, string Name);
    public record UserProject(int Id, int UserId, int ProjectId, int Role);
}