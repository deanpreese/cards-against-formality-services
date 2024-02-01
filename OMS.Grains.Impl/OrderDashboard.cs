using OMS.Core.Models;
using OMS.Grains.Interfaces;
using OMS.Core.Common;
using OMS.Services.Data;

using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace OMS.Grains.Impl;

public class OrderDashboard : Grain, IOrderDashboard
{
    private readonly IDataService _dataService;
    private IAsyncStream<string> _stream = null!;
    private readonly ILogger<OrderDashboard> _logger;

    public OrderDashboard(IDataService dataService, ILogger<OrderDashboard> logger)
    {
        _logger = logger;
        _dataService = dataService;
    }

    public async Task<List<LiveOrder>> GetLiveOrdersByTrader(UserInfo userInfo)
    {
        List<LiveOrder> orders = new List<LiveOrder>();

        // FIXME: GetLiveOrdersByTrader OrderDashboard    
        //List<LiveOrder> orders = await _traderService.GetOrdersByTraderAsync(userInfo.UserID, userInfo.GroupNumber);
        //return await Task.FromResult(orders);
        
        return await Task.FromResult(orders);
    }


    public async Task<List<ClosedTrade>> GetClosedOrdersByTrader(UserInfo userInfo)
    {
        List<ClosedTrade> tradea = new List<ClosedTrade>();
        
        //FIXME: GetClosedOrdersByTrader OrderDashboard
        //tradea = await _traderService.GetOrdersByTraderAsync(userInfo.UserID, userInfo.GroupNumber);
        
        return await Task.FromResult(tradea);
    }


    public async Task<List<UserInfo>> GetActiveTraders(UserInfo userInfo)
    {
        List<UserInfo> orders = new List<UserInfo>();
        
        //FIXME: GetActiveTraders OrderDashboard
        //List<LiveOrder> orders = await _traderService.GetOrdersByTraderAsync(userInfo.UserID, userInfo.GroupNumber);
        //return await Task.FromResult(orders);
    
        return await Task.FromResult(orders);
    }


    public async Task<List<LiveOrder>> GetAllLiveOrders(UserInfo userInfo)
    {
        List<LiveOrder> orders = new List<LiveOrder>();
        orders = await _dataService.GetAllLiveOrders(userInfo.GroupNumber);
        return await Task.FromResult(orders);
    }

    public async Task<List<ClosedTrade>> GetAllClosedOrders(UserInfo userInfo)
    {
        List<ClosedTrade> orders = new List<ClosedTrade>();
        orders = await _dataService.GetAllClosedTrades(userInfo.GroupNumber);
        return await Task.FromResult(orders);

    }
}
