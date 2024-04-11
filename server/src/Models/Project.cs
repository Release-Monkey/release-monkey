namespace ReleaseMonkey.Server.Models
{
  public record Project
  (
    int Id,
    string Name,
    string Repo,
    string Token,
    bool PublicProject
  );
}