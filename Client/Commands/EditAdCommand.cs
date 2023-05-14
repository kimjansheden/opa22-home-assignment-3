using System.Net.Http.Json;
using System.Text.Json;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;
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
        // Make sure the user is logged in.
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
        var (person, idToUpdateSkillTo, skillToUpdate, newSkillName, newSkillLevel) = await GetUserInputForUpdateSkill();
        if (person == null && idToUpdateSkillTo == 0 && skillToUpdate == null && newSkillName == null && newSkillLevel == 0)
        {
            return;
        }

        await UpdateSkill(person, idToUpdateSkillTo, skillToUpdate, newSkillName, newSkillLevel);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri("https://localhost:7242/api/Person/updateSkill");
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
    
    private async Task<(User? person, int personId, string? skillToUpdate, string? newSkillName, int newSkillLevel)>
        GetUserInputForUpdateSkill()
    {
        Console.WriteLine("Which person ID?");
        var success = int.TryParse(Console.ReadLine(), out int personId);
        if (!success)
        {
            Console.WriteLine("Invalid ID.");
            return (null, 0, null, null, 0);
        }
        
        _app.CurrentUri = new Uri(_app.GetUserByUsernameUri + personId);

        var person = await GetPerson(_client, _app.CurrentUri);
        if (person == null)
        {
            Console.WriteLine($"No person with ID {personId}. Try again.");
            return (null, 0, null, null, 0);
        }

        Console.WriteLine("Which skill do you want to update?");
        var skillToUpdate = Console.ReadLine();
        
        var skillIndex = person.BuyAds.FindIndex(s => s.Title == skillToUpdate);
        if (skillIndex == -1)
        {
            Console.WriteLine($"No such skill exists for {person.Username}. Did you spell correctly? Please note that the Skill Name is case sensitive.");
            return (null, 0, null, null, 0);
        }
        
        Console.WriteLine("Do you want to change the name of the skill? Y/N");
        var changeNameOfSkill = Console.ReadLine();
        var newSkillName = "";
        if (changeNameOfSkill.ToLower() == "y")
        {
            Console.WriteLine("Type the new skill name.");
            newSkillName = Console.ReadLine();
        }

        Console.WriteLine("Do you want to change the level of the skill? Y/N");
        var changeLevelOfSkill = Console.ReadLine();
        int newSkillLevel = -1;
        if (changeLevelOfSkill.ToLower() == "y")
        {
            Console.WriteLine("Type the new skill level.");
            int.TryParse(Console.ReadLine(), out newSkillLevel);
        }
        
        // Uppdatera inte Skill Name om newSkillName är tom.
        if (newSkillName == "")
        {
            newSkillName = person.BuyAds[skillIndex].Title;
        }

        // Uppdatera inte Skill Level om newSkillLevel är -1.
        if (newSkillLevel == -1)
        {
            newSkillLevel = (int)person.BuyAds[skillIndex].Price;
        }

        return (person, personId, skillToUpdate, newSkillName, newSkillLevel);
    }
    private async Task UpdateSkill(User? person, int personId, string? skillToUpdate, string? newSkillName, int newSkillLevel)
    {
        var putData = new
        {
            Id = personId,
            OldSkillName = skillToUpdate,
            NewSkillName = newSkillName,
            NewLevel = newSkillLevel
        };
        
        HttpResponseMessage response = await _client.PutAsJsonAsync(_uri, putData);
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