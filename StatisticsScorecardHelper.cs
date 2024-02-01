using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using OMS.Common;   
using OMS.Core.Models;

namespace OMS.Data
{
    public class StatisticsScorecardHelper
    {
        ColoredScreenWriter csw = new ColoredScreenWriter(0);
        
        private OrderManagementDbContext ldc;

        public StatisticsScorecardHelper(OrderManagementDbContext context)
        {
            this.ldc = context;
        }

        private ScoreCard GetScoreCardDataLocal(int UserID, int GroupNumber)
        {
            return (from u in ldc.ScoreCard
                    where u.UserID == UserID
                          && u.GroupID == GroupNumber
                    select u).Single();
        }
        
               
        
        public string SerializeScorecard(int UserID, int GroupNumber)
        {
            string strData = "";
            ScoreCard scd = GetScoreCardDataLocal(UserID, GroupNumber);
            StringWriter writer = new StringWriter(CultureInfo.InvariantCulture);
            XmlSerializer serializer = new XmlSerializer(scd.GetType());
            serializer.Serialize(writer, scd);
            strData = writer.ToString();
            return strData;
        }



        // ******************************************************************************************* /
        public ScoreCard GenerateStatistics(int UserID, int tradesBack, int GroupNumber)
        {

            List<ClosedTrade> orders = (from o in ldc.ClosedTrades
                                      where o.UserID == UserID
                                      //&& o. == GroupNumber
                                      select o).OrderByDescending(x => x.CloseOrderTime).ToList();


            int tradesToUse = tradesBack;
            if (tradesBack == -1)
                tradesToUse = orders.Count;

            // Do the calcs
            // ============================================

            int num_trades = 0;
            int winners = 0;
            int losers = 0;
            int longs = 0;
            int shorts = 0;
            double net_profit_long = 0;
            double net_profit_short = 0;
            double gross_profit = 0;
            double gross_loss = 0;
            double largest_winner = 0;
            double largest_loser = 0;
            int largest_winning_streak = 0;
            int largest_losing_streak = 0;
            int current_winning_streak = 0;
            int current_losing_streak = 0;
            double last_pnl = 0;
            double this_pnl = 0;
            double total_net_profit = 0;


            List<ClosedTrade> lastOrders = new List<ClosedTrade>();
            int lastOrdersCnt = 0;


            foreach (ClosedTrade ho in orders.Take(tradesToUse))
            {

                if ((lastOrdersCnt) < 101)
                {
                    lastOrders.Add(ho);
                }
                lastOrdersCnt++;	


                num_trades++;
                this_pnl = ho.PNL;
                total_net_profit = total_net_profit + this_pnl;

                // Winning Trade
                if (this_pnl > 0)
                {
                    gross_profit = gross_profit + this_pnl;
                    winners++;

                    if (last_pnl > 0) { current_winning_streak++; }


                    if (last_pnl < 0)
                    {
                        if (current_winning_streak > largest_winning_streak) { largest_winning_streak = current_winning_streak; }
                        current_winning_streak = 1;
                    }

                    if (this_pnl > largest_winner) largest_winner = this_pnl;

                    // Buying 1    Selling Short   -2
                    if (ho.OpenOrderAction == OrderAction.Buy)
                    {
                        longs++;
                        net_profit_long = net_profit_long + this_pnl;

                    }
                    else
                    {
                        shorts++;
                        net_profit_short = net_profit_short + this_pnl;
                    }
                }

                // Losing Trade
                if (this_pnl <= 0)
                {
                    gross_loss = gross_loss + this_pnl;
                    losers++;

                    if (last_pnl < 0)
                    {
                        current_losing_streak++;
                    }

                    if (last_pnl > 0)
                    {
                        if (current_losing_streak > largest_losing_streak)
                        {
                            largest_losing_streak = current_losing_streak;
                        }
                        current_losing_streak = 1;
                    }

                    if (ho.PNL < largest_loser)
                        largest_loser = ho.PNL;

                    // Buying 1    Selling Short   -2
                    if (ho.OpenOrderAction == OrderAction.Buy)
                    {
                        longs++;

                    }
                    else
                    {
                        shorts++;

                    }
                }

                last_pnl = this_pnl;

            }//end foreach


            List<double> pnlData = lastOrders.Select(ho => ho.PNL).ToList();
            StringWriter writer = new StringWriter();
            XmlSerializer serializer = new XmlSerializer(typeof(List<double>));
            serializer.Serialize(writer, pnlData);
            string dataXML = writer.ToString();


            ScoreCard scx = new ScoreCard();
            scx.Trades = num_trades;
            scx.Winners = winners;
            scx.Losers = losers;
            scx.Longs = longs;
            scx.Shorts = shorts;
            scx.NetProfitLong = Math.Round(net_profit_long, 2);
            scx.NetProfitShort = Math.Round(net_profit_short, 2);
            scx.GrossProfit = Math.Round(gross_profit, 2);
            scx.GrossLoss = Math.Round(gross_loss, 2);
            scx.LargestWinner = Math.Round(largest_winner, 2);
            scx.LargestLoser = Math.Round(largest_loser, 2);
            scx.LargestWinningStreak = largest_winning_streak;
            scx.LargestLosingStreak = largest_losing_streak;
            scx.UserID = UserID;
            scx.GroupID = GroupNumber;
            scx.TotalNetProfit = Math.Round(total_net_profit, 2);
            scx.TradeXML = dataXML;

            if (winners > 0)
                scx.WinLossRatio = Math.Round((Convert.ToDouble(winners) / Convert.ToDouble(num_trades)), 2);

            if (gross_profit > 0)
                scx.AveWin = Math.Round(gross_profit / winners, 2);

            if (gross_loss < 0)
                scx.AveLoss = Math.Round(gross_loss / losers, 2);

            return scx;
        }


