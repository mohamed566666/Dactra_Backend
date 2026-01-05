namespace Dactra.BackgroundServices
{
    public class CleanupExpiredTokensService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CleanupExpiredTokensService> _logger;

        public CleanupExpiredTokensService(
            IServiceScopeFactory scopeFactory,
            ILogger<CleanupExpiredTokensService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CleanupExpiredTokensService started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var now = DateTime.UtcNow;

                    var expiredOtps = await context.EmailVerifications
                        .Where(x => x.ExpiryDate < now)
                        .ToListAsync(stoppingToken);

                    if (expiredOtps.Any())
                    {
                        context.EmailVerifications.RemoveRange(expiredOtps);
                    }

                    var expiredRefreshTokens = await context.UserRefreshTokens
                        .Where(x => x.ExpireAt < now || x.IsUsed)
                        .ToListAsync(stoppingToken);

                    if (expiredRefreshTokens.Any())
                    {
                        context.UserRefreshTokens.RemoveRange(expiredRefreshTokens);
                    }

                    await context.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation(
                        "Cleanup completed. Removed {OtpCount} OTPs and {TokenCount} RefreshTokens",
                        expiredOtps.Count,
                        expiredRefreshTokens.Count
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while cleaning expired tokens.");
                }

                // ⏱️ كل 10 دقائق
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }
    }
}
