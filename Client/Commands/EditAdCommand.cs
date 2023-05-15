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
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = new Uri("https://localhost:7242/api/ad/edit");
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
}