using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using OMS.Core.Models;
using OMS.Common;


namespace OMS.Data
{

    public class StorageManager
    {
        private OrderManagementDbContext context;

        public StorageManager(OrderManagementDbContext context)
        {
            this.context = context;
        }
        
        // ******************************************************************************************* /
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
        private double DoNetDollars(string sym, double pnl)
        {
            double rtn = 0;

            TrackedInstruments ti = new TrackedInstruments();
            InstrumentData idata = ti.FindData(sym);

            rtn = InstrumentUtiliy.CalcNetDollars(idata, pnl);
            return rtn;

        }

        private double DoPNL(string sym, OrderAction closeOrderAction, double entryPX, double exitPX, int qty)
        {
            double PNL = 0;

            TrackedInstruments ti = new TrackedInstruments();
            InstrumentData idata = ti.FindData(sym);

            PNL = InstrumentUtiliy.CalcPNL(idata, closeOrderAction, entryPX, exitPX, qty);
            return PNL;
        }


        // ******************************************************************************************* /
        private void StoreOrder(LiveOrder ots)
        {
            OrderFlow ordFlow = new OrderFlow();
            ordFlow.OrderManagerID = ots.OrderManagerID;
            ordFlow.GroupID = ots.UserGroup;
            ordFlow.AuthToken = ots.AuthToken;
            ordFlow.ExecutedOrderID = ots.ExecutedOrderID;
            ordFlow.Instrument = ots.Instrument ?? PlatformConstants.InvalidSymbol;
            ordFlow.Leverage = ots.Leverage;
            ordFlow.Opposite = ots.Opposite;
            ordFlow.OrderAction = ots.OrderAction;
            ordFlow.OrderAction = ots.OrderAction;
            ordFlow.OrderPX = ots.OrderPX;
            ordFlow.OrderTime = ots.OrderTime;
            ordFlow.OrderType = ots.OrderType;
            ordFlow.PlatformOrderID = ots.PlatformOrderID;
            ordFlow.Quantity = ots.Quantity;
            ordFlow.RelatedOrderID = ots.RelatedOrderID;
            ordFlow.UserID = ots.UserID;

            //ordFlow.ScoreCardData = DAL.DataAccess.StatisticsScorecardHelper.SerializeScorecard(ots.TraderID, ots.OrderType);
            //ordFlow.ProfileData = DAL.DataAccess.UserProfileHelper.SerializeProfile(ots.TraderID, ots.OrderType);

            context.OrderFlow.Add(ordFlow);
            context.SaveChanges();

        }


        // ******************************************************************************************* /
        public double ProcessNewPosition(LiveOrder ots)
        {
            StoreOrder(ots);

            // Store new trade
            LiveOrder ord = new LiveOrder();
            ord.AuthToken = ots.AuthToken;
            ord.ExecutedOrderID = ots.ExecutedOrderID;
            ord.Instrument = ots.Instrument;
 
            // Buying to Cover  2   Selling -1
            // Buying 1    Selling Short   -2

            //if (ots.OrderAction == -1) { ots.OrderAction = -2; }
            //if (ots.OrderAction == 2) { ots.OrderAction = 1; }

            ord.OrderAction = ots.OrderAction;
            ord.OrderPX = ots.OrderPX;
            ord.OrderTime = ots.OrderTime;
            ord.OrderType = ots.OrderType;
            ord.PlatformOrderID = ots.PlatformOrderID;
            ord.Quantity = ots.Quantity;
            ord.RelatedOrderID = ots.RelatedOrderID;

            ord.UserID= ots.UserID;
            ord.UserGroup = ots.UserGroup;
            
            LiveOrderHelper loh = new LiveOrderHelper(context);
            loh.AddLiveOrder(ord);

            return GetDateStamp();
        }



