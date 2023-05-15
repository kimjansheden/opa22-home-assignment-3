using Client.Interfaces;

namespace Client.Commands;

public class LoginLogoutCommand : ICommand
{
    private IApp _app;
    public LoginLogoutCommand(IApp app)
    {
        _app = app;
    }
    public async Task Execute()
    {
        await _app.LoginState.LoginLogoutHandle();
    }

    public ICommand Initialize(IApp app)
    {
        return this;
    }
}