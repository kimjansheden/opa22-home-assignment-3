using System.Text;
using Client.Interfaces;
using Client.States.LoginStates;

namespace Client.Menus;

public class MainMenu : ISpecificMenu
{
    private string _menuString;
    private IApp _app;
    
    protected virtual string LoginStateString { get; private set; }

    public MainMenu(IApp app)
    {
        _app = app;
        _menuString = BuildMainMenu();
    }

    public ISpecificMenu Refresh()
    {
        _menuString = BuildMainMenu();
        return this;
    }
    private string BuildMainMenu()
    {
        SetLoginState();
        var menuCounter = 0;
        var sb = new StringBuilder();
        sb.Append("What do you want to do?").AppendLine()
            .Append("Choose between 1–").Append(_app.AmountOfOptions).AppendLine()
            .Append("Press \"q\" to quit.").AppendLine()
            .Append("Press \"m\" for Main Menu.").AppendLine()
            .Append(menuCounter += 1).Append(": Registrera användare").AppendLine() // POST
            .Append(menuCounter += 1).Append(": ").Append(LoginStateString).AppendLine()
            .Append(menuCounter += 1).Append(": Se dina annonser").AppendLine() // GET
            .Append(menuCounter += 1).Append(": Redigera annons").AppendLine() // PUT
            .Append(menuCounter += 1).Append(": Radera annons").AppendLine() // DELETE
            .Append(menuCounter += 1).Append(": Lägg upp ny annons").AppendLine(); // POST
        return sb.ToString();
    }
    public override string ToString()
    {
        return _menuString;
    }

    private void SetLoginState()
    {
        LoginStateString = _app.LoginState is LoggedInState ? "Logga ut" : "Logga in";
    }
}