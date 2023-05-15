using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Server.Models;

[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public string Username { get; set; }
    [Required]
    public string Password { get; set; }
    public bool LoggedIn { get; set; }
    public List<SellAd>? SellAds{ get; set; }
    public List<BuyAd>? BuyAds { get; set; }

    public User(string username, string password)
    {
        Username = username;
        Password = password;
        SellAds = new List<SellAd>();
        BuyAds = new List<BuyAd>();
        LoggedIn = false;
    }

    public User()
    {
        SellAds = new List<SellAd>();
        BuyAds = new List<BuyAd>();
    }

}