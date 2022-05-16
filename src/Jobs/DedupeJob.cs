namespace Mover.Jobs;

public class DedupeJob : Job
{
    public bool PreferNewest { get; set; }

    public bool PreferOldest { get; set; }

    public bool PreferShortest { get; set; }

    public DedupeJob()
    {
        PreferOldest = true;
    }
}