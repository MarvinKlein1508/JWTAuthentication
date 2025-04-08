using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Infrastructure;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DataAccess _dataAccess;

    public AuthController(DataAccess dataAccess)
    {
        _dataAccess = dataAccess;
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] RegisterRequest request)
    {
        string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var result = _dataAccess.RegisterUser(request.Email, hashedPassword, request.Role);

        if (result)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }

    }
}
