
namespace ReleaseMonkeyWeb.Models
{
    public record User(int Id, string Email, string Name, string Token);

    public record PublicProject(int Id, string Name);

    public record UserProject(int Id, int UserId, int ProjectId, int Role);
}
