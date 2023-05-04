using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class DatabaseContext : DbContext
{
    public DatabaseContext(DbContextOptions options) : base(options)
    {
    }
    
    public DbSet<BuyAd> BuyAds { get; set; }
    public DbSet<SellAd> SellAds { get; set; }
    public DbSet<User> Users { get; set; }
}