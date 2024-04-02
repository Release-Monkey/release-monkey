namespace ReleaseMonkey.Server.Models
{
  public record Release
  (
    int ReleaseId,
    string ReleaseName,
    int ProjectId
  );
}