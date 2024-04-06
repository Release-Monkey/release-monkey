namespace ReleaseMonkey.Server.Models
{
  public record UserProject
  (
    int Id,
    int UserId,
    int ProjectID,
    int Role
  );
}