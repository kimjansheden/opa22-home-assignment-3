using Client.Interfaces;

namespace Client.Commands;

public class GetPersonsByIdCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;
    private ICommandExecutor _commandExecutor;
    public GetPersonsByIdCommand(IApp app)
    {
        _app = app;
    }
    public async Task Execute()
    {
        // Makes sure anything is not null, so we have every information we need.
        CheckPropertiesForNull();
        Console.WriteLine("Which ID?");
        var idToGet = Console.ReadLine();
        _app.CurrentUri = new Uri(_app.GetUserByUsernameUri + idToGet);
        _commandExecutor.ExecuteCommandIfExists("1");
    }

    public ICommand Initialize(IApp app)
    {
        _client = app.Client;
        _uri = app.CurrentUri;
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