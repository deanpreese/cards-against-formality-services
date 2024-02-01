using System.Text;
using Newtonsoft.Json;
using OMS.Client;
using OMS.Core.Models;
using OMS.Core.Common;

namespace TestRunner;

public class DataGen
{

    public DataGen()
    {
    }

    public DataGen(int traders, int trades, int groups)
    {
        RunDataGen(traders, trades, groups);
    }

    public void RunDataGen(int traders, int trades, int groups)
    {
        // Generate Traders
        List<NewTrader> trader_list = GenerateTraders(traders, groups);
        //Authenticate Trader
        List<NewTrader> verified_traders = VerifyTraders(trader_list);
        //Add Orders
        List<NewOrder> order_list = GenerateOrders(verified_traders, trades, groups);
        //Execute Orders

        foreach (var order in order_list)
        {
            Task task = OMSClient.SendOrderAsync(order);

            while(!task.IsCompleted)
            {
                //Thread.Sleep(250);
            }
            
        }
    }



    // -------------------------------------------------------------
    public static List<NewTrader> GenerateTraders(int traders, int group)
    {
        List<NewTrader> generated_traders = new List<NewTrader>();
        for (int i = 0; i < traders; i++)
        {
            /*    
            {
            "group": 0,
            "userId": 0,
            "displayName": "string",
            "userPwd": "string",
            "firstName": "string",
            "lastName": "string",
            "email": "string"
            }
            */

            NewTrader newTrader = new NewTrader
            {
                UserId = 0,
                Group = group,
                UserPwd= "abc",
                DisplayName ="Gen Display",
                FirstName = "Gen First",
                LastName = "Gen Last",
                Email = "gen@example.com"
            };

            Task<int>  trader = OMSClient.AddTraderAsync(newTrader);
            newTrader.UserId = trader.Result;
            generated_traders.Add(newTrader);
        }
        return generated_traders;
    }


    // -------------------------------------------------------------
    public static List<NewTrader> VerifyTraders(List<NewTrader> traders)
    {
        List<NewTrader> verified_traders = new List<NewTrader>();

        foreach (var trader in traders)
        {

            UserInfo u = new UserInfo();
            u.UserID = trader.UserId;
            u.GroupNumber = trader.Group;
            u.Password = trader.UserPwd;

            
            Task<int>  tid = OMSClient.AuthenticateTraderAsync(u);

            if (tid.Result == 0)
            {
                Console.WriteLine($"Auth Failed.  { trader.UserId}" );
            }else
            {    
                Console.WriteLine($"Auth Success.  { trader.UserId}" );
                verified_traders.Add(trader);
            }
        }
        return verified_traders;    
    }


    // -------------------------------------------------------------
    public static List<NewOrder> GenerateOrders(List<NewTrader> newTraders, int trades, int groups)
    {
        Random random = new Random();
        List<NewOrder> generated_orders = new List<NewOrder>();

        foreach (NewTrader trader in newTraders)
        {
            for (int groupNumber = 0; groupNumber <= groups; groupNumber++)
            {
                for (int i = 0; i < trades; i++)
                {

                    NewOrder BuyOrder = new NewOrder
                    {
                        AuthToken = 1111111,
                        OrderType = OrderType.MARKET,
                        PlatformOrderID = random.Next(1000000, 5000000),
                        UserID = trader.UserId,
                        UserGroup = trader.Group,
                        UserName = "ME",
                        Instrument = "DEMO",
                        OrderPX = random.Next(10, 30),
                        OrderAction = OrderAction.Buy,
                        Quantity = 100
                    };

                    NewOrder SellOrder = new NewOrder
                    {
                        AuthToken = 1111111,
                        OrderType = OrderType.MARKET,
                        PlatformOrderID = random.Next(1000000, 5000000),
                        UserID = trader.UserId,
                        UserGroup = trader.Group,
                        UserName = "ME",
                        Instrument = "DEMO",
                        OrderPX = random.Next(10, 30),
                        OrderAction = OrderAction.Sell,
                        Quantity = 100
                    };

                    generated_orders.Add(BuyOrder);
                    generated_orders.Add(SellOrder);

                }
            }
        }

        
        Random rng = new Random();
        int n = generated_orders.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            NewOrder value = generated_orders[k];
            generated_orders[k] = generated_orders[n];
            generated_orders[n] = value;
        }
        
        
        foreach (NewOrder no in generated_orders)
        {
            Console.WriteLine("Order: " + no.UserID + " " + no.OrderAction + " " + no.OrderPX + " " + no.Quantity);
        }
        return generated_orders;

    }



}
