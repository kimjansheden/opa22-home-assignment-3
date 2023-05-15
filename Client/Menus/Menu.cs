using Client.Interfaces;

namespace Client.Menus;

public class Menu : IMenu
{
    private ISpecificMenu _mainMenu;
    private readonly IReadOnlyDictionary<Enum, Func<ISpecificMenu>> _menuDelegates;
    private IApp _app;

    public Menu(IApp app)
    {
        _mainMenu = new MainMenu(app);
        _menuDelegates = new Dictionary<Enum, Func<ISpecificMenu>>
        {
            { Enums.Menus.Main, GetMainMenu }
        };
    }

    private ISpecificMenu GetMainMenu()
    {
        return _mainMenu.Refresh();
    }

    public ISpecificMenu GetMenu(Enum menu)
    {
        return _menuDelegates[menu].Invoke();
    }
}