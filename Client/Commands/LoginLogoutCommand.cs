using System.Net.Http.Json;
using System.Text.Json;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;

namespace Client.Commands;

public class LoginLogoutCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;
    public LoginLogoutCommand(IApp app)
    {
        _app = app;
    }
    public async Task Execute()
    {
        // Make sure the user it logged out.
        try
        {
            _app.LoginState.LoginLogoutHandle(this);
        }
        catch (NotLoggedOutException e)
        {
            Console.WriteLine(e.Message);
            return ;
        }
        await AttemptLogin();
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

    private async Task AttemptLogin()
    {
        var (username, password) = PromptForCredentials();
        _app.CurrentUri = new Uri(_app.GetUserByUsernameUri);
        
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        UpdateUri();
        
        string jsonString = "";
        try
        {
            var postData = new { Username = username, Password = password };
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
            
            //Console.WriteLine(jsonString); //Hela Usern inkl. annonser
            
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
        
        _app.LoginState = _app.LoginStates["LoggedIn"];
        _app.CurrentUser.LoggedIn = true;
    }

    private (string, string) PromptForCredentials()
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
}