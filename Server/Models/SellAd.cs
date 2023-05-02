namespace Server.Models;

public class SellAd:Ad
{
    public SellAd(string title, string description, string category, string imageUrl, int price, int length) : base(title, description, category, imageUrl, price, length)
    {
    }
}