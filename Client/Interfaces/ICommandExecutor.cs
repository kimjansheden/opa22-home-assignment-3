namespace Client.Interfaces;

public interface ICommandExecutor
{
    Task ExecuteCommandIfExists(string? input);
    ICommandExecutor Initialize(IApp app);
}