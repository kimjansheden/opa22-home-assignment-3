using System.Net.Http.Json;
using Client.Exceptions;
using Client.Interfaces;
using Uri = System.Uri;

namespace Client.Commands;

public class AddAdCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;

    public AddAdCommand(IApp app)
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
        
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "ad/new");
        UpdateUri();
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();

        var (description, title, category, buyAd, price, length) = await _app.Helper.PromptForAdData();
        
        var postData = new
        {
            Username = _app.CurrentUser.Username,
            Password = _app.CurrentUser.Password,
            BuyAd = buyAd,
            Title = title,
            Description = description,
            Category = category,
            Price = price,
            Length = length
        };
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, postData);
        Console.WriteLine(response.StatusCode);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = app.CurrentUri;
        return this;
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
}