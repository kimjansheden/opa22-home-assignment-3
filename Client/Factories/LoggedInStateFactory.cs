using Client.AbstractClasses;
using Client.Interfaces;
using Client.States.LoginStates;

namespace Client.Factories;

public class LoggedInStateFactory : ILoginStateFactory
{
    public LoginState CreateState(App app)
    {
        var state = new LoggedInState(app);
        return state;
    }
}