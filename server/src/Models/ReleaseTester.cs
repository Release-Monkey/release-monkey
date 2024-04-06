namespace ReleaseMonkey.Server.Models
{
  public record ReleaseTester
  (
    int ReleaseTesterId,
    int ReleaseId,
    int TesterId,
    int State,
    string Comment
  );
}