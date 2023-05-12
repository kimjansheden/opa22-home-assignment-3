using System.Net.Http.Json;
using System.Text.Json;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;
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
        // Make sure the user is logged in.
        try
        {
            _app.LoginState.LoginLogoutHandle(this);
        }
        catch (NotLoggedInException e)
        {
            Console.WriteLine(e.Message);
            return;
        }
        
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        var (personId, skillIndex) = await GetUserInputForUpdateSkill();
        if (personId == -1 && skillIndex == -1)
        {
            return;
        }

        await DeleteSkill(personId, skillIndex);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri("https://localhost:7242/api/Person/DeleteSkill");
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
    
    private async Task<(int personId, int skillIndex)>
        GetUserInputForUpdateSkill()
    {
        Console.WriteLine("Which person ID?");
        var success = int.TryParse(Console.ReadLine(), out int personId);
        if (!success)
        {
            Console.WriteLine("Invalid ID.");
            return (0, 0);
        }
        
        _app.CurrentUri = new Uri(_app.GetUserByUsernameUri + personId);

        var person = await GetPerson(_client, _app.CurrentUri);
        if (person == null)
        {
            Console.WriteLine($"No person with ID {personId}. Try again.");
            return (0, 0);
        }

        Console.WriteLine("Which skill do you want to delete?");
        var skillToDelete = Console.ReadLine();
        
        var skillIndex = person.BuyAds.FindIndex(s => s.Title == skillToDelete);
        if (skillIndex == -1)
        {
            Console.WriteLine($"No such skill exists for {person.Username}. Did you spell correctly? Please note that the Skill Name is case sensitive.");
            return (0, 0);
        }

        return (personId, skillIndex);
    }
    private async Task DeleteSkill(int personId, int skillIndex)
    {
        var postData = new
        {
            PersonId = personId,
            SkillToDeleteIndex = skillIndex
        };
        
        HttpResponseMessage response = await _client.PostAsJsonAsync(_uri, postData);
        Console.WriteLine(response.StatusCode);
    }
    private async Task<User?> GetPerson(HttpClient client, Uri uri)
    {
        HttpResponseMessage response = await client.GetAsync(uri);
        string jsonString = await response.Content.ReadAsStringAsync();
        User? person = JsonSerializer.Deserialize<User>(jsonString, new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        });
        return person;
    }
}