using cli;
using cli.services;
using ReleaseMonkey.Server.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            await ParseArgs(args);
        }
        catch (ApiException e)
        {
            Console.WriteLine(e.Message);
        }
    }

    private static async Task ParseArgs(string[] args)
    {
        LocalPreferencesServices localPreferencesServices = new();
        ApiService apiService = new(localPreferencesServices);
        GithubService githubService = new("Iv1.2a4a99768f6b514e", 3001);
        Commands commands = new(localPreferencesServices, apiService, githubService);

        if (args.Length > 0)
        {
            switch (args[0])
            {
                case "login":
                    await commands.LoginWithGithub();
                    break;
                case "logout":
                    await commands.Logout();
                    break;
                case "user":
                    await commands.PrintCurrentUser();
                    break;
                case "create-project":
                    if (args.Length > 1)
                    {
                        await commands.CreateProject(args[1], args[2]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide the project name followed by the Github repo (owner/repo_name) to create a project.");
                    }
                    break;
                case "set-project":
                    if (args.Length > 1)
                    {
                        await commands.SetProject(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide the project id.");
                    }
                    break;
                case "project":
                    await commands.PrintProject();
                    break;
                case "list-projects":
                    await commands.ListProjects();
                    break;
                case "add-testers":
                    if (args.Length > 1)
                    {
                        var emails = args.Skip(1).ToList();
                        await commands.AddTesters(emails);
                    }
                    else
                    {
                        Console.WriteLine("Please provide at least one email address to add as tester.");
                    }
                    break;
                case "create-release":
                    if (args.Length > 1)
                    {
                        await commands.CreateRelease(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide a name for the project");
                    }
                    break;
                case "list-releases":
                    await commands.ListReleases();
                    break;
                case "approve-release":
                    if (args[0].Length > 1)
                    {
                        await commands.ApproveRelease(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide the id of the release to approve.");
                    }
                    break;
                case "release-key":
                    await commands.PrintReleaseKey();
                    break;
                case "load-release-key":
                    if (args.Length > 1)
                    {
                        await commands.LoadReleaseKey(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide a release key to load.");
                    }
                    break;
                case "repos":
                    await commands.ListRepos();
                    break;
                case "version":
                    commands.PrintVersion();
                    break;
                case "help":
                    commands.PrintHelp();
                    break;
                default:
                    Console.WriteLine("Invalid command given.");
                    commands.PrintHelp();
                    break;
            }
        }
        else
        {
            commands.PrintHelp();
        }
    }
}
