using Client.AbstractClasses;
using Client.Interfaces;

namespace Client.States.LoginStates;

public class LoggedInState : LoginState
{
    public LoggedInState(App app) : base(app)
    {
    }

    protected internal override async Task LoginLogoutHandle()
    {
        await LogOut();
    }

    protected internal override void RequestHandle(ICommand currentCommand)
    {
        
    }

    protected internal override LoginState Initialize(IApp app)
    {
        return this;
    }

    private Task LogOut()
    {
        App.LoginState = App.LoginStates["LoggedOut"];
        App.CurrentUser.LoggedIn = false;
        Console.WriteLine("You are now logged out.");
        return Task.CompletedTask;
    }
    
}