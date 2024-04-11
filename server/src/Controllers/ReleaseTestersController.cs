using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;

namespace ReleaseMonkey.Server.Controller
{

  public record UpdateReleaseTesterRequest
  (
      int ReleaseTesterId,
      int State,
      string Comment
  );

  public record CreateReleaseTesterRequest
  (
      int ReleaseId,
      int TesterId
  );

  [ApiController]
  [Route("release-testers")]
  public class ReleaseTestersController(ReleaseTestersService releaseTesters) : ControllerBase
  {
    [HttpGet]
    public IActionResult Fetch()
    {
      return Ok(releaseTesters.GetAllReleaseTesters());
    }

    [HttpGet("{id:int}", Name = "FetchReleaseTesterById")]
    public IActionResult FetchById(int id)
    {
      try
      {
        return Ok(releaseTesters.GetReleaseTesterById(id));
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("testers/{id:int}", Name = "FetchReleaseTesterByTesterId")]
    public IActionResult FetchByTesterId(int id)
    {
      return Ok(releaseTesters.GetReleaseTestersByTesterId(id));
    }

    [HttpGet("release/{id:int}", Name = "FetchReleaseTesterByReleaseId")]
    public IActionResult FetchByReleaseId(int id)
    {
      return Ok(releaseTesters.GetReleaseTestersByReleaseId(id));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UpdateReleaseTesterRequest body)
    {
      var updatedReleaseTester = await releaseTesters.UpdateReleaseTester(body.ReleaseTesterId, body.State, body.Comment);
      return CreatedAtRoute("FetchReleaseTesterById", new { updatedReleaseTester.Id }, updatedReleaseTester);
    }
  }
}
