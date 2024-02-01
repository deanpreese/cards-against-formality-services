using System;
using OMS.Core.Common;
using OMS.Core.Models;  
using OMS.Client;
using System.Diagnostics;


namespace TestRunner;


    public class DataRunner
    {

        public async Task ExecuteOrdersFromCsv(string csvFilePath)
        {
            var lines = await File.ReadAllLinesAsync(csvFilePath);

            var orders = new List<NewOrder>();
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');

                Random random = new Random();
                int rndm = random.Next(1000000, 5000000);

                NewOrder o = new NewOrder
                {
                    AuthToken = 1111111,
                    OrderType = OrderType.MARKET,
                    PlatformOrderID = rndm,
                    UserName = "ME",
                    Instrument = "DEMO",
                    OrderPX = 0,
                    OrderAction = 0,
                    Quantity = 0
                };

                o.UserID = int.Parse(values[0]);
                o.UserGroup = int.Parse(values[1]);

                if (values[2] == "Buy")
                {
                    o.OrderAction = OrderAction.Buy;
                }
                else if (values[2] == "Sell")
                {
                    o.OrderAction = OrderAction.Sell;
                }

                o.Quantity = int.Parse(values[3]);
                o.OrderPX = double.Parse(values[4]);
                o.Instrument = values[5];

                orders.Add(o);
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
                await RunOrderList(orders);
            stopwatch.Stop();
            Console.WriteLine($"Process Complete - Elapsed Time: {stopwatch.Elapsed} for {orders.Count} Orders  - Press any key to exit");
            Console.ReadLine();


        }



        public async Task DataRunnerGen(int traders, int trades, int groups)
        {
            List<NewTrader> trader_list = DataGen.GenerateTraders(traders, groups);
            List<NewTrader> verified_traders = DataGen.VerifyTraders(trader_list);
            List<NewOrder> order_list = DataGen.GenerateOrders(verified_traders, trades, groups);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            await RunOrderListAsync(order_list);
            stopwatch.Stop();
            Console.WriteLine($"Process Complete - Elapsed Time: {stopwatch.Elapsed} for {order_list.Count} Orders  - Press any key to exit");
            Console.ReadLine();
        }
        public async Task RunOrderListAsync(List<NewOrder> orderList)
        {
            await Task.Run(async () =>
            {
                int on1 = orderList.Count ;

                foreach (var currentOrder in orderList)
                {
                    await SendOrderAsync(currentOrder);

                    on1 -= 1;
                    Console.WriteLine($"Remain  {on1} ");
                    //Thread.Sleep(100);
                }
            });
        }


        public async Task RunOrderList(List<NewOrder> orderList)
        {
                await Task.Run(async () =>
            {
                int on1 = orderList.Count ;

                foreach (var currentOrder in orderList)
                {
                    await SendOrderAsync(currentOrder);

                    on1 -= 1;
                    Console.WriteLine($"Remain  {on1} ");
                    Thread.Sleep(250);
                }
            });
        }


        public async Task<int> SendOrderAsync(NewOrder order)
        {
            int sendOrderResult = await OMSClient.SendOrderAsync(order);    
            return sendOrderResult;
        }


    }