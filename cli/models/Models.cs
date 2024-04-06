using System.Text.Json;

namespace cli.models
{
    internal record User(int Id, string Email, string Name, string Token);

    internal record Project(int Id, string Name, string Repo);

    internal record Release(int Id, string ReleaseName, int ProjectId);
}
