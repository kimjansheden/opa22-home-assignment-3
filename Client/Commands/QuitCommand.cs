using Client.Interfaces;

namespace Client.Commands;

public class QuitCommand : ICommand
{
    public QuitCommand()
    {
        
    }
    
    public ICommand Initialize(IApp app)
    {
        return this;
    }
    public async Task Execute()
    {
        
    }
}