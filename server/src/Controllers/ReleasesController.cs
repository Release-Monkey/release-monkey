using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;

namespace ReleaseMonkey.Server.Controller
{

  public record CreateReleaseRequest
  (
      string Name,
      int ProjectId,
      string DownloadLink
  );

  [ApiController]
  [Route("releases")]
  public class ReleasesController(ReleasesService releases) : ControllerBase
  {
    [HttpGet]
    public IActionResult Fetch()
    {
      var currentUser = HttpContext.Features.Get<UserWithToken>()!;

      return Ok(releases.GetAllReleases(currentUser.Id));
    }

    [HttpGet("{id:int}", Name = "FetchReleaseById")]
    public IActionResult FetchById(int id)
    {
      try
      {
        return Ok(releases.GetReleaseById(id));
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("user/{id:int}", Name = "FetchPendingReleasesByUserId")]
    public async Task<IActionResult> FetchPendingReleasesByUserId(int id)
    {
      try
      {
        return Ok(await releases.GetPendingReleasesByUserId(id));
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("project/{id:int}", Name = "FetchReleaseByProjectId")]
    public async Task<IActionResult> FetchByProjectId(int id)
    {
      return Ok(await releases.GetReleasesByProjectId(id));
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateReleaseRequest body)
    {
      var createdRelease = await releases.CreateRelease(body.Name, body.ProjectId, body.DownloadLink);
      return CreatedAtRoute("FetchReleaseById", new { createdRelease.Id }, createdRelease);
    }
  }
}