using Mover.Core;
using Mover.Jobs;

namespace Mover.Engines;

public class MoveJobEngine
{
    private readonly FileFinder fileFinder;
    private readonly IFileNamer fileNamer;
    private readonly IFileSystem fileSystem;

    public MoveJobEngine(
        FileFinder fileFinder, 
        IFileNamer fileNamer, 
        IFileSystem fileSystem)
    {
        this.fileFinder = fileFinder;
        this.fileNamer = fileNamer;
        this.fileSystem = fileSystem;
    }

    public virtual Task Run(MoveJob job)
    {
        var files = fileFinder.FindFiles(job);

        foreach (var file in files)
        {
            var when = File.GetLastWriteTime(file);

            var destinationPath = job
                .DestinationPath
                .Replace("{year}", when.Year.ToString())
                .Replace("{month}", when.Month.ToString("00"));

            if (!Directory.Exists(destinationPath))
            {
                Directory.CreateDirectory(destinationPath);
            }

            var fileName = Path.GetFileName(file);

            var destinationFileName = GetDestinationFileName(fileName, when, job.DestinationFileName);

            var destinationFilePath = Path.Combine(destinationPath, destinationFileName);

            destinationFilePath = fileNamer.GetFileName(destinationFilePath);


            try
            {
                fileSystem.MoveFile(file, destinationFilePath);

                Log.Info($"Moved: {destinationFilePath}");
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        return Task.CompletedTask;
    }

    private static string GetDestinationFileName(string fileName, DateTime when, string jobDestinationFileName)
    {
        if (string.IsNullOrWhiteSpace(jobDestinationFileName)) return fileName;

        return jobDestinationFileName
            .Replace("{year}", when.Year.ToString())
            .Replace("{month}", when.Month.ToString("00"))
            .Replace("{day}", when.Day.ToString("00"))
            .Replace("{0}", fileName);
    }

}