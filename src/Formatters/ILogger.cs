namespace Mover.Formatters;

internal interface ILogger
{
    void Verbose(string message);

    void Info(string message);

    void Warning(string message);

    void Error(string message);

    void Error(Exception ex);
}