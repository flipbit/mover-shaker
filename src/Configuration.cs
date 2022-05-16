using Microsoft.Extensions.DependencyInjection;
using Mover.Commands;
using Mover.Core;
using Mover.Engines;
using Mover.Reader;

namespace Mover
{
    internal static class Configuration
    {
        public static IServiceCollection AddMoverShaker(this IServiceCollection services)
        {
            // Commands
            services.AddTransient<RunCommand>();

            // Services
            services.AddTransient<IJobReader, JobReader>();

            // Job Engines
            services.AddTransient<IJobEngine, JobEngine>();
            services.AddTransient<MoveJobEngine>();
            services.AddTransient<DedupeJobEngine>();
            services.AddTransient<DeleteJobEngine>();

            // File System
            services.AddTransient<IFileSystem, FileSystem>();
            services.AddTransient<IFileNamer, FileNamer>();
            services.AddTransient<FileFinder>();

            return services;
        }
    }
}
