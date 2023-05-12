using Client.AbstractClasses;
using Client.Models;

namespace Client.Interfaces;

public interface IApp
{
    Dictionary<string, ICommand> Commands { get; set; }
    ICommand CurrentCommand { get; set; }
    HttpClient Client { get; set; }
    Uri CurrentUri { get; set; }
    ICommandExecutor CommandExecutor { get; set; }
    string GetUserByUsernameUri { get; set; }
    string GetAdByIdUri { get; set; }
    LoginState LoginState { get; set; }
    Dictionary<string, LoginState> LoginStates { get; set; }
    int AmountOfOptions { get; set; }
    User CurrentUser { get; internal set; }
    State CurrentState { get; set; }
    ISpecificMenu MainMenu { get; set; }
    Task Run();
}