using Mover.Formatters;

namespace Mover;

internal static class Log
{
    private static readonly List<ILogger> Loggers;

    private static bool _verboseEnabled;

    static Log()
    {
        Loggers = new List<ILogger>
        {
            new ConsoleLogger(),
            new FileLogger()
        };
    }

    public static void EnableVerboseLogging(bool enabled)
    {
        _verboseEnabled = enabled;
    }

    public static void Verbose(string message)
    {
        if (!_verboseEnabled) return;

        foreach (var logger in Loggers)
        {
            logger.Verbose(message);
        }
    }

    public static void Info(string message)
    {
        foreach (var logger in Loggers)
        {
            logger.Info(message);
        }
    }

    public static void Warning(string message)
    {
        foreach (var logger in Loggers)
        {
            logger.Warning(message);
        }
    }

    public static void Error(string message)
    {
        foreach (var logger in Loggers)
        {
            logger.Error(message);
        }
    }

    public static void Error(Exception ex)
    {
        foreach (var logger in Loggers)
        {
            logger.Error(ex);
        }
    }
}