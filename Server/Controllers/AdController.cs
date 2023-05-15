using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Models;

namespace Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdController : ControllerBase
{
    private DatabaseContext _db;
    private readonly ILogger<UserController> _logger;
    private List<BuyAd> BuyAds { get; set; }
    private List<SellAd> SellAds { get; set; }

    public AdController(DatabaseContext db, ILogger<UserController> logger)
    {
        _db = db;
        _logger = logger;
    }
    [Route("sellads")]
    [HttpGet]
    public List<SellAd> GetSellAds()
    {
        SellAds = _db.SellAds.ToList();
        _logger.LogInformation($"SellAds count: {SellAds.Count}");
        return SellAds;
    }
    
    [Route("buyads")]
    [HttpGet]
    public List<BuyAd> GetBuyAds()
    {
        BuyAds = _db.BuyAds.ToList();
        _logger.LogInformation($"BuyAds count: {BuyAds.Count}");
        return BuyAds;
    }
    
    [Route("new")]
    [HttpPost]
    public IActionResult CreateNewAd([FromBody] CreateAdRequest createAdRequest)
    {
        // Authenticate the credentials received from the POST request.
        
        _logger.LogInformation("Authenticating user");
        
        User? user = _db.Users.Include(u => u.BuyAds).Include(u => u.SellAds).FirstOrDefault(u => u.Username == createAdRequest.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", createAdRequest.Username);
            return BadRequest(new { message = "A user with this username does not exist" });
        }
        if (user.Password != createAdRequest.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", createAdRequest.Password, createAdRequest.Username);
            return BadRequest(new { message = "Incorrect password" });
        }
        
        _logger.LogInformation("{User} successfully authenticated", createAdRequest.Username);
        
        // Create the ad.
        Ad ad;

        // Assign the type of ad to the ad variable.
        if (createAdRequest.BuyAd)
        {
            ad = new BuyAd();
        }
        else
        {
            ad = new SellAd();
        }

        // Verify that the correct category is received.
        if (!(createAdRequest.Category == "motorboat" || createAdRequest.Category == "sailboat" || createAdRequest.Category == "other"))
        {
            _logger.LogInformation("Wrong category, {Category}, is not allowed", createAdRequest.Category);
            return BadRequest(new { message = "Category not allowed" });
        }
        
        // Populate the ad with the values received from the request.
        ad.Category = createAdRequest.Category;
        ad.Description = createAdRequest.Description;
        ad.Title = createAdRequest.Title;
        ad.Length = createAdRequest.Length == -1 ? null : createAdRequest.Length;
        ad.Price = createAdRequest.Price == -1 ? null : createAdRequest.Price;
        
        // Add the ad to the user's list of Buy Ads and Sell Ads respectively, depending on whether it's a BuyAd or a SellAd.
        if (ad is BuyAd buyAd)
        {
            user.BuyAds.Add(buyAd);
        }
        else if (ad is SellAd sellAd)
        {
            user.SellAds.Add(sellAd);
        }
        
        _db.SaveChanges();
        
        _logger.LogInformation("New {Ad} added successfully", ad.GetType());
        return Ok();
    }
}