using Mover.Jobs;

namespace Mover.Engines;

public interface IJobEngine
{
    Task Run(Job job);
}

public class JobEngine : IJobEngine
{
    private readonly MoveJobEngine moveJobEngine;
    private readonly DedupeJobEngine dedupeJobEngine;
    private readonly DeleteJobEngine deleteJobEngine;

    public JobEngine(
        MoveJobEngine moveJobEngine,
        DedupeJobEngine dedupeJobEngine,
        DeleteJobEngine deleteJobEngine)
    {
        this.moveJobEngine = moveJobEngine;
        this.dedupeJobEngine = dedupeJobEngine;
        this.deleteJobEngine = deleteJobEngine;
    }

    public Task Run(Job job)
    {
        switch (job)
        {
            case null:
                throw new ArgumentNullException(nameof(job));

            case MoveJob moveJob:
                return moveJobEngine.Run(moveJob);

            case DedupeJob dedupeJob:
                return dedupeJobEngine.Run(dedupeJob);

            case DeleteJob deleteJob:
                return deleteJobEngine.Run(deleteJob);

            default:
                throw new ArgumentOutOfRangeException($"Unknown Job type: {job.GetType().Name}");
        }
    }
}