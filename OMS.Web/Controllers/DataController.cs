using Microsoft.AspNetCore.Mvc;
using OMS.Core.Common;
using OMS.Core;
using OMS.Data;
using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Services.Queue;

using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

using OMS.Grains.Interfaces;

namespace OMS.Web;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
     private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;
    private ILogger<OMSController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private IUnitOfWork _unitOfWork;
    private readonly NewOrderChannelService _newOrderChannelService;


    public DataController(IClusterClient clusterClient, ITradingService tradingService,
        IGrainFactory grainFactory, ILogger<OMSController> logger, OrderManagementDbContext context, 
        IUnitOfWork unitOfWork,
        NewOrderChannelService newOrderChannelService )
    {
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _unitOfWork = unitOfWork;
        _newOrderChannelService = newOrderChannelService;

        
    }
    [HttpPost("add-tick")]
    public async Task<IActionResult> AddTick([FromBody] LastTick lastTick)
    {
        await Task.Delay(100); // Example of an async operation
        return Ok(0);
    }

    [HttpPost("add-feature-data")]
    public async Task<IActionResult> AddFeatureData([FromBody] FeatureData featureData)
    {
        await Task.Delay(100); // Example of an async operation
        return Ok(0);
    }


    [HttpPost("get-live-orders")]
    public async Task<IActionResult> GetLiveOrders([FromBody] UserInfo userInfo)
    {
        var dashboardGrain = _grainFactory.GetGrain<IOrderDashboard>(userInfo.UserID );
        var orders = await dashboardGrain.GetAllLiveOrders(userInfo); 

        return Ok(orders);
    }
    

    [HttpPost("get-closed-trades")]
    public async Task<IActionResult> GetClosedTrades([FromBody] UserInfo userInfo)
    {
        var dashboardGrain = _grainFactory.GetGrain<IOrderDashboard>(userInfo.UserID );
        var orders = await dashboardGrain.GetAllClosedOrders(userInfo);

        return Ok(orders);
    }


}
