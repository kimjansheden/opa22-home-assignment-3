using System.Net.Http.Json;
using System.Text.Json;
using Client.Interfaces;
using Client.Models;

namespace Client.Helpers;

public class Helper
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;

    public Helper(IApp app)
    {
        _app = app;
    }

    public (string, string) PromptForCredentials()
    {
        string? password = "";
        string? username = "";
        while (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Enter Username.");
            username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("You must write a username.");
            }
            Console.WriteLine("Enter Password.");
            password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("You must write a password.");
            }
        }
        return (username, password);
    }

    public Task<(string? description, string? title, string category, bool? buyAd, int price, int length)>
        PromptForAdData()
    {
        string? category = "";
        string? title = "";
        string? description = "";
        bool? buyAd = null;
        int price = -1;
        int length = -1;

        // Prompt for the correct value for Buy or Sell.
        while (buyAd == null)
        {
            Console.WriteLine($"Do you want to buy or sell?{Environment.NewLine}1. Buy{Environment.NewLine}2. Sell");
            var buyOrSellPrompt = int.TryParse(Console.ReadLine(), out int buyOrSell);
            if (!buyOrSellPrompt || buyOrSell != 1 && buyOrSell != 2)
            {
                Console.WriteLine("You need to write either 1 or 2.");
                continue;
            }

            buyAd = buyOrSell == 1;
        }

        // Prompt for the correct value for Category.
        while (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine(
                $"Choose a category.{Environment.NewLine}1. Motor Boat{Environment.NewLine}2. Sail Boat{Environment.NewLine}3. Other");
            var categoryPrompt = int.TryParse(Console.ReadLine(), out int categoryInt);
            if (!categoryPrompt || categoryInt != 1 && categoryInt != 2 && categoryInt != 3)
            {
                Console.WriteLine("You must write either 1, 2 or 3.");
                continue;
            }

            switch (categoryInt)
            {
                case 1:
                    category = "motorboat";
                    break;
                case 2:
                    category = "sailboat";
                    break;
                case 3:
                    category = "other";
                    break;
            }
        }

        // Prompt for the correct value for Title.
        while (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Enter title.");
            title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("You must write a title.");
                continue;
            }
        }

        // Prompt for the correct value for Description.
        while (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Enter description.");
            description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("You must write a description.");
                continue;
            }
        }

        // Prompt for a Price.
        Console.WriteLine("If you want to set a price, write the price in SEK. Otherwise just press Enter.");
        var priceSuccess = int.TryParse(Console.ReadLine(), out price);
        if (!priceSuccess)
        {
            price = -1;
        }
            
        // Prompt for a Length.
        Console.WriteLine("If you want to set a length, write the length in cm. Otherwise just press Enter.");
        var lengthSuccess = int.TryParse(Console.ReadLine(), out length);
        if (!lengthSuccess)
        {
            length = -1;
        }
        return Task.FromResult((description, title, category, buyAd, price, length));
    }
    
    public async Task<(int adId, string description, string title, string category, bool? buyAd, int price, int length)> PromptForUpdateAd()
    {
        string? category = "";
        string? title = "";
        string? description = "";
        bool? buyAd = null;
        int price = -1;
        int length = -1;
        int adId = 0;
        Ad ad = null;

        // Prompt for the correct value for Buy or Sell.
        // Whether it's a buy ad or a sell ad will never change, so we can safely get that information from the client. The rest of the data we need to get from the server in order to ensure we have the latest information.
        while (buyAd == null)
        {
            Console.WriteLine($"Do you want to edit a Buy Ad or a Sell Ad?{Environment.NewLine}1. Buy{Environment.NewLine}2. Sell");
            var buyOrSellPrompt = int.TryParse(Console.ReadLine(), out int buyOrSell);
            if (!buyOrSellPrompt || buyOrSell != 1 && buyOrSell != 2)
            {
                Console.WriteLine("You need to write either 1 or 2.");
                continue;
            }

            buyAd = buyOrSell == 1;
        }
        
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "ad/get/byidanduser");
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        UpdateUri();
        
        // Prompt for the correct value for adId. (An ID can never be 0.)
        while (adId == 0)
        {
            Console.WriteLine(
                "Write the ID of the ad.");
            var idPrompt = int.TryParse(Console.ReadLine(), out adId);
            if (!idPrompt)
            {
                Console.WriteLine("You must write a valid number.");
                continue;
            }
        }

        // Check if the ad belongs to the logged in user and confirm it's the one the user wants to edit.
        string jsonString = "";
        try
        {
            var postData = new
            {
                Username = _app.CurrentUser.Username,
                Password = _app.CurrentUser.Password,
                Id = adId,
                BuyAd = buyAd
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, postData);

            // If the credentials were wrong and the server returns an unsuccessful status code.
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (errorResponse != null && errorResponse.ContainsKey("message"))
                {
                    Console.WriteLine($"Error: {errorResponse["message"]}");
                }
                else
                {
                    Console.WriteLine("An unknown error occurred.");
                }
                return (0, null, null, null, null, -1, -1);
            }
            
            jsonString = await response.Content.ReadAsStringAsync();

            if ((bool)buyAd)
            {
                ad = JsonSerializer.Deserialize<BuyAd>(jsonString, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });   
            }
            else
            {
                ad = JsonSerializer.Deserialize<SellAd>(jsonString, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General exception: {e.Message}");
        }
        
        var proceed = "";
        while (string.IsNullOrWhiteSpace(proceed))
        {
            Console.WriteLine($"You have chosen to edit your ad with title {ad.Title}. Do you want to proceed? Write Y or N.");
            proceed = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(proceed) || !(proceed.ToLower() == "y" || proceed.ToLower() == "n"))
            {
                Console.WriteLine("You must write Y or N.");
                continue;
            }
        }

        if (proceed == "n")
        {
            return (0, null, null, null, null, -1, -1);
        }
        
        // Prompt for Title.
        Console.WriteLine($"If you want to change the title, write the new title. Otherwise just press Enter.");
        
        var newTitle = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(newTitle))
        {
            newTitle = ad.Title;
        }
        
        // Prompt for Category.
        Console.WriteLine($"Category: {ad.Category}. If you want to change it, write the new category. Otherwise just press Enter.");
        
        // If the user wants to change category, they will write the new category. If not, the "new" category will remain the same as it is.
        var newCategory = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(newCategory))
        {
            newCategory = ad.Category;
        }

        // Prompt for a Price.
        var newPrice = -1;
        var priceStr = "";
        priceStr = ad.Price == null ? "Not set" : ad.Price.ToString();
        Console.WriteLine($"Price: {priceStr}. If you want to change/set the price, write the price in SEK. Otherwise just press Enter.");
        var priceSuccess = int.TryParse(Console.ReadLine(), out newPrice);
        if (!priceSuccess)
        {
            if (ad.Price == null)
            {
                newPrice = -1;
            }
            else
            {
                newPrice = (int)ad.Price;
            }
        }
            
        // Prompt for a Length.
        var newLength = -1;
        var lengthStr = "";
        lengthStr = ad.Length == null ? "Not set" : ad.Length.ToString();
        Console.WriteLine($"Length: {lengthStr}. If you want to change/set the length, write the length in cm. Otherwise just press Enter.");
        var lengthSuccess = int.TryParse(Console.ReadLine(), out newLength);
        if (!lengthSuccess)
        {
            if (ad.Length == null)
            {
                newLength = -1;
            }
            else
            {
                newLength = (int)ad.Length;
            }
        }
        
        // Prompt for a Description.
        Console.WriteLine($"Description: {ad.Description}. If you want to change the description, write the new description. Otherwise just press Enter.");
        var newDesc = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(newDesc))
        {
            newDesc = ad.Description;
        }
        
        return await Task.FromResult((adId, newDesc, newTitle, newCategory, buyAd, newPrice, newLength));
    }
    
    public async Task<(int adId, bool? buyAd)> PromptForDeleteAd()
    {
        bool? buyAd = null;
        int adId = 0;
        Ad ad = null;

        // Prompt for the correct value for Buy or Sell.
        // Whether it's a buy ad or a sell ad will never change, so we can safely get that information from the client. The rest of the data we need to get from the server in order to ensure we have the latest information.
        while (buyAd == null)
        {
            Console.WriteLine($"Do you want to delete a Buy Ad or a Sell Ad?{Environment.NewLine}1. Buy{Environment.NewLine}2. Sell");
            var buyOrSellPrompt = int.TryParse(Console.ReadLine(), out int buyOrSell);
            if (!buyOrSellPrompt || buyOrSell != 1 && buyOrSell != 2)
            {
                Console.WriteLine("You need to write either 1 or 2.");
                continue;
            }

            buyAd = buyOrSell == 1;
        }
        
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "ad/get/byidanduser");
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        UpdateUri();
        
        // Prompt for the correct value for adId. (An ID can never be 0.)
        while (adId == 0)
        {
            Console.WriteLine(
                "Write the ID of the ad.");
            var idPrompt = int.TryParse(Console.ReadLine(), out adId);
            if (!idPrompt)
            {
                Console.WriteLine("You must write a valid number.");
                continue;
            }
        }

        // Check if the ad belongs to the logged in user and confirm it's the one the user wants to edit.
        string jsonString = "";
        try
        {
            var postData = new
            {
                Username = _app.CurrentUser.Username,
                Password = _app.CurrentUser.Password,
                Id = adId,
                BuyAd = buyAd
            };
            HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, postData);

            // If the credentials were wrong and the server returns an unsuccessful status code.
            if (!response.IsSuccessStatusCode)
            {
                var errorResponse = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                if (errorResponse != null && errorResponse.ContainsKey("message"))
                {
                    Console.WriteLine($"Error: {errorResponse["message"]}");
                }
                else
                {
                    Console.WriteLine("An unknown error occurred.");
                }
                return (0, null);
            }
            
            jsonString = await response.Content.ReadAsStringAsync();

            if ((bool)buyAd)
            {
                ad = JsonSerializer.Deserialize<BuyAd>(jsonString, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });   
            }
            else
            {
                ad = JsonSerializer.Deserialize<SellAd>(jsonString, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General exception: {e.Message}");
        }
        
        var proceed = "";
        while (string.IsNullOrWhiteSpace(proceed))
        {
            Console.WriteLine($"You have chosen to delete your ad with title {ad.Title}. Do you want to proceed? Write Y or N.");
            proceed = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(proceed) || !(proceed.ToLower() == "y" || proceed.ToLower() == "n"))
            {
                Console.WriteLine("You must write Y or N.");
                continue;
            }
        }

        if (proceed == "n")
        {
            return (0, null);
        }

        return await Task.FromResult((adId, buyAd));
    }
    
    private void UpdateUri()
    {
        _uri = _app.CurrentUri;
    }

    private void CheckPropertiesForNull()
    {
        if (_uri == null || _client == null)
        {
            Initialize(_app);
        }
    }
    private Helper Initialize(IApp app)
    {
        _client = app.Client;
        _uri = app.CurrentUri;
        return this;
    }
}