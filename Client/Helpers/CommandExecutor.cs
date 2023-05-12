using Client.Interfaces;

namespace Client.Helpers;

public class CommandExecutor : ICommandExecutor
{
    private IApp _app;

    public CommandExecutor(IApp app)
    {
        _app = app;
    }

    public ICommandExecutor Initialize(IApp app)
    {
        _app = app;
        return this;
    }

    public async Task ExecuteCommandIfExists(string input)
    {
        if (_app.Commands.ContainsKey(input))
        {
            await _app.Commands[input].Execute();
            _app.CurrentCommand = _app.Commands[input];
        }
        else
        {
            Console.WriteLine("Invalid option.");
        }
    }
}