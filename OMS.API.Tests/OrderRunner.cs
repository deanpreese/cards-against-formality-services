using OMS.Data;
using OMS.Data.Repositories;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Services.Data;
using OMS.Services.Trading; 
using Microsoft.Extensions.Logging;   
using OMS.Logging;

namespace OMS.API.Tests;

public class OrderRunner
{

    public async Task SendOrder(OrderManagementDbContext context, NewOrder order)  
    {
        ILogger<TradingService>? _logger = null;
        using (var factory = LoggerFactory.Create(b => b.AddSpectreConsole())) {
            _logger = factory.CreateLogger<TradingService>();
        }

        IUnitOfWork unitOfWork = new UnitOfWork(context);
        TradingService _service = new TradingService(unitOfWork, _logger);

        LiveOrder orderResult = await _service.ProcessNewOrderAsync(order);
        //Console.WriteLine("Finished OrderRunner  --  Order PlatformOrderID: " + orderResult.PlatformOrderID  + "   " + orderResult.OrderAction + "  " + orderResult.OrderManagerID);
    }
    
}
