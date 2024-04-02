using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;



namespace ReleaseMonkey.Server.Controller
{

    public record CreateReleaseRequest
    (
        string Name,
        int ProjectId
    );

    [ApiController]
    [Route("releases")]
    public class ReleasesController(ReleasesService releases) : ControllerBase
    {
        private readonly ReleasesService releases = releases;

        [HttpGet]
        public IActionResult Fetch()
        {
            return Ok(projects);
        }

        [HttpGet("{id:int}", Name = "FetchProjectById")]
        public IActionResult Fetch(int id)
        {
            /*return Ok(projects.Find(release => release.Id == id));*/
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReleaseRequest body)
        {
            int userId = 1;
            var createdRelease = await projects.CreateRelease(body.Name, body.ProjectId);
            return CreatedAtRoute("FetchReleaseById", new { createdProject.Id }, createdRelease);
        }
    }
}