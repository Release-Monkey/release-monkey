using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Models;
using ReleaseMonkey.Server.Services;

namespace ReleaseMonkey.Server.Controller
{

  public record CreateUserProjectRequest
  (
      int UserId,
      int ProjectId,
      int Role
  );

  public record CreateUserProjectViaEmailRequest
  (
      string Email,
      int ProjectId,
      int Role
  );

  [ApiController]
  [Route("user-projects")]
  public class UserProjectsController(UserProjectsService userProjects) : ControllerBase
  {

    [HttpGet("{id:int}", Name = "FetchUserProjectById")]
    public IActionResult FetchById(int id)
    {
      try
      {
        return Ok(userProjects.GetUserProjectById(id));
      }
      catch (KeyNotFoundException e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserProjectRequest body)
    {
      var createdUserProject = await userProjects.InsertUserProjectByUserID(body.UserId, body.ProjectId, body.Role);
      return CreatedAtRoute("FetchUserProjectById", new { createdUserProject.Id }, createdUserProject);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateUserProjectViaEmailRequest body)
    {
      var createdUserProject = await userProjects.InsertUserProjectByEmail(body.Email, body.ProjectId, body.Role);
      return CreatedAtRoute("FetchUserProjectById", new { createdUserProject.Id }, createdUserProject);
    }

  }
}