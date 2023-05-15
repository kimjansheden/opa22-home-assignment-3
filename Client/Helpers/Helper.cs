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

    public Task<(string? description, string? title, string category, bool? buyAd, int price, int length)>
        PromptForAdData()
    {
        string? category = "";
        string? title = "";
        string? description = "";
        bool? buyAd = null;
        int price = -1;
        int length = -1;

        // Prompt for the correct value for Buy or Sell.
        while (buyAd == null)
        {
            Console.WriteLine($"Do you want to buy or sell?{Environment.NewLine}1. Buy{Environment.NewLine}2. Sell");
            var buyOrSellPrompt = int.TryParse(Console.ReadLine(), out int buyOrSell);
            if (!buyOrSellPrompt || buyOrSell != 1 && buyOrSell != 2)
            {
                Console.WriteLine("You need to write either 1 or 2.");
                continue;
            }

            buyAd = buyOrSell == 1;
        }

        // Prompt for the correct value for Category.
        while (string.IsNullOrWhiteSpace(category))
        {
            Console.WriteLine(
                $"Choose a category.{Environment.NewLine}1. Motor Boat{Environment.NewLine}2. Sail Boat{Environment.NewLine}3. Other");
            var categoryPrompt = int.TryParse(Console.ReadLine(), out int categoryInt);
            if (!categoryPrompt || categoryInt != 1 && categoryInt != 2 && categoryInt != 3)
            {
                Console.WriteLine("You must write either 1, 2 or 3.");
                continue;
            }

            switch (categoryInt)
            {
                case 1:
                    category = "motorboat";
                    break;
                case 2:
                    category = "sailboat";
                    break;
                case 3:
                    category = "other";
                    break;
            }
        }

        // Prompt for the correct value for Title.
        while (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Enter title.");
            title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("You must write a title.");
                continue;
            }
        }

        // Prompt for the correct value for Description.
        while (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Enter description.");
            description = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(description))
            {
                Console.WriteLine("You must write a description.");
                continue;
            }
        }

        // Prompt for a Price.
        Console.WriteLine("If you want to set a price, write the price in SEK. Otherwise just press Enter.");
        var priceSuccess = int.TryParse(Console.ReadLine(), out price);
        if (!priceSuccess)
        {
            price = -1;
        }
            
        // Prompt for a Length.
        Console.WriteLine("If you want to set a length, write the length in cm. Otherwise just press Enter.");
        var lengthSuccess = int.TryParse(Console.ReadLine(), out length);
        if (!lengthSuccess)
        {
            length = -1;
        }
        return Task.FromResult((description, title, category, buyAd, price, length));
    }
}