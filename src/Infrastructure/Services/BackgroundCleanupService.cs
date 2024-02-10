using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    internal class BackgroundCleanupService : BackgroundService
    {
        private readonly ILogger<BackgroundCleanupService> _logger;
        private readonly IConfiguration _configuration;
        private IServiceProvider Services { get; }
        int defaultCleanupInHours;
        public BackgroundCleanupService(IServiceProvider services, ILogger<BackgroundCleanupService> logger, IConfiguration configuration)
        {
            Services = services;
            _logger = logger;
            _configuration = configuration;
            defaultCleanupInHours = _configuration.GetValue<int>("DefaultValues:DefaultCleanupValue");
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            await CleanupAsync();

            using PeriodicTimer timer = new(TimeSpan.FromHours(defaultCleanupInHours));

            try
            {
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    await CleanupAsync();
                    _logger.LogInformation("Cleaned up");
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Timed Hosted Service is stopping.");
            }
        }
        private async Task CleanupAsync()
        {
            using (var scope = Services.CreateScope())
            {
                var itemService =
                    scope.ServiceProvider
                        .GetRequiredService<IExchangeRatesService>();

                await itemService.CleanupAsync();
            }
        }
    }
}
