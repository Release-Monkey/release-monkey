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
    public class ProjectsController(ProjectsService projects) : ControllerBase
    {
        private readonly ProjectsService projects = projects;

        [HttpGet]
        public IActionResult Fetch()
        {
            return Ok(projects);
        }

        [HttpGet("{id:int}", Name = "FetchProjectById")]
        public IActionResult Fetch(int id)
        {
            /*return Ok(projects.Find(project => project.Id == id));*/
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProjectRequest body)
        {
            var user = HttpContext.Features.Get<UserWithToken>()!;
            var createdProject = await projects.CreateProject(user.Id, body.ProjectName, body.Repo, user.Token);
            return CreatedAtRoute("FetchProjectById", new { createdProject.Id }, createdProject);
        }
        /*
                [HttpPut("{id:int}")]
                public IActionResult Put(int id, UpdateProjectRequest payload)
                {
                    var index = projects.FindIndex(project => project.Id == id);

                    if (index == -1) return NotFound();

                    projects[index] = new Project(
                      id,
                      payload.Name,
                      payload.Repository
                    );

                    return NoContent();
                }

                [HttpDelete("{id:int}")]
                public IActionResult Delete(int id)
                {
                    projects.RemoveAll(project => project.Id == id);

                    return NoContent();
                }*/
    }
}