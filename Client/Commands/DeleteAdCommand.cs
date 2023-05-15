using System.Net.Http.Json;
using Client.Exceptions;
using Client.Interfaces;
using Uri = System.Uri;

namespace Client.Commands;

public class DeleteAdCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;
    private ICommandExecutor _commandExecutor;

    public DeleteAdCommand(IApp app)
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
        var (adID, buyAd) = await _app.Helper.PromptForDeleteAd();
        
        // If everything from the Helper Method is null: abort.
        if (adID == 0 && buyAd == null)
        {
            return;
        }

        await DeleteAd(adID, buyAd);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri(_app.DefaultApiUri + "ad/DeleteAd");
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
    
    private async Task DeleteAd(int adId, bool? buyAd)
    {
        var postData = new
        {
            Username = _app.CurrentUser.Username,
            Password = _app.CurrentUser.Password,
            Id = adId,
            BuyAd = buyAd
        };
        
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, postData);
        Console.WriteLine(response.StatusCode);
    }
}