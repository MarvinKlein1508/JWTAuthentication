namespace WebApi.Application.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    public required string SecretKey { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required int LifetimeInMinutes { get; set; }
}
