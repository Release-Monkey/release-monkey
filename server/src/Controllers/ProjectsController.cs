using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;
using ReleaseMonkey.Server.Types;



namespace ReleaseMonkey.Server.Controller
{
    public record CreateProjectRequest
    (
        int userId,
        string projectName,
        string repo
    );

    public record UpdateProjectRequest
    (
        string projectName,
        string repo
    );

    [ApiController]
    [Route("projects")]
    public class ProjectsController(ProjectsService projects) : ControllerBase
    {
        private readonly ProjectsService projects = projects;

        [HttpGet]
        public async Task<IActionResult> Fetch(string? search, string? orderBy, string? sort, int? size, int? page) {
            var modifier = new Modifier(
                search ?? "",
                orderBy ?? "id",
                sort?.ToUpper() ?? "ASC",
                size ?? (size >= 1 ? size : 3),
                page ?? (page >= 1 ? page : 1)
            );

            var fetchedProjects = await projects.FetchProjects(modifier);
            return Ok(fetchedProjects);
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
            var createdProject = await projects.CreateProject(body.userId, body.projectName, body.repo, new Random().NextDouble().ToString());
            return CreatedAtRoute("FetchProjectById", new { createdProject.id }, createdProject);
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