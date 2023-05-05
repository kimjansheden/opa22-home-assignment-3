using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Server.Pages;

public class CreateAd : PageModel
{
    // OnGet - varje gång sidan laddas
    
    [BindProperty]
    public int Number { get; set; }
    

    public void OnPost()
    {
        
    }
    
    public void OnGet()
    {
        
    }
}