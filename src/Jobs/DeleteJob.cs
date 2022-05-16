namespace Mover.Jobs;

public class DeleteJob : Job
{
    public bool RemoveEmptyDirectories { get; set; }

    public DeleteJob()
    {
        RemoveEmptyDirectories = true;
    }
}