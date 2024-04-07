using Microsoft.AspNetCore.Mvc;

namespace ReleaseMonkey.Server.Controllers
{
  [ApiController]
  [Route("home")]
  public class HomePageController : ControllerBase
  {
    [HttpGet]
    public IActionResult Get()
    {
      return Ok("Welcome to release monkey server");
    }
  }
}
