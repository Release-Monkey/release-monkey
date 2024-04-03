namespace ReleaseMonkey.Server.Models
{
  public record UserProject
  (
    int userId,
    int projectID,
    int role
  );
}