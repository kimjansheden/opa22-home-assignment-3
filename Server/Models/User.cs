namespace Server.Models;

public class User
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool LoggedIn { get; set; }
    public List<SellAd> SellAds{ get; set; }
    public List<BuyAd> ByAds { get; set; }
    public int Id { get; set; }

    public User(string username, string password)
    {
        //Id
        Username = username;
        Password = password;
        SellAds = new List<SellAd>();
        ByAds = new List<BuyAd>();
        LoggedIn = false;
    }
    
}