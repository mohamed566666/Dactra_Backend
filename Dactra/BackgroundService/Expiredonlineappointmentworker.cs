using Dactra.Services.Interfaces;

namespace Dactra.BackgroundServices
{
    public class ExpiredOnlineAppointmentWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ExpiredOnlineAppointmentWorker> _logger;

        private static readonly TimeSpan CheckInterval = TimeSpan.FromMinutes(5);

        public ExpiredOnlineAppointmentWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ExpiredOnlineAppointmentWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "{Worker} started. Interval: {Interval} min.",
                nameof(ExpiredOnlineAppointmentWorker),
                CheckInterval.TotalMinutes);

            await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                await RunCheckAsync();
                await Task.Delay(CheckInterval, stoppingToken);
            }
        }

        private async Task RunCheckAsync()
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var statisticsSvc = scope.ServiceProvider
                    .GetRequiredService<IAppointmentStatisticsService>();

                _logger.LogDebug(
                    "{Worker} checking expired online appointments at {Time} UTC",
                    nameof(ExpiredOnlineAppointmentWorker),
                    DateTime.UtcNow);

                await statisticsSvc.MarkExpiredOnlineAppointmentsAsFailedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "{Worker} error during check",
                    nameof(ExpiredOnlineAppointmentWorker));
            }
        }
    }
}