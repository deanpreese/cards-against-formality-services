using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;
using OMS.Data;

using OMS.Grains.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OMS.Services.Queue;

public class NewOrderProcessorService : BackgroundService
{
    private readonly object _lock = new object();

    private readonly NewOrderChannelService _orderChannelService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NewOrderProcessorService> _logger;

    public NewOrderProcessorService( ILogger<NewOrderProcessorService> logger, 
            NewOrderChannelService newOrderChannelService,
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
            using (var scope = _scopeFactory.CreateScope())
            {
                var scopedContext = scope.ServiceProvider.GetRequiredService<OrderManagementDbContext>();
                UnitOfWork unitOfWork = new UnitOfWork(scopedContext);
                ILogger<TradingService> logger = scope.ServiceProvider.GetRequiredService<ILogger<TradingService>>();
                TradingService _trader_service = new TradingService(unitOfWork, logger);   
                
                Task<LiveOrder> live = _trader_service.ProcessNewOrderAsync(newOrder);

                int om_id = live.Result.OrderManagerID;

                if (om_id != 0)
                {
                    //var _grainFactory = scope.ServiceProvider.GetRequiredService<IGrainFactory>();
                    //var orderGrain = _grainFactory.GetGrain<IAuthenticated>(newOrder.UserID);
                    //await orderGrain.AddOrder(newOrder.UserID);

                    AnsiConsole.MarkupLine("Order Processed " + newOrder.UserID  + "  " +  om_id);
                }
                else
                {
                    AnsiConsole.MarkupLine("Error Processing " + newOrder.UserID + "  " +  live.Result.PlatformOrderID);
                }
                    
            }
        await Task.CompletedTask; 
    }
}
