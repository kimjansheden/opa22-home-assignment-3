using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;

namespace Server.Pages;

public class CreateUser : PageModel
{
    public List<User> Users { get; set; }
    public DatabaseContext Context { get; set; }
    [BindProperty]
    public User TheUser { get; set; }

    public CreateUser(DatabaseContext context)
    {
        Context = context;
        Users = new List<User>();
    }
    public void OnGet()
    {
        
    }
    public IActionResult OnPost()
    {
        ModelState.Clear();
        if (!ModelState.IsValid)
        {
            return RedirectToPage("/Index");
        }

        Context.Users.Add(TheUser);
        Context.SaveChanges();
        return RedirectToPage("/Index");
    }
}