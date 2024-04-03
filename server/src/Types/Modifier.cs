namespace ReleaseMonkey.Server.Types
{
  public record Modifier
  (
    string? searchTerm,
    string? orderBy,
    string? sort,
    int? size,
    int? page
  );
}