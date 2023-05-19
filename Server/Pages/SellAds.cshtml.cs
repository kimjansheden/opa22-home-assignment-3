using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;


namespace Server.Pages
{
    public class SellAds : PageModel
    {
      
        public List<User> Users { get; set; }
        public List<SellAd> AllSellAds { get; set; }
        public DatabaseContext Context { get; set; }
        [BindProperty]
        public User TheUser { get; set; }

        public SellAds(DatabaseContext context)
        {
            Context = context;
            Users = new List<User>();
            AllSellAds = new List<SellAd>();
        }

        public void OnGet()
        {
            try
            {
                AllSellAds = Context.SellAds.ToList();
            }
            catch (Exception e)
            {
                // ignored
            }
        }
    }
}