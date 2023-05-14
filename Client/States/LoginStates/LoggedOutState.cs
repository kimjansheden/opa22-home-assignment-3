using System.Net.Http.Json;
using System.Text.Json;
using Client.AbstractClasses;
using Client.Commands;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;

namespace Client.States.LoginStates;

public class LoggedOutState : LoginState
{
    private HttpClient _client;
    private Uri _uri;
    public LoggedOutState(App app) : base(app)
    {
        
    }

    protected internal override async Task LoginLogoutHandle()
    {
        await AttemptLogin();
    }

    protected internal override void RequestHandle(ICommand currentCommand)
    {
        if (currentCommand is not LoginLogoutCommand && currentCommand is not AddUserCommand&& currentCommand is not GetAdsCommand)
        {
            MustBeLoggedIn();
        }
    }

    private void UpdateUri()
    {
        _uri = App.CurrentUri;
    }

    private void CheckPropertiesForNull()
    {
        if (_uri == null || _client == null)
        {
            Initialize(App);
        }
    }

    protected internal override LoggedOutState Initialize(IApp app)
    {
        _client = app.Client;
        _uri = app.CurrentUri;
        return this;
    }

    private async Task AttemptLogin()
    {
        var (username, password) = PromptForCredentials();
        App.CurrentUri = new Uri(App.GetUserByUsernameUri);
        
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
            
            App.CurrentUser = JsonSerializer.Deserialize<User>(jsonString, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request exception: {e.Message}");
            return;
        }
        catch (Exception e)
        {
            Console.WriteLine($"General exception: {e.Message}");
            return;
        }
        
        App.LoginState = App.LoginStates["LoggedIn"];
        App.CurrentUser.LoggedIn = true;
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

    private void MustBeLoggedIn()
    {
        throw new NotLoggedInException("You need to be logged in to do this.");
    }
}