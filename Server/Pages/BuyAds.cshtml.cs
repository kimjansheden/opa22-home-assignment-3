using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;


namespace Server.Pages
{
    public class BuyAds : PageModel
    {
        public List<BuyAd> AllBuyAds { get; set; }
        public DatabaseContext Context { get; set; }

        public BuyAds(DatabaseContext context)
        {
            Context = context;
            AllBuyAds = new List<BuyAd>();
        }

        public void OnGet()
        {
            try
            {
                AllBuyAds = Context.BuyAds.ToList();
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}