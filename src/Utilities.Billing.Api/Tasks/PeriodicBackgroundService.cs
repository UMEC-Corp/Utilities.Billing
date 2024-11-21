namespace Utilities.Billing.Api.Tasks
{
    public abstract class PeriodicBackgroundService : BackgroundService
    {
        public readonly ILogger<PeriodicBackgroundService> Logger;
        private readonly PeriodicTaskSettings _settings;

        public PeriodicBackgroundService(ILogger<PeriodicBackgroundService> logger, PeriodicTaskSettings settings)
        {
            Logger = logger;
            _settings = settings;
        }
        protected abstract Task Execute();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var _ = Logger.BeginScope("TaskName: {TaskName}", this.GetType().Name);

            await ExecuteInWrap();

            using PeriodicTimer timer = new(TimeSpan.FromSeconds(_settings.Period));
            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await ExecuteInWrap();
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }

        private async Task ExecuteInWrap()
        {
            using var __ = Logger.BeginScope("TaskId: {TaskId}", Guid.NewGuid());
            try
            {
                Logger.LogInformation("Task started");
                await Execute();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Task failed");
            }
            finally
            {
                Logger.LogInformation("Task finished");
            }
        }
    }
}
