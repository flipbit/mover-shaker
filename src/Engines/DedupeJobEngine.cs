using Mover.Core;
using Mover.Jobs;

namespace Mover.Engines;

public class DedupeJobEngine
{
    private readonly IFileSystem fileSystem;
    private readonly FileFinder fileFinder;

    private class DuplicateSet
    {
        public string FilePath { get; }

        public ISet<string> DuplicateFilePaths { get; set; }

        public DuplicateSet(string filePath)
        {
            FilePath = filePath;
            DuplicateFilePaths = new HashSet<string>();
        }
    }

    public DedupeJobEngine(FileFinder fileFinder, IFileSystem fileSystem)
    {
        this.fileFinder = fileFinder;
        this.fileSystem = fileSystem;
    }

    public Task Run(DedupeJob job)
    {
        var files = fileFinder.FindFiles(job);

        var duplicates = new List<DuplicateSet>();

        foreach (var file in SortByPreference(files, job))
        {
            if (duplicates.Any(x => x.DuplicateFilePaths.Contains(file))) continue;

            Dedupe(file, files, duplicates);
        }

        foreach (var duplicate in duplicates)
        {
            Log.Info($"File: {duplicate.FilePath} - {duplicate.DuplicateFilePaths.Count} duplicate(s):");

            foreach (var filePath in duplicate.DuplicateFilePaths)
            {
                Log.Info($"  - {filePath}");
            }

            Log.Info(string.Empty);

            foreach (var filePath in duplicate.DuplicateFilePaths)
            {
                Log.Info($"DELETING: {filePath}");
                fileSystem.Delete(filePath);

            }
        }

        return Task.CompletedTask;
    }

    private List<string> SortByPreference(IEnumerable<string> filePaths, DedupeJob job)
    {
        if (job.PreferShortest)
        {
            return filePaths
                .OrderBy(x => Path.GetFileName(x).Length)
                .ToList();
        }

        if (job.PreferNewest)
        {
            return filePaths
                .OrderByDescending(x => fileSystem.GetLastWriteTime(x))
                .ToList();
        }

        if (job.PreferOldest)
        {
            return filePaths
                .OrderBy(x => fileSystem.GetLastWriteTime(x))
                .ToList();
        }

        throw new ArgumentOutOfRangeException($"Unknown dedupe preference for {job.Name}");
    }

    private void Dedupe(string originalFilePath, List<string> filePaths, List<DuplicateSet> duplicates)
    {
        foreach (var filePath in filePaths)
        {
            // Ignore comparision to the same file
            if (filePath == originalFilePath) continue;

            if (IsDuplicate(originalFilePath, filePath))
            {
                var duplicate = duplicates.FirstOrDefault(x => x.FilePath == originalFilePath);

                if (duplicate is not null)
                {
                    duplicate.DuplicateFilePaths.Add(filePath);
                }
                else
                {
                    duplicate = new DuplicateSet(originalFilePath);
                    duplicate.DuplicateFilePaths.Add(filePath);

                    duplicates.Add(duplicate);
                }
            }
        }
    }

    private bool IsDuplicate(string first, string second)
    {
        if (!File.Exists(first)) return false;
        if (!File.Exists(second)) return false;

        var firstInfo = new FileInfo(first);
        var secondInfo = new FileInfo(second);

        if (firstInfo.Length != secondInfo.Length) return false;

        var firstHash = fileSystem.CalculateHash(first);
        var secondHash = fileSystem.CalculateHash(second);

        return firstHash == secondHash;
    }
}