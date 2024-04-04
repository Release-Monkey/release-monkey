namespace ReleaseMonkey.Server.Models
{
  public record Project
  (
    int ProjectId,
    string Name,
    string Repository,
    string Token
  );
}