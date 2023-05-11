using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;

namespace Server.Pages;

public class CreateAd : PageModel
{
    private DatabaseContext DatabaseContext { get; set; }
    private string? _invalidModelState;
    private readonly ILogger<CreateAd> _logger;
    [BindProperty]
    public User TheUser { get; set; }

    [BindProperty]
    public int BuyOrSell { get; set; }
    [BindProperty]
    public int Category { get; set; }

    [BindProperty]
    public BuyAd BuyAd { get; set; }

    [BindProperty]
    public SellAd SellAd { get; set; }
    
    public string TitleError { get; set; }
    public string DescriptionError { get; set; }

    public CreateAd(DatabaseContext databaseContext, ILogger<CreateAd> logger)
    {
        DatabaseContext = databaseContext;
        _logger = logger;
        _invalidModelState = "ModelState is not valid";
    }


    public IActionResult OnPost()
    {
        Ad ad;
        
        // Assign the type of ad to the ad variable.
        if (BuyOrSell == 1)
        {
            ad = BuyAd;
        }
        else
        {
            ad = SellAd;
        }

        if (string.IsNullOrWhiteSpace(TheUser.Username))
        {
            ModelState.AddModelError("TheUser.Username", "Du måste skriva in ditt användarnamn.");
            _logger.LogInformation("Username in the form is null or empty");
        }
        
        if (string.IsNullOrWhiteSpace(TheUser.Password))
        {
            ModelState.AddModelError("TheUser.Password", "Du måste skriva in ditt lösenord.");
            _logger.LogInformation("Password in the form is null or empty");
        }

        User? user = null;
        if (!string.IsNullOrWhiteSpace(TheUser.Username))
        {
            user = DatabaseContext.Users.FirstOrDefault(u => u.Username == TheUser.Username.Trim());    
        }
        
        if (BuyOrSell != 1 && BuyOrSell != 2)
        {
            ModelState.ClearValidationState("BuyOrSell");
               ModelState.AddModelError("BuyOrSell", "Du måste välja om du vill köpa eller sälja.");
            _logger.LogInformation("Buy or Sell is not chosen");
        }

        if (Category == 0)
        {
            ModelState.ClearValidationState("Category");
            ModelState.AddModelError("Category", "Du måste välja en kategori.");
            _logger.LogInformation("Category is not chosen");
        }
        
        if (user == null)
        {
            ModelState.AddModelError("TheUser.Username", "Det finns ingen med det användarnamnet.");
            _logger.LogInformation("A user with username {Username} does not exist", TheUser.Username);
        }

        if (!string.IsNullOrWhiteSpace(TheUser.Password) && user.Password != TheUser.Password.Trim())
        {
            ModelState.AddModelError("TheUser.Password", "Du har skrivit in fel lösenord.");
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", TheUser.Password, TheUser.Username);
        }

        if ((ad is BuyAd && string.IsNullOrWhiteSpace(BuyAd.Title)) || ad is SellAd && string.IsNullOrWhiteSpace(SellAd.Title))
        {
            ModelState.ClearValidationState("Title");
            ModelState.AddModelError("Title", "Du måste skriva in en rubrik.");
            _logger.LogInformation("Title in the form is null or empty");
            TitleError = ModelState["Title"].Errors[0].ErrorMessage;
        }
        
        if ((ad is BuyAd && string.IsNullOrWhiteSpace(BuyAd.Description)) || ad is SellAd && string.IsNullOrWhiteSpace(SellAd.Description))
        {
            ModelState.ClearValidationState("Description");
            ModelState.AddModelError("Description", "Du måste skriva in en beskrivning.");
            _logger.LogInformation("Description in the form is null or empty");
            DescriptionError = ModelState["Description"].Errors[0].ErrorMessage;
        }
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            foreach (var error in errors)
            {
                foreach (var errorMessage in error.Errors)
                {
                    _logger.LogError("Validation error in property {Property}: {ErrorMessage}", error.Key, errorMessage.ErrorMessage);
                }
            }
            _logger.LogInformation("{Message}", _invalidModelState);
            return Page();
        }

        // Assign the type of Category to the ad.
        if (Category == 1)
        {
            ad.Category = "motorboat";
        }
        else if (Category == 2)
        {
            ad.Category = "sailboat";
        }
        else
        {
            ad.Category = "other";
        }

        // Add the ad to the user's list of Buy Ads and Sell Ads respectively, depending on whether it's a BuyAd or a SellAd.
        if (ad is BuyAd buyAd)
        {
            user.BuyAds.Add(buyAd);
        }
        else if (ad is SellAd sellAd)
        {
            user.SellAds.Add(sellAd);
        }

        DatabaseContext.SaveChanges();
        
        _logger.LogInformation("New {Ad} added successfully", ad.GetType());

        return RedirectToPage("/Index");
    }
    
    // OnGet - varje gång sidan laddas
    public void OnGet()
    {
        
    }
}