using OMS.Core.Models;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OMS.Services.Queue;

public class ClosedOrderProcessor : BackgroundService
{
    private readonly object _lock = new object();

    private readonly ClosedOrderQueue _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ClosedOrderProcessor> _logger;

    public ClosedOrderProcessor( ILogger<ClosedOrderProcessor> logger, 
            ClosedOrderQueue newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var closedTrade in _orderChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessLiveOrderAsync(closedTrade, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task ProcessLiveOrderAsync(ClosedTrade closedTrade, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing closed trade " + closedTrade.ToString());   
        await Task.CompletedTask; 
    }
}
