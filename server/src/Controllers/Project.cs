using Microsoft.AspNetCore.Mvc;

namespace ReleaseMonkey.Server.Controller
{
  [ApiController]
  [Route("projects")]
  public class Project : ControllerBase
  {
    private static List <Model.Project> projects = [
      new Model.Project(1, "Project 1", "Repository 1"),
      new Model.Project(2, "Project 2", "Repository 2"),
      new Model.Project(3, "Project 3", "Repository 3"),
      new Model.Project(4, "Project 4", "Repository 4"),
    ];

    [HttpGet]
    public IActionResult Fetch ()
    {
      return Ok(projects);
    }

    [HttpGet("{id:int}", Name = "FetchProjectById")]
    public IActionResult Fetch (int id)
    {
      return Ok(projects.Find(project => project.Id == id));
    }

    [HttpPost]
    public IActionResult Create (Request.CreateProject payload)
    {
      var id = projects.Count + 1;

      var project = new Model.Project(
        id,
        payload.Name,
        payload.Repository
      );

      projects.Add(project);

      return CreatedAtRoute("FetchProjectById", new { Id = id }, project);
    }

    [HttpPut("{id:int}")]
    public IActionResult Put (int id, Request.UpdateProject payload)
    {
      var index = projects.FindIndex(project => project.Id == id);

      if (index == -1) return NotFound();

      projects[index] = new Model.Project(
        id,
        payload.Name,
        payload.Repository
      );

      return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete (int id)
    {
      projects.RemoveAll(project => project.Id == id);

      return NoContent();
    }
  }
}