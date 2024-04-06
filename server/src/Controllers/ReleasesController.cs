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
            return Ok(releases.GetAllReleases());
        }

        [HttpGet("{id:int}", Name = "FetchReleaseById")]
        public IActionResult FetchById(int id)
        {
            try { 
                return Ok(releases.GetReleaseById(id));
            }
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpGet("project/{id:int}", Name = "FetchReleaseByProjectId")]
        public IActionResult FetchByProjectId(int id)
        {
            return Ok(releases.GetReleasesByProjectId(id));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateReleaseRequest body)
        {
            var createdRelease = await releases.CreateRelease(body.Name, body.ProjectId);
            return CreatedAtRoute("FetchReleaseById", new { createdRelease.Id }, createdRelease);
        }
    }
}