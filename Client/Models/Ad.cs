namespace Client.Models;

public abstract class Ad
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public int? Price { get; set; }
    public string Category { get; set; }
    public bool Active { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime TimeCreated { get; set; }
    public int? Length { get; set; }

    public Ad(int id, string title, string description, string category, string imageUrl, int price, int length)
    {
        Id = id;
        Price = price;
        Length = length;
        Title = title;
        Description = description;
        Category = category;
        ImageUrl = imageUrl;
        Active = true;
        TimeCreated = DateTime.Now;
    }

    public Ad()
    {
        Active = true;
        TimeCreated = DateTime.Now;
    }
    
    public void PrintAd()
    {
        var lengthStr = this.Length != null ? $" Length: {this.Length} cm." : string.Empty;
        var priceStr = this.Price != null ? $" Price: {this.Price} kr. " : string.Empty;
                        
        Console.WriteLine("ID: " + this.Id + " Title: " + this.Title + " ");
        Console.Write(
            $"Category: {this.Category}.{lengthStr} Time created: {this.TimeCreated}.{priceStr}");
        Console.WriteLine();
        Console.Write($"Description: {this.Description}");
        Console.WriteLine();
        Console.WriteLine();
    }
}