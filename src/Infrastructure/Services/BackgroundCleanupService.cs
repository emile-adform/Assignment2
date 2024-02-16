using Application.ExchangeRates.Commands;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

internal class BackgroundCleanupService : BackgroundService
{
    private readonly ILogger<BackgroundCleanupService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;
    private IServiceProvider Services { get; }
    int defaultCleanupInSeconds;
    public BackgroundCleanupService(IServiceProvider services, ILogger<BackgroundCleanupService> logger, IConfiguration configuration, IMediator mediator)
    {
        Services = services;
        _logger = logger;
        _configuration = configuration;
        _mediator = mediator;
        defaultCleanupInSeconds = _configuration.GetValue<int>("DefaultValues:DefaultCleanupValue");
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Timed Hosted Service running.");

        await CleanupAsync();

        using PeriodicTimer timer = new(TimeSpan.FromSeconds(defaultCleanupInSeconds));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanupAsync();
                _logger.LogInformation("Time Hosted Service Cleaned up");
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
            await _mediator.Send(new CleanupDatabaseCommand(), CancellationToken.None);
        }
    }
    //private async Task CleanupAsync()
    //{
    //    using (var scope = _serviceScopeFactory.CreateScope())
    //    {
    //        try
    //        {
    //            // Use MediatR to send the cleanup command
    //            await _mediator.Send(new CleanupDatabaseCommand(), CancellationToken.None);
    //        }
    //        catch (Exception ex)
    //        {
    //            // Handle exceptions as needed
    //            // Log the exception, retry, or take appropriate action
    //        }
    //    }
    //}
}
