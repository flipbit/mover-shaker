using Mover.Core;
using Mover.Jobs;

namespace Mover.Engines;

public class DeleteJobEngine
{
    private readonly FileFinder fileFinder;
    private readonly IFileSystem fileSystem;

    public DeleteJobEngine(FileFinder fileFinder, IFileSystem fileSystem)
    {
        this.fileFinder = fileFinder;
        this.fileSystem = fileSystem;
    }

    public Task Run(DeleteJob job)
    {
        var files = fileFinder.FindFiles(job);

        foreach (var file in files)
        {
            Log.Info($"DELETE: {file}");

            fileSystem.Delete(file);
        }


        if (job.RemoveEmptyDirectories)
        {
            var directories = fileFinder.FindDirectories(job);

            foreach (var directory in directories)
            {
                if (fileSystem.DirectoryIsEmpty(directory))
                {
                    fileSystem.DeleteDirectory(directory);
                }
            }
        }

        return Task.CompletedTask;
    }
}