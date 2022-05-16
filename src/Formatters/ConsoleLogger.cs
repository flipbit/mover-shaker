namespace Mover.Formatters;

public class ConsoleLogger : ILogger
{
    public void Verbose(string message)
    {
        Console.WriteLine(message);
    }

    public void Info(string message)
    {
        Console.WriteLine(message);
    }

    public void Warning(string message)
    {
        Console.WriteLine($"Warning: {message}");
    }

    public void Error(string message)
    {
        Console.Error.WriteLine(message);
    }

    public void Error(Exception ex)
    {
        Console.Error.WriteLine(ex.Message);
        Console.Error.WriteLine(ex.StackTrace);
    }

}