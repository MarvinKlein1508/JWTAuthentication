using Microsoft.AspNetCore.Mvc;
using WebApi.DTO;
using WebApi.Infrastructure;
using WebApi.Models;

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
        response.RefreshToken = token.RefreshToken.Token;
        _dataAccess.DisableUserTokenByEmail(user.Email);
        _dataAccess.InsertRefreshToken(token.RefreshToken, user.Email);

  

        return Ok(response);
    }

    [HttpPost("refresh")]
    public ActionResult<AuthResponse> RefreshToken()
    {
        AuthResponse response = new AuthResponse();
        var refreshToken = Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            return BadRequest();
        }

        var isValid = _dataAccess.IsRefreshTokenValid(refreshToken);

        if (!isValid)
        {
            return BadRequest();
        }


        var currentUser = _dataAccess.FindUserByToken(refreshToken);
        if (currentUser is null)
        {
            return BadRequest();
        }

        // Generate Access token
        var token = _tokenProvider.GenerateToken(currentUser);
        response.AccessToken = token.AccessToken;
        response.RefreshToken = token.RefreshToken.Token;
        
        _dataAccess.DisableUserToken(refreshToken);
        _dataAccess.InsertRefreshToken(token.RefreshToken, currentUser.Email);

        return Ok(response);
    }

    [HttpPost("logout")]
    public ActionResult Logout()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        if (refreshToken is not null)
        {
            _dataAccess.DisableUserToken(refreshToken);
        }

        return Ok();
    }
}
