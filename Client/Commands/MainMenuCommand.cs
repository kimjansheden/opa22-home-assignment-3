using Client.Interfaces;

namespace Client.Commands;

public class MainMenuCommand : ICommand
{
    private IApp _app;

    public MainMenuCommand(IApp app)
    {
        _app = app;
    }

    public Task Execute()
    {
        Console.WriteLine(_app.MainMenu.Refresh());
        return Task.CompletedTask;
    }

    public ICommand Initialize(IApp app)
    {
        return this;
    }
}