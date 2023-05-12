using Client.Interfaces;
using Client.Models;

namespace Client.AbstractClasses;

public abstract class State
{
    protected IApp App { get; }

    protected virtual int AmountOfOptions
    {
        get => App.AmountOfOptions;
        set => App.AmountOfOptions = value;
    }
    protected internal virtual List<string> Options
    { get; set; }

    protected User CurrentUser
    {
        get => App.CurrentUser;
        set => App.CurrentUser = value;
    }

    protected virtual State CurrentState
    {
        get => App.CurrentState;
        set => App.CurrentState = value;
    }

    protected virtual LoginState LoginState
    {
        get => App.LoginState;
        set => App.LoginState = value;
    }

    protected State(IApp app)
    {
        App = app;
    }
}