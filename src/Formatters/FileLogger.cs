namespace Mover.Formatters;

public class FileLogger : ILogger
{
    public void Verbose(string message)
    {
        AppendLine(message);
    }

    public void Info(string message)
    {
        AppendLine(message);
    }

    public void Warning(string message)
    {
        AppendLine(message);
    }

    public void Error(string message)
    {
        AppendLine(message);
    }

    public void Error(Exception ex)
    {
        AppendLine(ex.Message);
        AppendLine(ex.StackTrace);
    }

    private void AppendLine(string? contents)
    {
        if (string.IsNullOrWhiteSpace(contents)) return;

        try
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var path = Path.Combine(homeDirectory, "mover-shaker", "logs");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var logFile = Path.Combine(path, $"{DateTime.Now:yyyy-MM-dd}-mover-shaker.log");

            File.AppendAllText(logFile, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} : {contents}{Environment.NewLine}");
        }
        catch
        {
            // Nothing
        }
    }

}