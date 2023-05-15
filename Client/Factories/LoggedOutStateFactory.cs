using Client.AbstractClasses;
using Client.Interfaces;
using Client.States.LoginStates;

namespace Client.Factories;

public class LoggedOutStateFactory : ILoginStateFactory
{
    public LoginState CreateState(App app)
    {
        var state = new LoggedOutState(app);
        return state;
    }
}