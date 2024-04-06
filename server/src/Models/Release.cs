namespace ReleaseMonkey.Server.Models
{
  public record Release
  (
    int Id,
    string ReleaseName,
    int ProjectId
  );
}