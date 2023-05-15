namespace Client.Interfaces;

public interface IMenu
{
    ISpecificMenu GetMenu(Enum menu);
}