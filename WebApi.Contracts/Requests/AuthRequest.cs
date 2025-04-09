using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Requests;

public class AuthRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
