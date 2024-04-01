﻿using cli;
using cli.services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        LocalPreferencesServices localPreferencesServices = new();
        AuthService authService = new(localPreferencesServices);
        ApiService apiService = new(authService);

        Commands commands = new(authService, apiService);

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
                    if (args.Length > 0)
                    {
                        await commands.SetProject(args[1]);
                    }
                    else
                    {
                        Console.WriteLine("Please provide the project id.");
                    }
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