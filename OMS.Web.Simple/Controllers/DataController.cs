using Microsoft.AspNetCore.Mvc;
using OMS.Core.Common;
using OMS.Core;
using OMS.Data;
using OMS.Core.Models;
using OMS.Services.Trading;
using OMS.Services.Queue;
using OMS.Services.Data;

using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Spectre.Console;

using OMS.Grains.Interfaces;
using OMS.Services;

namespace OMS.Web;

[ApiController]
[Route("api/data")]
public class DataController : ControllerBase
{
    private ILogger<DataController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private IDataService _data_service;
    private IUnitOfWork _unitOfWork;
    private readonly NewOrderChannelService _newOrderChannelService;


    public DataController(ITradingService tradingService,
        ILogger<DataController> logger, OrderManagementDbContext context, 
        IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService, IDataService data_service)
    {
        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _unitOfWork = unitOfWork;
        _newOrderChannelService = newOrderChannelService;
        _data_service = data_service;

        
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
        List<LiveOrder> orders = new List<LiveOrder>();
        orders = await _data_service.GetAllLiveOrders(userInfo.GroupNumber);
        return Ok(orders);
    }
    

    [HttpPost("get-closed-trades")]
    public async Task<IActionResult> GetClosedTrades([FromBody] UserInfo userInfo)
    {
        List<ClosedTrade> orders = new List<ClosedTrade>();
        orders = await _data_service.GetAllClosedTrades(userInfo.GroupNumber);
        return Ok(orders);
    }


}
