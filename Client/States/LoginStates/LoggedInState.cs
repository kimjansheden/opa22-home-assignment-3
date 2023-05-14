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

    protected internal override void LoginLogoutHandle()
    {
        LogOut();
    }

    protected internal override void RequestHandle(ICommand currentCommand)
    {
        
    }

    protected internal override LoginState Initialize(IApp app)
    {
        return this;
    }

    private void LogOut()
    {
        App.LoginState = App.LoginStates["LoggedOut"];
        App.CurrentUser.LoggedIn = false;
        Console.WriteLine("You are now logged out.");
    }
    
}