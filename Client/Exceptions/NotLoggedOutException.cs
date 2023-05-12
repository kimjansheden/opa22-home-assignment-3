namespace Client.Exceptions;

public class NotLoggedOutException : Exception
{
    public NotLoggedOutException(string message)
        : base(message)
    {
    }
}