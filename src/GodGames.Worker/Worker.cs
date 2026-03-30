using GodGames.Application.Jobs;
using Hangfire;

namespace GodGames.Worker;

public class Worker(IRecurringJobManager jobManager) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Register the world tick job to run every 6 hours
        jobManager.AddOrUpdate<WorldTickJob>(
            "world-tick",
            job => job.ExecuteAsync(),
            "0 */6 * * *");

        return Task.CompletedTask;
    }
}
