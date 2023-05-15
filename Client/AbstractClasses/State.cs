using Client.Helpers;
using Client.Interfaces;
using Client.Models;

namespace Client.AbstractClasses;

public abstract class State
{
    protected IApp App { get; }
    protected Helper Helper { get; }

    protected State(IApp app)
    {
        App = app;
        Helper = App.Helper;
    }
}