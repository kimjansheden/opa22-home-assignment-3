using System.ComponentModel.DataAnnotations;

namespace Server.Models;

public abstract class Ad
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Title { get; set; }
    public string Description { get; set; }
    public int? Price { get; set; }
    [Required]
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
}