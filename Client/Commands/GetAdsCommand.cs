using System.Text.Json;
using Client.Interfaces;
using Client.Models;

namespace Client.Commands;

public class GetAdsCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;

    public GetAdsCommand(IApp app)
    {
        _app = app;
    }
    public async Task Execute()
    {
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        
        var buyAds = await GetBuyAds();
        var sellAds = await GetSellAds();

        Console.WriteLine($"There are currently {buyAds.Count + sellAds.Count} ads on the site.");
        
        // Print BuyAds
        if (buyAds != null)
        {
            Console.WriteLine("Buy Ads");
            Console.WriteLine();
            foreach (var ad in buyAds)
            {
                ad.PrintAd();
            }
        }

        // Print SellAds
        if (sellAds != null)
        {
            Console.WriteLine("Sell Ads");
            Console.WriteLine();
            foreach (var ad in sellAds)
            {
                ad.PrintAd();
            }
        }
    }

    private async Task<List<BuyAd>> GetBuyAds()
    {
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "ad/buyads");
        UpdateUri();
        string jsonString = "";
        HttpResponseMessage response = await _client.GetAsync(_uri);

        jsonString = await response.Content.ReadAsStringAsync();

        List<BuyAd>? buyAds = JsonSerializer.Deserialize<List<BuyAd>>(jsonString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        return buyAds;
    }
    
    private async Task<List<SellAd>> GetSellAds()
    {
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "ad/sellads");
        UpdateUri();
        string jsonString = "";
        HttpResponseMessage response = await _client.GetAsync(_uri);

        jsonString = await response.Content.ReadAsStringAsync();

        List<SellAd>? sellAds = JsonSerializer.Deserialize<List<SellAd>>(jsonString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });

        return sellAds;
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

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = app.CurrentUri;
        return this;
    }
}