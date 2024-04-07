using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;



namespace ReleaseMonkey.Server.Controller
{
  public record CreateProjectRequest
  (
      string ProjectName,
      string Repo
  );

  public record UpdateProjectRequest
  (
      string ProjectName,
      string Repo
  );

  [ApiController]
  [Route("projects")]
  public class ProjectsController(ProjectsService projects, GithubService githubService) : ControllerBase
  {
    private readonly ProjectsService projects = projects;

    [HttpGet]
    public IActionResult Fetch()
    {
      return Ok(projects);
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

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectRequest body)
    {
      if (userRepos.Contains(body.Repo))
      {
        var createdProject = await projects.CreateProject(user.Id, body.ProjectName, body.Repo, user.Token);
        return CreatedAtRoute("FetchProjectById", new { createdProject.Id }, createdProject);
      }
      else
      {
        return Forbid($"Please use the full name of a repo you have access to. According to Github, you don't have access to {body.Repo}. The full name of a repo is owner_name/repo_name.");
      }
    }
  }
}