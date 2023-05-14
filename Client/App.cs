using Client.AbstractClasses;
using Client.Commands;
using Client.Factories;
using Client.Helpers;
using Client.Interfaces;
using Client.Menus;
using Client.Models;

namespace Client;

public class App : IApp
{
    private IMenu Menu { get; set; }
    public ISpecificMenu MainMenu { get; set; }
    public string DefaultApiUri { get; set; }
    private Uri ServerUri { get; set; }
    private Dictionary<string, ILoginStateFactory> _loginStateFactories;
    public string GetUserByUsernameUri { get; set; }
    public string GetAdByIdUri { get; set; }
    public LoginState LoginState { get; set; }
    public Dictionary<string, LoginState> LoginStates { get; set; }
    public int AmountOfOptions
    { get; set; }

    public User CurrentUser { get; set; }
    public State CurrentState { get; set; }
    public HttpClient Client { get; set; }
    public ICommandExecutor CommandExecutor { get; set; }
    public Uri CurrentUri { get; set; }
    public ICommand CurrentCommand { get; set; }
    public Dictionary<string, ICommand> Commands { get; set; }

    public App()
    {
        var menuCounter = 0;
        // The reason why we don't send other variables directly to the constructors of the commands is that we don't have those variables ready at this stage. That's why we do Initialize later, as we get those variables needed.
        Commands = new Dictionary<string, ICommand>
        {
            {$"{menuCounter += 1}", new AddUserCommand(this)},
            {$"{menuCounter += 1}", new LoginLogoutCommand(this)},
            {$"{menuCounter += 1}", new GetYourAdsCommand(this)},
            {$"{menuCounter += 1}", new GetAdsCommand(this)},
            {$"{menuCounter += 1}", new EditAdCommand(this)},
            {$"{menuCounter += 1}", new DeleteAdCommand(this)},
            {$"{menuCounter += 1}", new AddAdCommand(this)},
            {"m", new MainMenuCommand(this)},
            {"q", new QuitCommand()}
        };

        AmountOfOptions = Commands.Count - 2;
        CreateDefaultStates();
        Menu = new Menu(this);
        MainMenu = Menu.GetMenu(Enums.Menus.Main);

        Client = new HttpClient();

        CommandExecutor = new CommandExecutor(this);

        DefaultApiUri = "https://localhost:7167/api/";
        GetUserByUsernameUri = DefaultApiUri + "user/get";
        GetAdByIdUri = DefaultApiUri + "ad/byID?Id=";

        CurrentUri = new Uri(DefaultApiUri);
        ServerUri = CurrentUri;
    }

    public async Task Run()
    {
        using (Client)
        {
            ServerUri = GetUriFromUser();
            
            while (CurrentCommand is not QuitCommand)
            {
                Console.WriteLine(MainMenu.Refresh());
                var userInput = Console.ReadLine();
                CurrentUri = ServerUri;
                await CommandExecutor.ExecuteCommandIfExists(userInput);
            }
        }
    }

    private Uri GetUriFromUser()
    {
        Console.WriteLine("Write the URL to the api. Just press Enter for Default.");
        var uriString = Console.ReadLine();
        return string.IsNullOrEmpty(uriString) ? CurrentUri : new Uri(uriString);
    }
    
    private void CreateDefaultStates()
    {
        _loginStateFactories = new Dictionary<string, ILoginStateFactory>();
        _loginStateFactories.Add("LoggedIn", new LoggedInStateFactory());
        _loginStateFactories.Add("LoggedOut", new LoggedOutStateFactory());
        
        AddLoginStates();
        
        LoginState = LoginStates["LoggedOut"];
    }

    private void AddLoginStates()
    {
        LoginStates = new Dictionary<string, LoginState>();
        foreach (var factory in _loginStateFactories)
        {
            LoginStates.Add(factory.Key, factory.Value.CreateState(this));
        }
    }
}