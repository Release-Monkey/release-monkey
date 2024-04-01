using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;



namespace ReleaseMonkey.Server.Controller
{
    public record CreateProjectRequest
    (
        string Name,
        string Repository
    );

    public record UpdateProjectRequest
    (
        string Name,
        string Repository
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
            int userId = 1;
            var createdProject = await projects.CreateProject(userId, body.Name, body.Repository, new Random().NextDouble().ToString());
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