using OMS.Core.Models;
using OMS.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Spectre.Console;
using System.Data;

namespace OMS.Services.Queue;

public class NewOrderProcessor : BackgroundService
{

    //private readonly ClosedOrderQueue _closedOrderChannelService;
    private readonly NewOrderQueue _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessor> _logger;

    public NewOrderProcessor( ILogger<NewOrderProcessor> logger, 
            NewOrderQueue newOrderChannelService,
            IServiceScopeFactory scopeFactory,
                IServiceProvider serviceProvider
                //,ClosedOrderQueue closedOrderChannelService
              )
    {
        _orderChannelService = newOrderChannelService;
        _scopeFactory = scopeFactory;
        _serviceProvider = serviceProvider;
        _logger = logger;
       // _closedOrderChannelService = closedOrderChannelService;
       
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var liveOrder in _orderChannelService.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessLiveOrderAsync(liveOrder, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }

    private async Task ProcessLiveOrderAsync(NewOrder newOrder, CancellationToken cancellationToken)
    {

        if (newOrder.OrderAction > 0)
        {
            AnsiConsole.MarkupLine("[lightgreen]" + DateTime.UtcNow  + " New Order " + newOrder.UserName + " " + newOrder.Instrument + " " + newOrder.OrderAction + " " + newOrder.OrderPX + "[/]");
        }else{
             AnsiConsole.MarkupLine("[indianred_1]" + DateTime.UtcNow +  " New Order " + newOrder.UserName + " " + newOrder.Instrument + " " + newOrder.OrderAction + " " + newOrder.OrderPX + "[/]");
        }

        await Task.CompletedTask; 
    }
}
