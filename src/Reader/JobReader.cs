using Mover.Core;
using Mover.Jobs;
using SharpConfig;

namespace Mover.Reader;

internal interface IJobReader
{
    /// <summary>
    /// Reads the jobs in the given <see cref="fileName"/>.
    /// </summary>
    Task<List<Job>> GetJobsAsync(string fileName);
}

internal class JobReader : IJobReader
{
    private readonly IFileSystem fileSystem;

    public JobReader(IFileSystem fileSystem)
    {
        this.fileSystem = fileSystem;
    }

    public async Task<List<Job>> GetJobsAsync(string fileName)
    {
        if (!fileSystem.FileExists(fileName))
        {
            throw new FileNotFoundException($"Path not found: {fileName}");
        }

        var contents = await File.ReadAllTextAsync(fileName);

        // Uncomment to allow '#' characters in config values
        // SharpConfig.Configuration.IgnoreInlineComments = true;

        var config = SharpConfig.Configuration.LoadFromString(contents);

        var jobs = new List<Job>();

        foreach (var section in config)
        {
            var job = CreateJob(section);

            if (job is null) continue;

            job.Name = section.Name;

            foreach (var setting in section)
            {
                switch (setting.Name.ToLowerInvariant())
                {
                    case "sourcepath":
                        job.SourcePaths.Add(setting.StringValue);
                        break;

                    case "filenamefilter":
                        job.FileNameFilter.Add(setting.StringValue);
                        break;

                    case "caseinsensitive":
                        job.CaseInsensitive = setting.BoolValue;
                        break;

                    case "waitdays":
                        job.WaitDays = setting.IntValue;
                        break;

                    case "waitseconds":
                        job.WaitSeconds = setting.IntValue;
                        break;

                    case "filesizemb":
                        job.FileSizeMb = setting.IntValue;
                        break;

                    case "recursive":
                        job.Recursive = setting.BoolValue;
                        break;

                    case "usecreatedtime":
                        job.UseCreatedTime = true;
                        job.UseModifiedTime = false;
                        break;
                        
                    case "usemodifiedtime":
                        job.UseCreatedTime = false;
                        job.UseModifiedTime = true;
                        break;
                }

                if (job is MoveJob moveJob)
                {
                    switch (setting.Name.ToLowerInvariant())
                    {
                        case "destinationpath":
                            moveJob.DestinationPath = setting.StringValue;
                            break;

                        case "destinationfilename":
                            moveJob.DestinationFileName = setting.RawValue;
                            break;
                    }
                }

                if (job is DedupeJob dedupeJob)
                {
                    switch (setting.Name.ToLowerInvariant())
                    {
                        case "prefernewest":
                            dedupeJob.PreferNewest = setting.BoolValue;
                            dedupeJob.PreferOldest = false;
                            dedupeJob.PreferShortest = false;
                            break;

                        case "preferoldest":
                            dedupeJob.PreferOldest = setting.BoolValue;
                            dedupeJob.PreferNewest = false;
                            dedupeJob.PreferShortest = false;
                            break;

                        case "prefershortest":
                            dedupeJob.PreferShortest = setting.BoolValue;
                            dedupeJob.PreferNewest = false;
                            dedupeJob.PreferOldest = false;
                            break;
                    }
                }

                if (job is DeleteJob deleteJob)
                {
                    
                    switch (setting.Name.ToLowerInvariant())
                    {
                        case "removeemptydirectories":
                            deleteJob.RemoveEmptyDirectories = setting.BoolValue;
                            break;
                    }
                }
            }

            if (job.SourcePaths.Any())
            {
                jobs.Add(job);
            }
        }

        return jobs;
    }

    private Job? CreateJob(Section section)
    {
        if (section.Name == "$SharpConfigDefaultSection") return null;

        foreach (var setting in section)
        {
            var name = setting.Name.ToLowerInvariant();

            if (name != "type") continue;

            var value = setting.RawValue?.ToLowerInvariant();

            switch (value)
            {
                case "move":
                    return new MoveJob();
                case "dedupe":
                    return new DedupeJob();
                case "delete":
                    return new DeleteJob();
                default:
                    throw new ArgumentOutOfRangeException($"Unable to create job of type: {value}");
            }
        }

        throw new Exception($"Section [{section.Name}] is missing a job type.");
    }

    private string? GetConfigurationFileName()
    {
        return SettingFileNames.FirstOrDefault(File.Exists);
    }

    private bool AnyConfigurationPresent()
    {
        foreach (var fileName in SettingFileNames)
        {
            if (File.Exists(fileName)) return true;
        }

        return false;
    }

    private async Task WriteDefaultConfiguration()
    {
        var assembly = typeof(JobReader).Assembly;

        await using var stream = assembly.GetManifestResourceStream("Mover.Reader.Defaults.txt");

        if (stream is null) return;   

        using var reader = new StreamReader(stream);
        var defaultConfiguration = await reader.ReadToEndAsync();

        var defaultPath = SettingFileNames.Last();

        await File.WriteAllTextAsync(defaultPath, defaultConfiguration);
    }

    private IEnumerable<string> SettingFileNames
    {
        get
        {
            var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            yield return Path.Join(homeDirectory, ".config", "mover-shaker", "mover-shaker.config");

            yield return Path.Join(homeDirectory, ".mover-shaker");
        }
    }
}