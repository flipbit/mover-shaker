using System.Diagnostics;
using System.Reflection;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mover;
using Mover.Commands;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.AddMoverShaker();
    })
    .Build();

if (Debugger.IsAttached)
{
    args = new[] { "run" };
}

await Parser
    .Default
    .ParseArguments(args, LoadVerbs())
    .WithParsedAsync(x => Run(host, x));

Type[] LoadVerbs()
{
    return Assembly
        .GetExecutingAssembly()
        .GetTypes()
        .Where(t => t.GetCustomAttribute<VerbAttribute>() is not null)
        .ToArray();
}

static async Task Run(IHost host, object args)
{
    var exitCode = 1;

    try
    {
        switch (args)
        {
            case RunCommand.Options ackOptions:
                var ack = host.Services.GetService<RunCommand>();
                exitCode = await ack.Execute(ackOptions);
                break;
        }
    }
    catch (Exception e)
    {
        Log.Error(e);

        exitCode = 1;
    }

    Environment.Exit(exitCode);
}
