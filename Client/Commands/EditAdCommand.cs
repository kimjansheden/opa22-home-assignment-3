using System.Net.Http.Json;
using Client.Exceptions;
using Client.Interfaces;
using Uri = System.Uri;

namespace Client.Commands;

public class EditAdCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;
    private ICommandExecutor _commandExecutor;

    public EditAdCommand(IApp app)
    {
        _app = app;
    }
    public async Task Execute()
    {
        // Check if the user is logged in.
        try
        {
            _app.LoginState.RequestHandle(this);
        }
        catch (NotLoggedInException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        
        // Prompt the user for the required data.
        var (adID, newDescription, newTitle, newCategory, buyAd, newPrice, newLength) = await _app.Helper.PromptForUpdateAd();
        
        // If everything from the Helper Method is null: abort.
        if (adID == 0 && newDescription == null && newTitle == null && newCategory == null && buyAd == null && newPrice == -1 && newLength == -1)
        {
            return;
        }

        await UpdateAd(adID, newDescription, newTitle, newCategory, buyAd, newPrice, newLength);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri(_app.DefaultApiUri + "ad/UpdateAd");
        _commandExecutor = app.CommandExecutor;
        return this;
    }

    private void CheckPropertiesForNull()
    {
        if (_uri == null || _client == null || _commandExecutor == null)
        {
            Initialize(_app);
        }
    }
    
    private async Task UpdateAd(int adId, string? description, string? title, string category, bool? buyAd, int price, int length)
    {
        var putData = new
        {
            Username = _app.CurrentUser.Username,
            Password = _app.CurrentUser.Password,
            Id = adId,
            BuyAd = buyAd,
            Title = title,
            Description = description,
            Category = category,
            Price = price,
            Length = length
        };
        
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, putData);
        Console.WriteLine(response.StatusCode);
    }
}