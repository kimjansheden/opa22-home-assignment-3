namespace Client.Interfaces;

public interface ICommand
{
    Task Execute();
    ICommand Initialize(IApp app);
}