        // ******************************************************************************************* /
        public void UpdateTraderStatistics(int UserID, int GroupNumber)
        {


            

            ScoreCard scData = GenerateStatistics(UserID, -1, GroupNumber);


            csw.LogMagentaType(100, " Trader Statistics Generated ");


            UpdateSpecificTraderRankData(UserID, 7, 2, GroupNumber);


            csw.LogMagentaType(100, " Trader RANK DATA Updated ");


            List<ScoreCard> scorecards = (from o in ldc.ScoreCard
                                              where o.UserID == UserID
                                              && o.GroupID == GroupNumber
                                              select o).ToList();

            int count = scorecards.Count();
            if (count > 0)
            {

                csw.LogMagentaType(100, count +  "   Scorecards Exist ");

                ScoreCard sc = scorecards.First();
                sc.Trades = scData.Trades;
                sc.Winners = scData.Winners;
                sc.Losers = scData.Losers;
                sc.Longs = scData.Longs;
                sc.Shorts = scData.Shorts;
                sc.NetProfitLong = scData.NetProfitLong;
                sc.NetProfitShort = scData.NetProfitShort;
                sc.GrossProfit = scData.GrossProfit;
                sc.GrossLoss = scData.GrossLoss;
                sc.LargestWinner = scData.LargestWinner;
                sc.LargestLoser = scData.LargestLoser;
                sc.LargestWinningStreak = scData.LargestWinningStreak;
                sc.LargestLosingStreak = scData.LargestLosingStreak;
                sc.UserID= UserID;
                sc.TotalNetProfit = scData.TotalNetProfit;
                sc.TradeXML = scData.TradeXML;
                sc.GroupID = GroupNumber;

                if (scData.Winners > 0)
                    sc.WinLossRatio = Math.Round((Convert.ToDouble(scData.Winners) / Convert.ToDouble(scData.Trades)), 2);

                if (scData.GrossProfit > 0)
                    sc.AveWin = Math.Round((double)scData.GrossProfit / (double)scData.Winners, 2);

                if (scData.GrossLoss < 0)
                    sc.AveLoss = Math.Round((double)scData.GrossLoss / (double)scData.Losers, 2);

                //sc.TotalMAEOne = GetTotalMAE(TraderID, 10, GroupNumber);
                //sc.TotalMFEOne = GetTotalMFE(TraderID, 10, GroupNumber);
                //sc.TotalMAETwo = GetTotalMAE(TraderID, 25, GroupNumber);
                //sc.TotalMFETwo = GetTotalMFE(TraderID, 25, GroupNumber);


                try
                {
                    ldc.SaveChanges();
                }
                catch (Exception e)
                {
                    try
                    {   
                        Console.WriteLine(e.Message);
                    }
                    catch (Exception err)
                    {
                        csw.LogRedType(100, "Error UpdateTraderStatistics -- " +  err );    
                    }
                }

                csw.LogMagentaType(100, count + "  ---- Scorecards Changed  ");

            }
            else
            {
                ldc.ScoreCard.Add(scData);
                ldc.SaveChanges();

                csw.LogMagentaType(100, count + "   Scorecards Created");
            }


            csw.LogMagentaType(100, " Stats Finished updating ");
        }






