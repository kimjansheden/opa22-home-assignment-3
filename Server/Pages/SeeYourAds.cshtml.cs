using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;

namespace Server.Pages
{
    public class SeeYourAds : PageModel
    {
        private readonly ILogger<SeeYourAds> _logger;
        public List<BuyAd> BuyAds { get; set; }
        public List<SellAd> SellAds { get; set; }
        public DatabaseContext Context { get; set; }
        [BindProperty]
        public User TheUser { get; set; }

        public SeeYourAds(ILogger<SeeYourAds> logger, DatabaseContext context)
        {
            _logger = logger;
            Context= context;
            BuyAds = new List<BuyAd>();
            SellAds = new List<SellAd>();
        }

        public void OnGet()
        {
            if (TheUser != null)
            {
                BuyAds = Context.BuyAds.Where(ad => ad.UserId == TheUser.Id).ToList();
                SellAds = Context.SellAds.Where(ad => ad.UserId == TheUser.Id).ToList();
            }
        }
        public void OnPost() 
        {
            TheUser = Context.Users.FirstOrDefault(u => u.Username == TheUser.Username && u.Password == TheUser.Password);
    
            if (TheUser != null)
            {
                BuyAds = Context.BuyAds.Where(ad => ad.UserId == TheUser.Id).ToList();
                SellAds = Context.SellAds.Where(ad => ad.UserId == TheUser.Id).ToList();
            }
        }
    }
}