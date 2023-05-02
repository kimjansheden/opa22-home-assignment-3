namespace Server.Models;

public class BuyAd:Ad
{
    public BuyAd(string title, string description, string category, string imageUrl, int price, int length) : base(title, description, category, imageUrl, price, length)
    {
    }
}