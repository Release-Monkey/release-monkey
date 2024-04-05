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
        [HttpGet]
        public IActionResult Fetch()
        {
            return Ok(releases);
        }

        [HttpGet("{id:int}", Name = "FetchReleaseById")]
        public IActionResult Fetch(int id)
        {
            /*return Ok(projects.Find(release => release.Id == id));*/
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReleaseRequest body)
        {
            var user = HttpContext.Features.Get<UserWithToken>()!;
            var createdRelease = await releases.CreateRelease(body.Name, body.ProjectId);
            return CreatedAtRoute("FetchReleaseById", new { createdRelease.Id }, createdRelease);
        }
    }
}