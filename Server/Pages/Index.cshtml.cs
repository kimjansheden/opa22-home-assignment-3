using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;

namespace Server.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public List<User> Users { get; set; }
        public DatabaseContext Context { get; set; }
        [BindProperty]
        public User TheUser { get; set; }

        public IndexModel(ILogger<IndexModel> logger, DatabaseContext context)
        {
            _logger = logger;
            Context = context;
            Users = new List<User>();
        }

        public void OnGet()
        {
            Users = Context.Users.ToList();
        }
    }
}