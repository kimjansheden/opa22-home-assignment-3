using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class GetAdCheckUserRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public int Id { get; set; }
    [Required]
    public bool BuyAd { get; set; }
}