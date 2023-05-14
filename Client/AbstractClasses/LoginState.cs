using Client.Interfaces;

namespace Client.AbstractClasses;

public abstract class LoginState : State
{
    protected internal abstract void LoginLogoutHandle();
    protected internal abstract void RequestHandle(ICommand currentCommand);
    protected internal abstract LoginState Initialize(IApp app);

    protected LoginState(IApp app) : base(app)
    {
    }
}