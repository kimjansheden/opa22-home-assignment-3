namespace Server.Models;

public abstract class Ad
{
    public string Title { get; set; }
    
    public int Id { get; set; }

    public string Description { get; set; }

    public int Price { get; set; }

    public string Category { get; set; }
    
    public bool Active { get; set; }
    
    public string ImageUrl { get; set; }
    
    public DateTime TimeCreated { get; set; }

    public int Length { get; set; }

    public Ad(string title, string description, string category, string imageUrl, int price, int length)
    {
        //Id på något sätt från databasen
        Price = price;
        Length = length;
        Title = title;
        Description = description;
        Category = category;
        ImageUrl = imageUrl;
        Active = true;
        TimeCreated = DateTime.Now;
    }
}