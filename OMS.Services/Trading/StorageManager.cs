using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using OMS.Core.Models;
using OMS.Data;
using OMS.Core.Common;
using OMS.Core.Interfaces;
using System.Drawing.Printing;

namespace OMS.Services.Trading;

public class StorageManager
{
    OrderRepository _repository;

    public StorageManager(IOrderRepository repository)
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


    // ******************************************************************************************* /
    public double ProcessNewPosition(LiveOrder orderToStore)
    {
        Console.WriteLine("Processing New Position "  +  orderToStore.OrderManagerID);
        _repository.AddLiveOrderAsync(orderToStore);
        return GetDateStamp();
    }



    // ******************************************************************************************* /
    public async Task<double> CloseOrder(LiveOrder orderToClose, LiveOrder orderToStore, bool isHistoricalOrder)
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

        //ho.MAE = ordertoclose.MAE;
        //ho.MFE = ordertoclose.MFE;

        double PNL = 0;
        double netChange = 0;

        PNL = InstrumentUtility.CalcPNL(orderToStore.Instrument, orderToStore.OrderAction, orderToClose.OrderPX, orderToStore.OrderPX, orderToStore.Quantity);
        netChange = InstrumentUtility.CalcNetDollars(orderToStore.Instrument, PNL);

        histOrder.PNL = PNL;
        histOrder.NetChange = netChange;
        await _repository.AddClosedOrder(histOrder);

        Console.WriteLine("Removing Order: " + orderToClose.OrderManagerID);
        await _repository.DeleteOrderAsyncByOrderManagerID(orderToClose.OrderManagerID);

