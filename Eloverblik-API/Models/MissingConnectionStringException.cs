namespace Eloverblik_API.Models;

public class MissingConnectionStringException : Exception
{
    public MissingConnectionStringException(string message)
        : base(message)
    {
    }
}
