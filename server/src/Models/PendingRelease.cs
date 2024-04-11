namespace ReleaseMonkey.Server.Models
{
  public record PendingRelease(
    int ReleaseTesterId,
    string ReleaseName,
    string ProjectName,
    string DownloadLink
  );
}