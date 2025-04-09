using System.ComponentModel.DataAnnotations;

namespace WebApi.Contracts.Requests;

public class RegisterRequest
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public string Role { get; set; }
}
