using System.Net.Http.Json;
using Client.Exceptions;
using Client.Interfaces;

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
        
        // Post
        Console.WriteLine("What skill do you want to add?");
        var skillToAdd = Console.ReadLine();
        Console.WriteLine("To which ID?");
        int.TryParse(Console.ReadLine(), out int idToAddSkillTo);

        var postData = new { Id = idToAddSkillTo, SkillName = skillToAdd };
        HttpResponseMessage response2 = await _client.PostAsJsonAsync(_uri, postData);
        Console.WriteLine(response2.StatusCode);
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri("https://localhost:7242/api/Person/AddSkill");
        return this;
    }
    
    private void CheckPropertiesForNull()
    {
        if (_uri == null || _client == null)
        {
            Initialize(_app);
        }
    }
}