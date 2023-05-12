using Client.AbstractClasses;
using Client.Commands;
using Client.Exceptions;
using Client.Interfaces;

namespace Client.States.LoginStates;

public class LoggedOutState : LoginState
{
    public LoggedOutState(App app) : base(app)
    {
        
    }

    protected internal override void LoginLogoutHandle(ICommand currentCommand)
    {
        if (currentCommand is not LoginLogoutCommand)
        {
            MustBeLoggedIn();
        }
    }

    private void MustBeLoggedIn()
    {
        throw new NotLoggedInException("You need to be logged in to do this.");
    }
}