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
    public List<SellAd> SellAds{ get; set; }
    public List<BuyAd> ByAds { get; set; }

    public User(int id, string username, string password)
    {
        Id = id;
        Username = username;
        Password = password;
        SellAds = new List<SellAd>();
        ByAds = new List<BuyAd>();
        LoggedIn = false;
    }

    public User()
    {
    }
    
}