namespace Mover.Jobs;

public class MoveJob : Job
{
    public string DestinationPath { get; set; }

    public string DestinationFileName { get; set; }

    public MoveJob()
    {
        Name = "New Move Job";
        DestinationPath = string.Empty;
        DestinationFileName = string.Empty;
    }
}