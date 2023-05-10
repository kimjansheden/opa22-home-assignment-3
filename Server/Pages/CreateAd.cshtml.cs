using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Server.Data;
using Server.Models;

namespace Server.Pages;

public class CreateAd : PageModel, IValidatableObject
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

    public CreateAd(DatabaseContext databaseContext, ILogger<CreateAd> logger)
    {
        DatabaseContext = databaseContext;
        _logger = logger;
        _invalidModelState = "ModelState is not valid";
    }


    public IActionResult OnPost()
    {
        Ad ad;
        var user = DatabaseContext.Users.FirstOrDefault(u => u.Username == TheUser.Username.Trim());
        if (user == null)
        {
            _logger.LogInformation("A user with username {Username} does not exist", TheUser.Username);
            return Page();
        }

        if (user.Password != TheUser.Password.Trim())
        {
            _logger.LogInformation("The password you have entered, {Password}, does not match the password of user {Username}", TheUser.Password, TheUser.Username);
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

        if (BuyOrSell == 1)
        {
            ad = BuyAd;
        }
        else
        {
            ad = SellAd;
        }

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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BuyOrSell != 1 && BuyOrSell != 2)
        {
            yield return new ValidationResult("Du måste välja om du vill köpa eller sälja.",
                new[] { nameof(BuyOrSell) });
        }

        if (BuyOrSell == 1 && BuyAd == null)
        {
            yield return new ValidationResult("Du måste ange information för en köp-annons.", new[] { nameof(BuyAd) });
        }
        
        if (BuyOrSell == 2 && SellAd == null)
        {
            yield return new ValidationResult("Du måste ange information för en sälj-annons.", new[] { nameof(SellAd) });
        }
    }
}