namespace Mover.Jobs;

public abstract class Job
{
    public string Name { get; set; }

    public List<string> SourcePaths { get; }

    public List<string> FileNameFilter { get; }

    public bool CaseInsensitive { get; set; }

    public bool Recursive { get; set; }

    public bool UseCreatedTime { get; set; }

    public bool UseModifiedTime { get; set; }

    public int WaitSeconds { get; set; }

    public int WaitDays { get; set; }

    public int FileSizeMb { get; set; }

    protected Job()
    {
        Name = "New Job";
        SourcePaths = new List<string>();
        FileNameFilter = new List<string>();
        CaseInsensitive = true;
        Recursive = false;
        UseCreatedTime = false;
        UseModifiedTime = true;
        WaitSeconds = 60;
        WaitDays = 0;
        FileSizeMb = 0;
    }
}