        // ******************************************************************************************* /
        public double CloseOrder(LiveOrder ordertoclose, LiveOrder ots, bool isHistoricalOrder)
        {

            // Store the order for orderflow tracking
            StoreOrder(ots);

            ClosedTrade ho = new ClosedTrade();

            ho.UserID = ordertoclose.UserID;
            ho.GroupID = ordertoclose.UserGroup;
            ho.Instrument = ordertoclose.Instrument;
            ho.Quantity = ordertoclose.Quantity;
            ho.Leverage = ordertoclose.Leverage;

            if (ordertoclose.Opposite == 1)
            {
                ho.OppositeTrader = true;
            }
            else
            {
                ho.OppositeTrader = false;
            }

            ho.OpenPlatformOrderID = ordertoclose.PlatformOrderID;
            ho.OpenAuthToken = ordertoclose.AuthToken;
            ho.OpenExecutionID = ordertoclose.ExecutedOrderID;
            ho.OpenRelatedOrderID = ordertoclose.RelatedOrderID;
            ho.OpenOrderTime = ordertoclose.OrderTime;
            ho.OpenOrderPX = ordertoclose.OrderPX;

            ho.OpenOrderType = ordertoclose.OrderType;
            ho.OpenOrderAction = ordertoclose.OrderAction;

            ho.ClosePlatformOrderID = ots.PlatformOrderID;
            ho.CloseAuthToken = ots.AuthToken;
            ho.CloseExecutionID = ots.ExecutedOrderID;
            ho.CloseRelatedOrderID = ots.RelatedOrderID;
            ho.CloseOrderTime = ots.OrderTime;
            ho.CloseOrderPX = ots.OrderPX;

            ho.CloseOrderType = ots.OrderType;
            ho.CloseOrderAction = ots.OrderAction;

            //ho.MAE = ordertoclose.MAE;
            //ho.MFE = ordertoclose.MFE;

            double PNL = 0;
            double netchange = 0;

            PNL = DoPNL(ots.Instrument, ots.OrderAction, ordertoclose.OrderPX, ots.OrderPX, ots.Quantity);
            netchange = DoNetDollars(ots.Instrument, PNL);

            ho.PNL = PNL;
            ho.NetChange = netchange;

            HistOrderHelper hoh = new HistOrderHelper(context);
            hoh.StoreHistOrder(ho);

            LiveOrderHelper loh = new LiveOrderHelper(context);
            loh.DeleteOrder(ordertoclose.OrderManagerID);

            //if (!isHistoricalOrder)
            {
                /*
                ASyncServices.ASyncDataClient dc = new ASyncServices.ASyncDataClient();
                ASyncServices.UpdateGroupRankingsRequest ugr = new ASyncServices.UpdateGroupRankingsRequest(ordertoclose.TraderID);
                ASyncServices.UpdateTraderStatisticsRequest ats = new ASyncServices.UpdateTraderStatisticsRequest(ordertoclose.TraderID);
                dc.UpdateGroupRankingsAsync(ugr);
                dc.UpdateTraderStatisticsAsync(ats);
                */

            }

            return GetDateStamp();
        }


        // ******************************************************************************************* /
        public double IntegratePosition(LiveOrder ots, bool isHistoricalOrder)
        {

            double rtn = 100;

            try
            {
                LiveOrderHelper loh = new LiveOrderHelper(context);

                List<LiveOrder> orders =  loh.GetOrdersByTrader(ots.UserID, ots.Instrument, ots.OrderType);

                int cnt = orders.Count();
                if (cnt > 0)
                {
                    LiveOrder existingorder = orders.First();

                    // Buying to Cover  2   Selling -1
                    // Buying 1    Selling Short   -2

                    // ----------------------------------------------------------
                    // Curent Position Long
                    // ----------------------------------------------------------
                    if (existingorder.OrderAction == OrderAction.Buy)
                    {
                        // Buying       -- adding to Long Position
                        // Buy to Cover -- same as adding

                        if (ots.OrderAction > 0  )
                        {
                            ots.OrderAction = OrderAction.Buy;
                            ProcessNewPosition(ots);
                        }

                        //  Closing (Partial or Full) Long Postiion
                        //  by Selling or Selling short
                        if (ots.OrderAction == OrderAction.Sell)
                        {
                            int qty_toclose_long = existingorder.Quantity;

                            // Full position to close
                            if (qty_toclose_long == ots.Quantity)
                            {
                                ots.OrderAction = OrderAction.Sell;
                                CloseOrder(existingorder, ots, isHistoricalOrder);
                            }
                            else
                            {

                                List<LiveOrder> toclose = new List<LiveOrder>();

                                int qty_closed_long = 0;
                                qty_toclose_long = 0;

                                foreach (LiveOrder ord in orders)
                                {

                                    qty_toclose_long = qty_toclose_long + ord.Quantity;

                                    // Simply close the order
                                    if (ots.Quantity >= (qty_toclose_long))
                                    {
                                        qty_closed_long = qty_closed_long + ord.Quantity;
                                        ord.RelatedOrderID = ots.PlatformOrderID;
                                        ord.OrderManagerID = GetNextExecutionID();
                                        CloseOrder(ord, ots, isHistoricalOrder);
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

                                            CloseOrder(ord, ots, isHistoricalOrder);

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
                                    //new_order.ExecutedOrderID = ots.ExecutedOrderID;


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
                    // Curent Position Short
                    // ----------------------------------------------------------
                    if (existingorder.OrderAction == OrderAction.Sell)
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

                            int qty_toclose_short = existingorder.Quantity;

                            // Full position to close
                            if (qty_toclose_short == ots.Quantity)
                            {
                                ots.OrderAction = OrderAction.Buy;
                                CloseOrder(existingorder, ots, isHistoricalOrder);
                            }
                            else
                            {
                                List<LiveOrder> toclose = new List<LiveOrder>();
                                int qty_closed_short = 0;
                                qty_toclose_short = 0;

                                foreach (LiveOrder ord in orders)
                                {
                                    qty_toclose_short = qty_toclose_short + ord.Quantity;

                                    // Simply close the order
                                    if (ots.Quantity >= (qty_toclose_short))
                                    {
                                        qty_closed_short = qty_closed_short + ord.Quantity;
                                        ord.RelatedOrderID = ots.PlatformOrderID;

                                        ord.ExecutedOrderID = GetNextExecutionID();

                                        ots.OrderAction = OrderAction.Buy;
                                        CloseOrder(ord, ots, isHistoricalOrder);
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


                                            CloseOrder(ord, ots, isHistoricalOrder);

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

                                    //new_order.ExecutionID = ots.ExecutionID;
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
}

