using System.Security.Cryptography;

namespace Mover.Core;

public interface IFileSystem
{
    string CalculateHash(string path);

    DateTime GetLastWriteTime(string path);

    void Delete(string path);

    bool DirectoryIsEmpty(string path);

    void DeleteDirectory(string path);

    bool FileExists(string path);

    void MoveFile(string sourcePath, string destinationPath);
}

public class FileSystem : IFileSystem
{
    public string CalculateHash(string path)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path);

        var hash = md5.ComputeHash(stream);

        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    public DateTime GetLastWriteTime(string path)
    {
        return File.GetLastWriteTime(path);
    }

    public void Delete(string path)
    {
        File.Delete(path);
    }

    public bool DirectoryIsEmpty(string path)
    {
        return !Directory.EnumerateFileSystemEntries(path).Any();
    }

    public void DeleteDirectory(string path)
    {
        Directory.Delete(path);
    }

    public bool FileExists(string path)
    {
        return File.Exists(path);
    }

    public void MoveFile(string sourcePath, string destinationPath)
    {
        File.Move(sourcePath, destinationPath);
    }
}