using Client.Interfaces;

namespace Client;

public class Program
{
    public static async Task Main(string[] args)
    {
        IApp app = new App();
        await app.Run();
    }
}