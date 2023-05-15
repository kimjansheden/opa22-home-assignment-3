using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public class CreateAdRequest
{
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    [Required]
    public bool BuyAd { get; set; }
    [Required]
    public string Title { get; set; }
    [Required]
    public string Description { get; set; }

    [Required]
    public string Category { get; set; }

    public int? Price { get; set; }
    public int? Length { get; set; }
}