using Client.AbstractClasses;
using Client.Commands;
using Client.Exceptions;
using Client.Interfaces;

namespace Client.States.LoginStates;

public class LoggedInState : LoginState
{
    public LoggedInState(App app) : base(app)
    {
    }

    protected internal override void LoginLogoutHandle(ICommand currentCommand)
    {
        if (currentCommand is LoginLogoutCommand)
        {
            LogOut();
        }
    }

    private void LogOut()
    {
        App.LoginState = App.LoginStates["LoggedOut"];
        App.CurrentUser.LoggedIn = false;
        throw new NotLoggedOutException("You are now logged out.");
    }
    
}