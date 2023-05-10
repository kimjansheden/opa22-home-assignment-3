namespace Server.Models;

public class BuyAd:Ad
{
    public BuyAd(int id, string title, string description, string category, string imageUrl, int price, int length) : base(id, title, description, category, imageUrl, price, length)
    {
    }

    public BuyAd() : base()
    {
    }
}