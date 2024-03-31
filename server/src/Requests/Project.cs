namespace ReleaseMonkey.Server.Request
{
  public record CreateProject
  (
    string Name,
    string Repository
  );
  
  public record UpdateProject
  (
    string Name,
    string Repository
  );
}