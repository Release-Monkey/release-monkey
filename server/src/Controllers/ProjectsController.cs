using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;
using ReleaseMonkey.Server.Types;



namespace ReleaseMonkey.Server.Controller
{
  public record CreateProjectRequest
  (
      string ProjectName,
      string Repo,
      string Token,
      bool PublicProject
  );

  public record UpdateProjectRequest
  (
      string ProjectName,
      string Repo,
      string Token,
      bool PublicProject
  );

  [ApiController]
  [Route("projects")]
  public class ProjectsController(ProjectsService projects, GithubService githubService) : ControllerBase
  {
    private readonly ProjectsService projects = projects;

    [HttpGet]
    public async Task<IActionResult> Fetch(string? search, string? orderBy, string? sort, int? size, int? page) {
        var currentUser = HttpContext.Features.Get<UserWithToken>()!;

        var modifier = new Modifier(
            search ?? "",
            orderBy ?? "id",
            sort?.ToUpper() ?? "ASC",
            size ?? (size >= 1 ? size : 3),
            page ?? (page >= 1 ? page : 1)
        );

        var fetchedProjects = await projects.GetProjectsByUserId(modifier, currentUser.Id);
        return Ok(fetchedProjects);
    }
    

    [HttpGet("{id:int}", Name = "FetchProjectById")]
    public async Task<IActionResult> Fetch(int id)
    {
      var currentUser = HttpContext.Features.Get<UserWithToken>()!;

      try
      {
        var project = await projects.GetProjectById(id);
        var releaseMakerIds = projects.GetReleaseMakerUserIds(id);

        if (releaseMakerIds.Contains(currentUser.Id))
        {
          return Ok(project);
        }
        else
        {
          return Forbid("Cannot access projects for which you are not the release maker.");
        }
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("public/{id:int}", Name = "FetchPublicProjectById")]
    public async Task<IActionResult> FetchPublicById(int id)
    {
      try
      {
        return Ok(await projects.GetPublicProjectById(id));
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("public", Name = "FetchPublicProject")]
    public async Task<IActionResult> FetchPublic(int id)
    {
      
      return Ok(await projects.GetPublicProjects());
      
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest body)
    {
      var user = HttpContext.Features.Get<UserWithToken>()!;
      var userRepos = await githubService.ListRepos(user.Token);

      if (userRepos.Contains(body.Repo))
      {
        var tokenRepos = await githubService.ListRepos(body.Token);
        if (tokenRepos.Contains(body.Repo)) 
        { 
          var createdProject = await projects.CreateProject(user.Id, body.ProjectName, body.Repo, body.Token, body.PublicProject);
          return CreatedAtRoute("FetchProjectById", new { createdProject.Id }, createdProject);
        } else 
        {
          return Forbid($"Please ensure that the personal access token inserted has access to the given repo.");
        }
      }
      else
      {
        return Forbid($"Please use the full name of a repo you have access to. According to Github, you don't have access to {body.Repo}. The full name of a repo is owner_name/repo_name.");
      }
    }
  }
}