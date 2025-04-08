using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RessourceController : ControllerBase
{
    [Authorize]
    [HttpGet]
    [Route("verfiy")]
    public ActionResult Verify()
    {
        return Ok("You are authorized");
    }
}
