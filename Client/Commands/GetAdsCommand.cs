using Client.Exceptions;
using Client.Interfaces;

namespace Client.Commands;

public class GetAdsCommand : ICommand
{
    private HttpClient _client;
    private Uri _uri;
    private IApp _app;

    public GetAdsCommand(IApp app)
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