using System;
using System.Threading.Tasks;
using OMS.Core.Interfaces;
using OMS.Data.Repositories;
using OMS.Core.Models;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace OMS.Services.Trading;

public class TradingService : ITradingService
{
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<TradingService> _logger;

    public TradingService(IUnitOfWork unitOfWork, ILogger<TradingService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;

    }

    public async Task<int> AddTraderAsync(NewTrader newTrader)
    {
        int traderID = 0;

        try
        {
            _logger.LogInformation("Adding new trader...");
            // Use TraderRepository to add a new trader
            traderID = await _unitOfWork.TraderRepository.AddTraderAsync(newTrader);
            // Commit transaction
            _unitOfWork.Commit();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        _logger.LogInformation($"Trader added  {traderID}  ");
        return traderID;
    }

    public async Task<int> AuthenticateTraderAsync(UserInfo newTrader)
    {
        int auth_code = 0;    

        if (newTrader.Password != null)
        {
            auth_code = await _unitOfWork.TraderRepository.AuthenticateTraderAsync(newTrader.UserID, newTrader.Password, newTrader.GroupNumber);    
            _unitOfWork.Commit();
        }    

        return auth_code;
    }


    // Example method to handle a new order
    public async Task<LiveOrder> ProcessNewOrderAsync(NewOrder newOrder)
    {

        LiveOrder liveOrder = MapOrder(newOrder);
        await _unitOfWork.UnderCoverRepository.AddToOrderFlowAsync(liveOrder);
        _unitOfWork.Commit();

        OrderStorageManager storageManager = new OrderStorageManager(_unitOfWork.OrderRepository);
        double r_val = await storageManager.ProcessNewOrder(liveOrder); 

        //StorageManager storageManager = new StorageManager(_unitOfWork.OrderRepository);
        //await storageManager.IntegratePosition(liveOrder,false);

        _unitOfWork.Commit();

        return liveOrder;
    }


    private LiveOrder MapOrder(NewOrder newOrder)
    {

        int om_id = 0;
        using (var generator = RandomNumberGenerator.Create())
        {
            var salt = new byte[4];
            generator.GetBytes(salt);
            om_id = BitConverter.ToInt32(salt, 0);
        }

        LiveOrder liveOrder = new LiveOrder();
        liveOrder.OrderPX = newOrder.OrderPX;
        liveOrder.OrderTime = DateTime.UtcNow;
        liveOrder.OrderManagerID = om_id;
        liveOrder.Instrument = newOrder.Instrument;
        liveOrder.OrderAction = newOrder.OrderAction;
        liveOrder.OrderType = newOrder.OrderType;
        liveOrder.PlatformOrderID = newOrder.PlatformOrderID;
        liveOrder.Quantity = newOrder.Quantity;
        liveOrder.UserID = newOrder.UserID;
        liveOrder.UserGroup = newOrder.UserGroup;
        liveOrder.Leverage = 1;
        liveOrder.Opposite = 0;
        return liveOrder;
    }



}
