﻿using cli.services;
using ReleaseMonkey.Server.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cli
{
    internal class Commands(LocalPreferencesServices preferencesServices, ApiService apiService, GithubService githubService)
    {
        public async Task LoginWithGithub()
        {
            var accessCode = await githubService.GetAccessCode();
            var user = await apiService.LoginUser(accessCode);
            preferencesServices.SetUser(user);

            Console.WriteLine($"Signed in as {user.Name}, {user.Email}.");
        }

        public Task Logout()
        {
            preferencesServices.ClearUser();
            preferencesServices.ClearProject();
            Console.WriteLine("Signed out. You will have to sign in again to publish releases.");
            return Task.FromResult<object?>(null);
        }

        public Task PrintCurrentUser()
        {
            var user = preferencesServices.GetUser();
            if (user == null)
            {
                Console.WriteLine("Not signed in.");
            }
            else
            {
                Console.WriteLine($"Signed in as {user.Name}, {user.Email}.");
            }

            return Task.FromResult<object?>(null);
        }

        public async Task CreateProject(string projectName, string githubRepo)
        {
            try
            {
                var project = await apiService.CreateProject(projectName, githubRepo);
                preferencesServices.SetProject(project);
                Console.WriteLine($"Project has been created. Project id is {project.Id}.");
            }
            catch (ApiException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task SetProject(string projectId)
        {
            try
            {
                var parsedProjectId = int.Parse(projectId);
                var project = await apiService.GetProjectById(parsedProjectId);
                preferencesServices.SetProject(project);
                Console.WriteLine($"Project has been set to {project.Name}, publishing {project.Repo}.");
            }
            catch (FormatException)
            {
                Console.WriteLine("Please enter a valid project id.");
            }

        }

        public Task PrintProject()
        {
            var project = preferencesServices.GetProject();
            if (project == null)
            {
                Console.WriteLine("No project set. Either create a new project or set project with 'set-project PROJECT_ID'.");
            }
            else
            {
                Console.WriteLine($"Current project is {project!.Name}, publishing {project.Repo}.");
            }

            return Task.FromResult<object?>(null);
        }

        public Task ListProjects() { throw new NotImplementedException(); }

        public Task AddTesters(List<string> testerEmails) { throw new NotImplementedException(); }

        public Task CreateRelease(string releaseName) { throw new NotImplementedException(); }

        public Task ListReleases() { throw new NotImplementedException(); }

        public Task ApproveRelease(string releaseId) { throw new NotImplementedException(); }

        public void PrintHelp()
        {
            Console.WriteLine("Welcome to Release Monkey. Use CLI to do everything.");
        }
    }
}
