using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Infrastructure;

namespace WebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly DataAccess _dataAccess;
    private readonly TokenProvider _tokenProvider;

    public AuthController(DataAccess dataAccess, TokenProvider tokenProvider)
    {
        _dataAccess = dataAccess;
        _tokenProvider = tokenProvider;
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

    [HttpPost]
    public ActionResult<AuthResponse> Login(AuthRequest request)
    {
        AuthResponse response = new AuthResponse();

        var user = _dataAccess.FindUserByEmail(request.Email);
        if (user is null)
        {
            return BadRequest("User is not found");
        }

        var verifyPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);

        if (!verifyPassword)
        {
            return BadRequest("Invalid password");
        }


        // Generate Access token
        var token = _tokenProvider.GenerateToken(user);
        response.AccessToken = token.AccessToken;

        // Generate Refresh token

        return response;
    }
}
