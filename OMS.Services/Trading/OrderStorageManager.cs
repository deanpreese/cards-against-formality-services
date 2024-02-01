using OMS.Core.Models;
using OMS.Data;
using OMS.Core.Common;
using OMS.Core.Interfaces;

namespace OMS.Services.Trading;

public class OrderStorageManager
{
    OrderRepository _repository;

    public OrderStorageManager(IOrderRepository repository)
    {
        _repository = (OrderRepository)repository;
    }

    
    public int GetNextExecutionID()
    {
        int rtnVal = 0;
        Random random = new Random();
        rtnVal = random.Next(1000000, 5000000);
        return rtnVal;
    }

    public double GetDateStamp()
    {
        DateTime dtn = DateTime.UtcNow;
        DateTime startdate = new DateTime(2010, 1, 1);

        // For Days
        long elapsedTicks = dtn.Ticks - startdate.Ticks;
        TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
        return Convert.ToDouble(elapsedSpan.TotalDays);
    }


    public async Task<double> ProcessNewOrder(LiveOrder newOrder)
    {
        double r_val = 100.0;
        try
        {
            List<LiveOrder> orders = await _repository.GetOrdersByTrader(newOrder.UserID, newOrder.Instrument);

            if (orders.Any())
            {
                ProcessExistingOrders(orders, newOrder);
            }
            else
            {
                r_val = ProcessNewPosition(newOrder);
            }
        }
        catch (Exception fail)
        {
            Console.WriteLine(fail.Message);
            r_val = 0;
        }

        return await Task.FromResult(r_val);
    }


    private async void ProcessExistingOrders(List<LiveOrder> orders, LiveOrder newOrder)
    {
        LiveOrder existingOrder = orders.First();


        if ((existingOrder.OrderAction == OrderAction.Buy && newOrder.OrderAction == OrderAction.Buy) ||
                (existingOrder.OrderAction == OrderAction.Sell && newOrder.OrderAction == OrderAction.Sell))
        {
            double rtn_val =  ProcessNewPosition(newOrder);
            //Console.WriteLine("Adding to existing position "  +  existingOrder.OrderManagerID + " " + rtn_val);
        }
        else 
        {
            // means that the - existingOrder - is the order to close
            // For right now we assume that the single order quantity = newOrder quantity
            //if (orders.Count == 1)
            {
                //Console.WriteLine("Closing an existing position "  +  existingOrder.OrderManagerID);
                await CloseOrder(existingOrder, newOrder);
            }
            // Need to figure out how to close out these orders
        }
    }




    private void ProcessLongPosition(List<NewOrder> orders, NewOrder ots, bool isHistoricalOrder)
    {
        // Logic for processing long position
        // Extract code from IntegratePosition that processes long positions
        // This may include further breaking down into smaller methods
    }

    private void ProcessShortPosition(List<NewOrder> orders, NewOrder ots, bool isHistoricalOrder)
    {
        // Logic for processing short position
        // Extract code from IntegratePosition that processes short positions
        // This may include further breaking down into smaller methods
    }

    public double ProcessNewPosition(LiveOrder order)
    {
        //Console.WriteLine("OM -- Processing New Position "  +  order.OrderManagerID);
        _repository.AddLiveOrderAsync(order);
        return GetDateStamp();
    }


        // ******************************************************************************************* /
        public async Task<double> CloseOrder(LiveOrder orderToClose, LiveOrder orderToStore)
        {

            ClosedTrade histOrder = new ClosedTrade();
            histOrder.UserID = orderToClose.UserID;
            histOrder.GroupID = orderToClose.UserGroup;
            histOrder.Instrument = orderToClose.Instrument;
            histOrder.Quantity = orderToClose.Quantity;
            histOrder.Leverage = orderToClose.Leverage;
            histOrder.OppositeTrader = false;
            histOrder.OpenPlatformOrderID = orderToClose.PlatformOrderID;
            histOrder.OpenAuthToken = orderToClose.AuthToken;
            histOrder.OpenExecutionID = orderToClose.ExecutedOrderID;
            histOrder.OpenRelatedOrderID = orderToClose.RelatedOrderID;
            histOrder.OpenOrderTime = orderToClose.OrderTime;
            histOrder.OpenOrderPX = orderToClose.OrderPX;
            histOrder.OpenOrderType = orderToClose.OrderType;
            histOrder.OpenOrderAction = orderToClose.OrderAction;
            histOrder.ClosePlatformOrderID = orderToStore.PlatformOrderID;
            histOrder.CloseAuthToken = orderToStore.AuthToken;
            histOrder.CloseExecutionID = orderToStore.ExecutedOrderID;
            histOrder.CloseRelatedOrderID = orderToStore.RelatedOrderID;
            histOrder.CloseOrderTime = orderToStore.OrderTime;
            histOrder.CloseOrderPX = orderToStore.OrderPX;
            histOrder.CloseOrderType = orderToStore.OrderType;
            histOrder.CloseOrderAction = orderToStore.OrderAction;

            histOrder.MAE = orderToClose.MAE; 
            histOrder.MFE = orderToClose.MFE;

            double PNL = 0;
            double netChange = 0;

            PNL = InstrumentUtility.CalcPNL(orderToStore.Instrument, orderToStore.OrderAction, orderToClose.OrderPX, orderToStore.OrderPX, orderToStore.Quantity);
            netChange = InstrumentUtility.CalcNetDollars(orderToStore.Instrument, PNL);

            histOrder.PNL = PNL;
            histOrder.NetChange = netChange;
            await _repository.AddClosedOrder(histOrder);

            //Console.WriteLine("Removing Order: " + orderToClose.OrderManagerID);

            await _repository.DeleteOrderAsyncByOrderManagerID(orderToClose.OrderManagerID);

            return GetDateStamp();
        }

}




