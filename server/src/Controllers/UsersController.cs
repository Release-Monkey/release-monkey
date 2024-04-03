using Microsoft.AspNetCore.Mvc;
using ReleaseMonkey.Server.Services;

namespace ReleaseMonkey.Server.Controllers
{
    public record AddUserRequest(string AccessCode);

    [ApiController]
    [Route("users")]
    public class UsersController(UsersService usersService) : ControllerBase
    {
        [HttpGet("{id:int}", Name = "FetchUserById")]
        public IActionResult Fetch(int id)
        {
            try
            {
                return Ok(usersService.GetUserById(id));
            }
            catch(KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post(AddUserRequest request)
        {
            var user = await usersService.SignInWithGithubAccessCode(request.AccessCode);
            return CreatedAtRoute("FetchUserById", new { user.Id }, user);
        }
    }
}