        /*
        foreach (LiveOrder ord in ordersToRemove)
        {
            Console.WriteLine("Removing Order: " + ord.OrderManagerID);
            await _repository.DeleteOrderAsync(ord.OrderManagerID);
        }
        */
        return GetDateStamp();
    }


    // ******************************************************************************************* /
    public async Task<double> IntegratePosition(LiveOrder ots, bool isHistoricalOrder)
    {

        double rtn = 100;

        try
        {
            List<LiveOrder> orders = await _repository.GetOrdersByTrader(ots.UserID, ots.Instrument, ots.OrderAction);

            int cnt = orders.Count();
            if (cnt > 0)
            {
                LiveOrder existingOrder = orders.First();

                // Buying to Cover  2   Selling -1
                // Buying 1    Selling Short   -2

                // ----------------------------------------------------------
                // Current Position Long
                // ----------------------------------------------------------
                if (existingOrder.OrderAction == OrderAction.Buy)
                {
                    // Buying       -- adding to Long Position
                    // Buy to Cover -- same as adding

                    if (ots.OrderAction > 0  )
                    {
                        ots.OrderAction = OrderAction.Buy;
                        ProcessNewPosition(ots);
                    }

                    //  Closing (Partial or Full) Long Position
                    //  by Selling or Selling short
                    if (ots.OrderAction == OrderAction.Sell)
                    {
                        int qty_toClose_long = existingOrder.Quantity;

                        // Full position to close
                        if (qty_toClose_long == ots.Quantity)
                        {
                            ots.OrderAction = OrderAction.Sell;
                            await CloseOrder(existingOrder, ots, isHistoricalOrder);
                        }
                        else
                        {

                            List<NewOrder> toClose = new List<NewOrder>();

                            int qty_closed_long = 0;
                            qty_toClose_long = 0;

                            foreach (LiveOrder ord in orders)
                            {

                                qty_toClose_long = qty_toClose_long + ord.Quantity;

                                // Simply close the order
                                if (ots.Quantity >= (qty_toClose_long))
                                {
                                    qty_closed_long = qty_closed_long + ord.Quantity;
                                    ord.RelatedOrderID = ots.PlatformOrderID;
                                    ord.OrderManagerID = GetNextExecutionID();
                                    await CloseOrder(ord, ots, isHistoricalOrder);
                                }
                                else
                                {
                                    // Catch the partial order
                                    if (qty_closed_long < ots.Quantity)
                                    {
                                        // partially close the  close_qty 
                                        int close_qty = ots.Quantity - qty_closed_long;

                                        // gen partial order for rem_qty
                                        int rem_qty = ord.Quantity - close_qty;


                                        LiveOrder partial_order = new LiveOrder();
                                        partial_order.PlatformOrderID = ord.PlatformOrderID;
                                        partial_order.RelatedOrderID = ots.PlatformOrderID;
                                        partial_order.AuthToken = ord.AuthToken;

                                        partial_order.OrderManagerID = GetNextExecutionID();

                                        partial_order.Instrument = ord.Instrument;
                                        partial_order.Leverage = ord.Leverage;

                                        partial_order.Opposite = 0;

                                        if (ord.Opposite > 0)
                                            partial_order.Opposite = 1;

                                        // Buying to Cover  2   Selling -1
                                        // Buying 1    Selling Short   -2

                                        partial_order.OrderAction = ord.OrderAction;

                                        partial_order.OrderPX = ord.OrderPX;
                                        partial_order.OrderTime = ord.OrderTime;
                                        partial_order.OrderType = ord.OrderType;
                                        partial_order.Quantity = rem_qty;
                                        partial_order.UserID = ord.UserID;
                                        partial_order.UserGroup = ord.UserGroup;
                                        ProcessNewPosition(partial_order);

                                        ord.Quantity = close_qty;

                                        ord.ExecutedOrderID= GetNextExecutionID();

                                        await CloseOrder(ord, ots, isHistoricalOrder);

                                        qty_closed_long = qty_closed_long + ord.Quantity;
                                    }
                                }


                            }//end foreach

                            // check - is the qty closed < ord.Quantity?
                            // if true -- then  we need to generate reversal orders for the balance
                            //  EX:  was long 200  -- but sold 300  -- creating net short 100

                            if (qty_closed_long < ots.Quantity)
                            {
                                int qty_balance = ots.Quantity - qty_closed_long;

                                LiveOrder new_order = new LiveOrder();
                                new_order.PlatformOrderID = ots.PlatformOrderID;
                                new_order.RelatedOrderID = ots.PlatformOrderID;
                                new_order.AuthToken = ots.AuthToken;

                                new_order.ExecutedOrderID= GetNextExecutionID();

                                new_order.Instrument = ots.Instrument;
                                new_order.Leverage = ots.Leverage;
                                new_order.Opposite= ots.Opposite;
                                new_order.OrderPX = ots.OrderPX;
                                new_order.OrderTime = ots.OrderTime;
                                new_order.OrderType = ots.OrderType;
                                new_order.UserID= ots.UserID;
                                new_order.UserGroup = ots.UserGroup;

                                new_order.OrderAction = OrderAction.Sell;
                                new_order.Quantity = qty_balance;

                                ProcessNewPosition(new_order);
                            }




                        }

                    }

                }

                // Buying to Cover  2   Selling -1
                // Buying 1    Selling Short   -2

                // ----------------------------------------------------------
                // Current Position Short
                // ----------------------------------------------------------
                if (existingOrder.OrderAction == OrderAction.Sell)
                {
                    // Adding to Short Position
                    // Selling -- same as adding
                    if (ots.OrderAction < 0)
                    {
                        ots.OrderAction = OrderAction.Sell;
                        ProcessNewPosition(ots);
                    }

                    // Close (Partial or Full) Short Position
                    // by Buying or Buying to Cover
                    if ( ots.OrderAction > 0  )
                    {

                        int qty_toClose_short = existingOrder.Quantity;

                        // Full position to close
                        if (qty_toClose_short == ots.Quantity)
                        {
                            ots.OrderAction = OrderAction.Buy;
                            await CloseOrder(existingOrder, ots, isHistoricalOrder);
                        }
                        else
                        {
                            List<NewOrder> toClose = new List<NewOrder>();
                            int qty_closed_short = 0;
                            qty_toClose_short = 0;

                            foreach (LiveOrder ord in orders)
                            {
                                qty_toClose_short = qty_toClose_short + ord.Quantity;

                                // Simply close the order
                                if (ots.Quantity >= (qty_toClose_short))
                                {
                                    qty_closed_short = qty_closed_short + ord.Quantity;
                                    ord.RelatedOrderID = ots.PlatformOrderID;

                                    ord.ExecutedOrderID = GetNextExecutionID();

                                    ots.OrderAction = OrderAction.Buy;
                                    await CloseOrder(ord, ots, isHistoricalOrder);
                                }
                                else
                                {
                                    // Catch the partial order
                                    if (qty_closed_short < ots.Quantity)
                                    {
                                        // partially close the  close_qty 
                                        int close_qty = ots.Quantity - qty_closed_short;

                                        // gen partial order for rem_qty
                                        int rem_qty = ord.Quantity - close_qty;


                                        LiveOrder partial_order = new LiveOrder();
                                        partial_order.PlatformOrderID = ord.PlatformOrderID;
                                        partial_order.RelatedOrderID = ots.PlatformOrderID;
                                        partial_order.AuthToken = ord.AuthToken;

                                        partial_order.ExecutedOrderID = GetNextExecutionID();
                                        //partial_order.ExecutionID = ord.ExecutionID;


                                        partial_order.Instrument = ord.Instrument;
                                        partial_order.Leverage = ord.Leverage;

                                        partial_order.Opposite = ord.Opposite;

                                        if (ord.Opposite > 0)
                                            partial_order.Opposite = 1;

                                        partial_order.OrderAction = ord.OrderAction;
                                        partial_order.OrderPX = ord.OrderPX;
                                        partial_order.OrderTime = ord.OrderTime;
                                        partial_order.OrderType = ord.OrderType;
                                        partial_order.Quantity = rem_qty;
                                        partial_order.UserID= ord.UserID;
                                        partial_order.UserGroup = ord.UserGroup;    

                                        ProcessNewPosition(partial_order);

                                        ord.Quantity = close_qty;
                                        ots.OrderAction = OrderAction.Buy;
                                        ots.ExecutedOrderID = GetNextExecutionID();


                                        await CloseOrder(ord, ots, isHistoricalOrder);

                                        qty_closed_short = qty_closed_short + ord.Quantity;
                                    }


                                }


                            }// end foreach


                            // check - is the qty closed < ord.Quantity?
                            // if true -- then  we need to generate reversal orders for the balance
                            //  EX:  was short 200  -- but bot 300  -- creating net long 100

                            if (qty_closed_short < ots.Quantity)
                            {
                                int qty_balance = ots.Quantity - qty_closed_short;

                                LiveOrder new_order = new LiveOrder();
                                new_order.PlatformOrderID = ots.PlatformOrderID;
                                new_order.RelatedOrderID = ots.PlatformOrderID;
                                new_order.AuthToken = ots.AuthToken;

                                new_order.ExecutedOrderID = GetNextExecutionID();

                                new_order.Instrument = ots.Instrument;
                                new_order.Leverage = ots.Leverage;
                                new_order.Opposite = ots.Opposite;
                                new_order.OrderPX = ots.OrderPX;
                                new_order.OrderTime = ots.OrderTime;
                                new_order.OrderType = ots.OrderType;
                                new_order.UserID = ots.UserID;
                                new_order.UserGroup = ots.UserGroup;    

                                new_order.OrderAction = OrderAction.Buy;
                                new_order.Quantity = qty_balance;

                                ProcessNewPosition(new_order);
                            }




                        }
                    }
                }
            }
            else
            {
                ProcessNewPosition(ots);
            }

        }
        catch (Exception fail)
        {
            Console.WriteLine(fail.Message);
            rtn = 0;
        }


        return rtn;
    }

}

