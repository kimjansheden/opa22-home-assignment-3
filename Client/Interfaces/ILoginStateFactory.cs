using Client.AbstractClasses;

namespace Client.Interfaces;

public interface ILoginStateFactory
{
    LoginState CreateState(App app);
}