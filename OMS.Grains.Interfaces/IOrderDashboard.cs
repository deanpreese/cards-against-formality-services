using OMS.Core.Models;
using OMS.Grains.Interfaces;
using OMS.Core.Common;

using Microsoft.Extensions.Logging;
using Orleans.Streams;


namespace OMS.Grains.Interfaces;

[Alias("IOrderDashboard")]
public interface IOrderDashboard : IGrainWithIntegerKey
{
    [Alias("GetLiveOrdersByTrader")]
    Task<List<LiveOrder>> GetLiveOrdersByTrader(UserInfo userInfo);

    [Alias("GetClosedOrdersByTrader")]
    Task<List<ClosedTrade>> GetClosedOrdersByTrader(UserInfo userInfo);

    [Alias("GetActiveTraders")]
    Task<List<UserInfo>> GetActiveTraders(UserInfo userInfo);

    [Alias("GetAllLiveOrders")]
    Task<List<LiveOrder>> GetAllLiveOrders(UserInfo userInfo);  

    [Alias("GetAllClosedOrders")]
    Task<List<ClosedTrade>> GetAllClosedOrders(UserInfo userInfo);  

}
