using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;

namespace OMS.Services.Trading;

public interface ITradingService
{
    Task<int> AddTraderAsync(NewTrader newTrader);
    Task<int> AuthenticateTraderAsync(UserInfo newTrader);
    Task<LiveOrder> ProcessNewOrderAsync(NewOrder newOrder);
}
