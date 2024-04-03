namespace ReleaseMonkey.Server.Models
{
  public record UserProject
  (
    int id,
    int userId,
    int projectID,
    int role
  );
}