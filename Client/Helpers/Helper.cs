namespace Client.Helpers;

public class Helper
{
    public (string, string) PromptForCredentials()
    {
        string? password = "";
        string? username = "";
        while (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("Enter Username.");
            username = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("You must write a username.");
            }
            Console.WriteLine("Enter Password.");
            password = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(username))
            {
                Console.WriteLine("You must write a password.");
            }
        }
        return (username, password);
    }
}