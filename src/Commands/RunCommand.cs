using CommandLine;
using Mover.Engines;
using Mover.Reader;

namespace Mover.Commands;

internal class RunCommand
{
    [Verb("run", HelpText = "Run Jobs")]
    public class Options
    {
        [Value(0, HelpText = "The path to the file containing jobs", Required = true)]
        public string FileName { get; set; } = string.Empty;

        [Option('v', "verbose", HelpText = "Enable verbose logging.", Default = false)]
        public bool Verbose { get; set; }
    }

    private readonly IJobEngine jobEngine;
    private readonly IJobReader jobReader;

    public RunCommand(IJobEngine jobEngine, IJobReader jobReader)
    {
        this.jobEngine = jobEngine;
        this.jobReader = jobReader;
    }

    public async Task<int> Execute(Options options)
    {
        Log.EnableVerboseLogging(options.Verbose);

        var jobs = await jobReader.GetJobsAsync(options.FileName);

        foreach (var job1 in jobs)
        {
            await jobEngine.Run(job1);
        }

        return 0;
    }
}
