namespace ReleaseMonkey.Server.Models
{
  public record User
  (
    int UserId,
    string Name,
    string EmailAddress
  );
}