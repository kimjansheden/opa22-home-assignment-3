namespace Client.Models;

public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
    public bool LoggedIn { get; set; }
    public List<SellAd>? SellAds{ get; set; }
    public List<BuyAd>? BuyAds { get; set; }

    public User(int id, string username, string password)
    {
        Id = id;
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
    
    public void PrintInfo()
    {
        Console.Write("Id: " + Id + ", Name: " + Username + ", Ads: ");
        Console.WriteLine();
        if (SellAds.Count != 0)
        {
            Console.WriteLine("Sell Ads");
            foreach (SellAd sellAd in SellAds)
            {
                Console.WriteLine(sellAd.Title + " ");
            }

            if (BuyAds.Count != 0)
            {
                Console.WriteLine();
                Console.WriteLine("Buy Ads");
                foreach (BuyAd buyAd in BuyAds)
                {
                    Console.WriteLine(buyAd.Title + " ");
                }   
            }
        }
        else
        {
            Console.Write("None");
        }
        Console.WriteLine();
    }
}