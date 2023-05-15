using System.Net.Http.Json;
using Client.Exceptions;
using Client.Helpers;
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
        
        _app.CurrentUri = new Uri(_app.DefaultApiUri + "user/new");
        UpdateUri();
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();

        var (username, password) = _app.Helper.PromptForCredentials();

        var postData = new { Username = username, Password = password };
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