        // ******************************************************************************************* /
        public double GetTotalMAE(int UserID, int duration, int GroupNumber)
        {
            double mae_total = 0;

            List<ClosedTrade> orders = (from o in ldc.ClosedTrades
                                      where o.UserID == UserID
                                      //&& o.OpenOrderType == GroupNumber
                                      select o).OrderByDescending(x => x.CloseOrderTime).ToList();

            int tradesToUse = duration;
            if (duration == -1)
                tradesToUse = orders.Count;

            foreach (ClosedTrade ho in orders.Take(tradesToUse))
            {
                mae_total = mae_total + ho.MAE;
            }

            return mae_total;
        }

        // ******************************************************************************************* /
        public double GetTotalMFE(int UserID, int duration, int GroupNumber)
        {
            double mfe_total = 0;

            List<ClosedTrade> orders = (from o in ldc.ClosedTrades
                                      where o.UserID == UserID
                                      //&& o.OpenOrderType == GroupNumber
                                      select o).OrderByDescending(x => x.CloseOrderTime).ToList();

            int tradesToUse = duration;
            if (duration == -1)
                tradesToUse = orders.Count;

            foreach (ClosedTrade ho in orders.Take(tradesToUse))
            {
                mfe_total = mfe_total + ho.MFE;
            }

            return mfe_total;
        }


        // ******************************************************************************************* /
        private void UpdateSpecificTraderRankData(int UserID, int rankingWindow, int minTrades, int GroupNumber)
        {

            IQueryable<ClosedTrade> lastOrders = (from h in ldc.ClosedTrades
                                                where h.UserID == UserID
                                                //&& h.OpenOrderType == GroupNumber
                                                select h).OrderByDescending(x => x.CloseOrderTime).Take(1);

            double pnlHolder = 0;
            int countHolder = 0;
            double percentHolder = 0;


            if (lastOrders.Count() > 0)
            {
                DateTime dteLast = DateTime.UtcNow;
                DateTime dteOne = dteLast;
                TimeSpan tsTwo = new TimeSpan(rankingWindow, 0, 0, 0);
                DateTime dteTwo = dteLast.Subtract(tsTwo);

                var rankData = (from historder in ldc.ClosedTrades
                                where
                                historder.UserID == UserID
                                //&& historder.OpenOrderType == GroupNumber
                                && historder.CloseOrderTime < dteOne
                                && historder.CloseOrderTime >= dteTwo
                                group historder by new
                                {
                                    historder.UserID
                                } into g
                                select new
                                {
                                    UserID,
                                    PNL = (Double?)g.Sum(p => p.PNL),
                                    COUNT = g.Count()
                                });


                if (rankData.Count() > 0)
                {

                    double percentRnk = 0.0;    
                    var rd = rankData.First();
                    if (rd.PNL != null)
                    {
                        percentRnk = Math.Round((double)rd.PNL / rd.COUNT, 2);
                        pnlHolder = (double)rd.PNL;
                    }

                    countHolder = rd.COUNT;
                    percentHolder = percentRnk;
                    if (rd.COUNT < minTrades)
                    {
                        percentHolder = 0;
                    }
                }
            }


            List<ScoreCard> toChange = (from sc in ldc.ScoreCard
                                            where sc.UserID == UserID
                                            && sc.GroupID == GroupNumber
                                            select sc).ToList();

            foreach (var sdc in toChange)
            {

                Console.WriteLine("Trader " + UserID + "  Count: " + countHolder + "  PNL: " + pnlHolder + "  Percent: " + percentHolder);

                //sdc.RankAve = percentHolder;
                //sdc.RankCount = countHolder;
                //sdc.RankPNL = pnlHolder;
                ldc.SaveChanges();
            }

        }



