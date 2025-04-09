using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ResourceController : ControllerBase
{
    [Authorize]
    [HttpGet]
    [Route("verify")]
    public ActionResult Verify()
    {
        return Ok("You are authorized");
    }
}
