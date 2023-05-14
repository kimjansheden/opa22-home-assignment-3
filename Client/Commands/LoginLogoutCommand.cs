using System.Net.Http.Json;
using System.Text.Json;
using Client.Exceptions;
using Client.Interfaces;
using Client.Models;

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
        _app.LoginState.LoginLogoutHandle();
    }

    public ICommand Initialize(IApp app)
    {
        return this;
    }
}