        // ******************************************************************************************* /
        public double GetPNLByTrader(int UserID, int GroupNumber)
        {
            var data = (from x in ldc.ScoreCard
                        where x.UserID == UserID
                              && x.GroupID == GroupNumber
                        select x);

            double profit = 0.0;

            if (data.Any())
            {
                var firstData = data.FirstOrDefault();
                if (firstData != null)
                {
                    profit = (double)firstData.TotalNetProfit;
                }
            }


            double startPNL = 0.0;


            var start = (from ac in ldc.UserProfiles
                         where ac.UserID == UserID
                               && ac.TraderGroup== GroupNumber
                         select ac);

            if (start.Any())
            {
                //startPNL = (double) start.FirstOrDefault().StartingAccountSize;
            }



            return profit + startPNL;
        }





        // ******************************************************************************************* /
        public List<double> GetDataByTraderSparkline(int UserID, int GroupNumber)
        {
            List<double> sparkData = new List<double>();


            var data = (from x in ldc.ScoreCard
                        where x.UserID == UserID
                        && x.GroupID == GroupNumber
                        select x).First();

            XmlSerializer deSerializer = new XmlSerializer(typeof(List<double>));
            StringReader sr = new StringReader(data.TradeXML);

            sparkData = deSerializer.Deserialize(sr) as List<double> ?? new List<double>();

            return sparkData;
        }




        // ******************************************************************************************* /
        public void ReRankGroup(int groupNumber)
        {


            // -------------------------------------------
            // Zero all rankings to remove any laggards
            // -------------------------------------------
            List<UserProfile> theGroup =
                (from g in ldc.UserProfiles where g.TraderGroup == groupNumber select g).ToList();

            foreach (var u in theGroup)
            {
                //u.CurrentRank = 0;
                //ldc.SubmitChanges(ConflictMode.ContinueOnConflict);
            }

            // -------------------------------------------



            List<ScoreCard> groupedTraders = ((from scd in ldc.ScoreCard
                                                   join up in ldc.UserProfiles on scd.UserID equals up.UserID
                                                   where up.TraderGroup == groupNumber
                                                   && up.TraderRole == 1
                                                   select scd)).ToList();



            //List<ScoreCard> gb1 = groupedTraders.Where(x => x.RankAve > 0).OrderBy(y => y.RankAve).ToList();
            //List<ScoreCard> gb2 = groupedTraders.Where(x => x.RankAve < 0).OrderBy(y => y.RankAve).ToList();
            //List<ScoreCard> gb3 = new List<ScoreCardData>();

            /*
            foreach (var item2 in gb2)
            {
                gb3.Add(item2);
            }

            foreach (var item in gb1)
            {
                gb3.Add(item);
            }


            int rank = gb3.Count();
            int tradersUpdated = 0;

            int traderCount = 0;

            foreach (var scdItem in gb3)
            {
                List<UserProfile> traders = (from t in ldc.UserProfiles
                                             where t.UserName == scdItem.TraderID
                                             select t).ToList();

                foreach (UserProfile upTrader in traders)
                {

                    traderCount++;

                    if (upTrader.CurrentRank != rank)
                    {

                        tradersUpdated++;
                        csw.LogRedType(1, upTrader.CurrentRank + "   ----- >New Rank: " + rank + " Trader " + upTrader.UserName + "  Count: " + scdItem.RankCount + "  PNL: " + scdItem.RankPNL + "  Percent: " + scdItem.RankAve);
                        upTrader.CurrentRank = rank;
                        ldc.SubmitChanges(ConflictMode.ContinueOnConflict);
                    }
                    else
                    {
                        csw.Log(1, upTrader.CurrentRank  +  "    "  +  rank + "  Trader " + upTrader.UserName + "  Count: " + scdItem.RankCount + "  PNL: " + scdItem.RankPNL + "  Percent: " + scdItem.RankAve);
                    }


                    rank--;
                }

            }

            csw.LogRedType(1,tradersUpdated + " changed rank out of a possbile " + traderCount);
            */

        }



    }


}
