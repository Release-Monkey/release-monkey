﻿using Microsoft.Data.SqlClient;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Repositories;
using ReleaseMonkey.src.Common;
using ReleaseMonkey.src.Repositories;
using System.Data.Common;
using System.Transactions;

namespace ReleaseMonkey.Server.Services
{
    public class ReleaseTestersService(ReleaseTestersRepository releaseTesters, ReleasesRepository releases, ProjectsRepository projects, UserProjectsRepository userProjects, UsersRepository users, Db db, GithubService github)
    {
        public Task<List<ReleaseTester>> GetAllReleaseTesters()
        {
            return Task.FromResult(releaseTesters.GetAllReleaseTesters(db));
        }

        public Task<List<ReleaseTester>> GetReleaseTestersByReleaseId(int releaseId)
        {
            return Task.FromResult(releaseTesters.GetReleaseTestersByReleaseId(db, releaseId));
        }

        public Task<List<ReleaseTester>> GetReleaseTestersByTesterId(int testerId)
        {
            return Task.FromResult(releaseTesters.GetReleaseTestersByTesterId(db, testerId));
        }

        public Task<ReleaseTester> GetReleaseTesterById(int releaseTesterId)
        {
            return Task.FromResult(releaseTesters.GetReleaseTesterById(db, releaseTesterId));
        }

        public async Task<ReleaseTester> UpdateReleaseTester(int ReleaseTesterId, int State, string Comment)
        {
            ReleaseTester releaseTester = releaseTesters.UpdateReleaseTester(db, ReleaseTesterId, State, Comment);
            int releaseState = GetReleaseState(releaseTester.ReleaseId);
            if (releaseState == 1) 
            {
                Release release = releases.GetReleaseById(db, releaseTester.ReleaseId);
                Project project = projects.GetProjectById(db, release.ProjectId);
                var userIds = from userProject in userProjects.GetUsersForProject(db, project.Id) select userProject.UserId;
                Email.sendEmail(users.GetUserEmailsByIds(db, userIds.ToList()), release.ReleaseName, project.Name, "", Email.RejectedRelease);
            } else if (releaseState == 0)
            {
                Release release = releases.GetReleaseById(db, releaseTester.ReleaseId);
                Project project = projects.GetProjectById(db, release.ProjectId);
                var userIds = from userProject in userProjects.GetUsersForProject(db, project.Id) select userProject.UserId;
                github.ReleaseProject(project.Repo, project.Token, release.ReleaseName);
                Email.sendEmail(users.GetUserEmailsByIds(db, userIds.ToList()), release.ReleaseName, project.Name, "", Email.AcceptedRelease);
            }
            return releaseTester;
        }

        public Task<ReleaseTester> CreateReleaseTester(int ReleaseId, int TesterId)
        {
            ReleaseTester releaseTester = releaseTesters.InsertReleaseTester(db, ReleaseId, TesterId);
            return Task.FromResult(releaseTester);
        }

        public int GetReleaseState(int ReleaseId)
        {
            List<ReleaseTester> releaseTesterList = releaseTesters.GetReleaseTestersByReleaseId(db, ReleaseId);
            var releaseStatesQuery = from tester in releaseTesterList select tester.State;
            var releaseStates = releaseStatesQuery.ToList();
            if (releaseStates.Contains(2)) 
            {
                return 2;
            } else if (releaseStates.Contains(1))
            {
                return 1;
            } else
            {
                return 0;
            }
        }
    }
}
