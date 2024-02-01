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
[Route("api/order")]
public class OMSController : ControllerBase
{
    private readonly IClusterClient _clusterClient;
    private readonly IGrainFactory _grainFactory;
    private ILogger<OMSController> _logger;
    private OrderManagementDbContext _context;
    private ITradingService _trader_service;
    private IUnitOfWork _unitOfWork;
    private readonly NewOrderChannelService _newOrderChannelService;


    public OMSController(IClusterClient clusterClient, ITradingService tradingService,
        IGrainFactory grainFactory, ILogger<OMSController> logger, OrderManagementDbContext context, 
        IUnitOfWork unitOfWork, NewOrderChannelService newOrderChannelService )
    {
        _clusterClient = clusterClient;
        _grainFactory = grainFactory;
        _logger = logger;
        _trader_service = tradingService;
        _context = context;
        _unitOfWork = unitOfWork;
        _newOrderChannelService = newOrderChannelService;
        
    }

    [HttpPost("add-new-trader")]
    public async Task<IActionResult> AddNewTrader([FromBody] NewTrader newTrader)
    {
        var traderGrain = _grainFactory.GetGrain<ITraderGrain>(Guid.NewGuid());

        // TODO: ADD SUPPORT - WHEN TRADER IS ADDED AUTOMATICALLY AUTHENTICATE AND ADD TO AUTHENTICATED GRAIN

        var oid = await traderGrain.AddNewTrader(newTrader);   
        return Ok(oid);
    }

    [HttpPost("authenticate")]
    public async Task<IActionResult> AuthenticateTrader([FromBody] UserInfo userInfo)
    {
        // TODO: ADD WORK TO SUPPORT - WHEN TRADER IS AUTHENTICATED ADD ADD ID TO TRADER GRAIN
        var traderGrain = _grainFactory.GetGrain<IAuthenticated>(userInfo.UserID);
        var isAuthenticated = await traderGrain.AuthenticateTrader(userInfo);
        return Ok(isAuthenticated);
    }


    [HttpPost("process-order")]
    public async Task<ActionResult> ProcessOrder([FromBody] NewOrder order)
    {
        int om_id = 0;
        try
        {
            await _newOrderChannelService.WriteAsync(order);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine(ex.Message);
        }
        return Ok(om_id);
    }


}
