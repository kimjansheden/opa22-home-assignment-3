using Client.Interfaces;

namespace Client.AbstractClasses;

public abstract class LoginState : State
{
    protected internal abstract void LoginLogoutHandle(ICommand currentCommand);

    protected LoginState(IApp app) : base(app)
    {
    }
}