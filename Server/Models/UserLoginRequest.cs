using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class UserLoginRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
}