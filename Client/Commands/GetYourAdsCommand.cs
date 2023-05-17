using System.Net.Http.Json;
using System.Text.Json;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;

namespace Client.Commands;

public class GetYourAdsCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;

    public GetYourAdsCommand(IApp app)
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
        // Here we have a logged in user, otherwise we wouldn't have reached this code.
        // Then we can just fetch the user's ads from the database to always have the latest ones and then print them here.
       
        // Send the username and password of the already logged in user (CurrentUser.Username and CurrentUser.Password) as a POST Request to the server and authenticate.
        // Then update CurrentUser with the up to date User object we receive from the server and lastly print the ads.
            
            _app.CurrentUri = new Uri(_app.GetUserByUsernameUri);
        
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        UpdateUri();
        
        string jsonString = "";
        try
        {
            var postData = new { Username = _app.CurrentUser.Username, Password = _app.CurrentUser.Password };
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
                return;
            }
            
            jsonString = await response.Content.ReadAsStringAsync();

            // Download the logged in user's User object to get the current info from the database.
            _app.CurrentUser = JsonSerializer.Deserialize<User>(jsonString, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"General exception: {e.Message}");
        }

        _app.CurrentUser.PrintInfo();
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