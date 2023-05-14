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
}