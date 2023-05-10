namespace Server.Models;

public class SellAd:Ad
{
    public SellAd(int id, string title, string description, string category, string imageUrl, int price, int length) : base(id, title, description, category, imageUrl, price, length)
    {
    }

    public SellAd() : base()
    {
    }
}