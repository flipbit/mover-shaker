namespace Mover.Core;

public interface IFileNamer
{
    string GetFileName(string path);
}

public class FileNamer : IFileNamer
{
    private readonly IFileSystem fileSystem;

    public FileNamer(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public string GetFileName(string path)
    {
        var result = path;

        var counter = 2;

        while (fileSystem.FileExists(result))
        {
            var filePath = Path.GetDirectoryName(path);
            var fileName = Path.GetFileNameWithoutExtension(path);
            var fileExt = Path.GetExtension(path);

            var candidateFileName = $"{fileName}-{counter:000}";

            if (!string.IsNullOrWhiteSpace(fileExt))
            {
                candidateFileName = $"{candidateFileName}{fileExt}";
            }

            result = Path.Join(filePath, candidateFileName);

            counter++;
        }

        return result;
    }
}