using System.Text.Json;

namespace cli.models
{
    internal record User(string Email, string DisplayName);

    internal record Project(int Id, string Name, string Repo);
}
