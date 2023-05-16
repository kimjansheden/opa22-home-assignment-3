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
    
    [Route("sellads/byid")]
    [HttpGet]
    public SellAd GetSellAdById(int id)
    {
        SellAd sellAd = _db.SellAds.FirstOrDefault(ad => ad.Id == id);
        return sellAd;
    }
    
    // Authenticate the credentials received from the POST request and return the User object including all ads to the requester.
    [Route("get/byidanduser")]
    [HttpPost]
    public IActionResult GetAdCheckUser([FromBody] GetAdCheckUserRequest request)
    {
        _logger.LogInformation("Authenticating user");
        
        User? user = _db.Users.Include(u => u.BuyAds).Include(u => u.SellAds).FirstOrDefault(u => u.Username == request.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", request.Username);
            return BadRequest(new { message = "A user with this username does not exist" });
        }
        if (user.Password != request.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", request.Password, request.Username);
            return BadRequest(new { message = "Incorrect password" });
        }
        _logger.LogInformation("{User} successfully authenticated", request.Username);
        
        // Declare the ad.
        Ad ad;

        // Assign the type of ad to the ad variable.
        var adBelongsToUser = false;
        if (request.BuyAd)
        {
            ad = new BuyAd();
            ad = _db.BuyAds.Find(request.Id);
            if (ad != null)
            {
                adBelongsToUser = user.BuyAds.Contains(ad);
            }
        }
        else
        {
            ad = new SellAd();
            ad = _db.SellAds.Find(request.Id);
            if (ad != null)
            {
                adBelongsToUser = user.SellAds.Contains(ad);
            }
        }

        if (ad == null || !adBelongsToUser)
        {
            _logger.LogInformation("A {Adtype} with ID {Id} does not belong to you", ad.GetType(), request.Id);
            return BadRequest(new { message = $"A {ad.GetType()} with ID {request.Id} does not belong to you" });
        }
        
        _logger.LogInformation("Ad belongs to user. Returning ad");
        return Ok(JsonSerializer.Serialize(ad));
    }
    
    [Route("buyads")]
    [HttpGet]
    public List<BuyAd> GetBuyAds()
    {
        BuyAds = _db.BuyAds.ToList();
        _logger.LogInformation($"BuyAds count: {BuyAds.Count}");
        return BuyAds;
    }
    
    [Route("buyads/byid")]
    [HttpGet]
    public BuyAd GetBuyAdById(int id)
    {
        BuyAd buyAd = _db.BuyAds.FirstOrDefault(ad => ad.Id == id);
        return buyAd;
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
    
    [Route("UpdateAd")]
    [HttpPut]
    public IActionResult UpdateAd([FromBody] UpdateAdRequest request)
    {
        Ad ad = null;
        _logger.LogInformation("Attempting to log in");
        
        User? user = _db.Users.Include(u => u.BuyAds).Include(u => u.SellAds).FirstOrDefault(u => u.Username == request.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", request.Username);
            return BadRequest(new { message = "A user with this username does not exist" });
        }
        if (user.Password != request.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", request.Password, request.Username);
            return BadRequest(new { message = "Incorrect password" });
        }
        _logger.LogInformation("{User} successfully authenticated", request.Username);

        // Find the ad to update.
        if (request.BuyAd)
        {
            ad = user.BuyAds.FirstOrDefault(ad => ad.Id == request.Id);
        }
        else
        {
            ad = user.SellAds.FirstOrDefault(ad => ad.Id == request.Id);
        }

        // If the ad is not found.
        if (ad == null && request.BuyAd)
        {
            _logger.LogInformation("Buy Ad with ID {Id} not found for {Username}", request.Id, request.Username);
            return NotFound($"Buy Ad with ID {request.Id} not found for {request.Username}");
        }
        if (ad == null && !request.BuyAd)
        {
            _logger.LogInformation("Sell Ad with ID {Id} not found for {Username}", request.Id, request.Username);
            return NotFound($"Sell Ad with ID {request.Id} not found for {request.Username}");
        }
        _logger.LogInformation("Ad of type {AdType} with ID {Id} was found for {Username}", ad.GetType(), request.Id, request.Username);

        // Don't update the properties where there are no changes. Update where there are.
        if (request.Description != ad.Description)
        {
            ad.Description = request.Description;
        }
        if (request.Category != ad.Category)
        {
            ad.Category = request.Category;
        }
        if (request.Title != ad.Title)
        {
            ad.Title = request.Title;
        }
        ad.Length = request.Length == -1 ? null : request.Length;
        ad.Price = request.Price == -1 ? null : request.Price;
        
        _db.SaveChanges();
        
        _logger.LogInformation("Ad {AdTitle} updated for {Username}", ad.Title, request.Username);
        return Ok($"Ad {ad.Title} updated for {request.Username}");
    }
    
    [Route("DeleteAd")]
    [HttpPost]
    public IActionResult DeleteAd([FromBody] GetAdCheckUserRequest request)
    {
        Ad ad = null;
        _logger.LogInformation("Authenticating user");
        
        User? user = _db.Users.Include(u => u.BuyAds).Include(u => u.SellAds).FirstOrDefault(u => u.Username == request.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", request.Username);
            return BadRequest(new { message = "A user with this username does not exist" });
        }
        if (user.Password != request.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", request.Password, request.Username);
            return BadRequest(new { message = "Incorrect password" });
        }
        _logger.LogInformation("{User} successfully authenticated", request.Username);

        // Find the ad to update.
        if (request.BuyAd)
        {
            ad = user.BuyAds.FirstOrDefault(ad => ad.Id == request.Id);
        }
        else
        {
            ad = user.SellAds.FirstOrDefault(ad => ad.Id == request.Id);
        }

        // If the ad is not found.
        if (ad == null && request.BuyAd)
        {
            _logger.LogInformation("Buy Ad with ID {Id} not found for {Username}", request.Id, request.Username);
            return NotFound($"Buy Ad with ID {request.Id} not found for {request.Username}");
        }
        if (ad == null && !request.BuyAd)
        {
            _logger.LogInformation("Sell Ad with ID {Id} not found for {Username}", request.Id, request.Username);
            return NotFound($"Sell Ad with ID {request.Id} not found for {request.Username}");
        }
        _logger.LogInformation("Ad of type {AdType} with ID {Id} was found for {Username}", ad.GetType(), request.Id, request.Username);

        // Remember the title before deleting the ad.
        var adTitle = ad.Title;
        
        // Delete the ad
        if (ad is BuyAd buyAd)
        {
            user.BuyAds.Remove(buyAd);
        }
        if (ad is SellAd sellAd)
        {
            user.SellAds.Remove(sellAd);
        }

        _db.SaveChanges();
        
        _logger.LogInformation("Ad {AdTitle} deleted for {Username}", adTitle, request.Username);
        return Ok($"Ad {adTitle} deleted for {request.Username}");
    }
}