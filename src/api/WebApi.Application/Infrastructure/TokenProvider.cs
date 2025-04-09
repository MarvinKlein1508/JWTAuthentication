using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using WebApi.Application.Models;

namespace WebApi.Application.Infrastructure;

public class TokenProvider
{
    private readonly IConfiguration _configuration;

    public TokenProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Token GenerateToken(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        return new Token
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    private RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString(),
            Expires = DateTime.Now.AddMonths(1),
            CreatedDate = DateTime.Now,
            IsEnabled = true
        };

        return refreshToken;
    }
    private string GenerateAccessToken(User user)
    {
        string secretKey = _configuration["Jwt:SecretKey"]!;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));  
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            ]),
            Expires = DateTime.Now.AddSeconds(15),
            SigningCredentials = credentials,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var tokenHandler = new JsonWebTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return token;
    }
}

public class Token
{
    public string AccessToken { get; set; }

    public RefreshToken RefreshToken { get; set; }
}
