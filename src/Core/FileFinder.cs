using System.Text.RegularExpressions;
using Mover.Jobs;

namespace Mover.Core;

public class FileFinder
{
    public List<string> FindFiles(Job job)
    {
        var files = new List<string>();

        // File files in all source paths
        foreach (var sourcePath in job.SourcePaths)
        {
            var searchOption = job.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            var sourceFiles = Directory.GetFiles(sourcePath, "*", searchOption);

            foreach (var sourceFile in sourceFiles)
            {
                var sourceFileName = Path.GetFileName(sourceFile).ToLowerInvariant();

                if (job.CaseInsensitive) sourceFileName = sourceFileName.ToLowerInvariant();

                foreach (var filter in job.FileNameFilter)
                {
                    var expression = WildCardToRegularExpression(filter, job.CaseInsensitive);

                    var matched = Regex.IsMatch(sourceFileName, expression);

                    if (matched && !files.Contains(sourceFile))
                    {
                        files.Add(sourceFile);

                        Log.Verbose($"Found: {sourceFile}");

                        break;
                    }
                }
            }
        }

        // Remove files that are too old
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];

            var when = job.UseCreatedTime ? File.GetCreationTime(file) : File.GetLastWriteTime(file);

            if (IsTimeToProcessFile(when, job)) continue;

            Log.Verbose($"Waiting to process file: {file}");

            files.RemoveAt(i);
        }

        // Remove files that are too small
        for (var i = files.Count - 1; i >= 0; i--)
        {
            var file = files[i];

            if (job.FileSizeMb <= 0) continue;

            var info = new FileInfo(file);
            var sizeMb = info.Length / 1024 / 1024;

            if (sizeMb >= job.FileSizeMb) continue;

            Log.Verbose($"Skipping file: {file} (too small)");

            files.RemoveAt(i);
        }

        return files;
    }


    public List<string> FindDirectories(Job job)
    {
        var directories = new List<string>();

        if (!job.Recursive) return directories;

        foreach (var sourcePath in job.SourcePaths)
        {
            var searchStack = new Stack<string>();
            searchStack.Push(sourcePath);

            while (searchStack.Count > 0)
            {
                var path = searchStack.Pop();

                foreach (var directory in Directory.GetDirectories(path))
                {
                    directories.Insert(0, directory);
                    searchStack.Push(directory);
                }
            }
        }

        return directories;

    }

    private static string WildCardToRegularExpression(string value, bool lowerCase) 
    {
      var expression = "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";

      if (lowerCase) expression = expression.ToLowerInvariant();

      return expression;
    }

    private static bool IsTimeToProcessFile(DateTime writeTime, Job job)
    {
        var delay = TimeSpan.Zero;

        delay = delay.Add(TimeSpan.FromSeconds(job.WaitSeconds));
        delay = delay.Add(TimeSpan.FromDays(job.WaitDays));

        var processAfter = writeTime.Add(delay);

        return processAfter < DateTime.Now;
    }
}