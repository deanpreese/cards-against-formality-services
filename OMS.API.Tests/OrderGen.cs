using OMS.Data;
using OMS.Data.Repositories;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Services;
using System.Diagnostics;

namespace OMS.API.Tests;

public class OrderGen
{
    private Stopwatch timer;

    public OrderGen(OrderManagementDbContext context)
    {
        timer = new Stopwatch();
       

        Console.WriteLine("Enter CSV File Name");
        string? csvFilePath = Console.ReadLine();

        if (csvFilePath != null)
        {
            var lines = File.ReadAllLines(csvFilePath);
            List<NewOrder> orders = new List<NewOrder>();

            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');

                NewOrder o = GetNewOrder();
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
                o.Instrument = "DEMO";

                orders.Add(o);
            }

            
            timer.Start();
            foreach (var order in orders)
            {
                //Thread.Sleep(250);    
                //Console.WriteLine("OrderGen  -- Order PlatformOrderID: " + order.PlatformOrderID + "   " + order.OrderAction);
                OrderRunner orderRunner = new OrderRunner();
                orderRunner.SendOrder(context, order).Wait();
            }
            timer.Stop();
            Console.WriteLine("   " ); 
            Console.WriteLine("Processed " + orders.Count  ); 
            Console.WriteLine("Time: " + timer.Elapsed);    
            Console.WriteLine("   " ); 

        }

    }

    private NewOrder GetNewOrder()
    {

        Random random = new Random();
        int rndm = random.Next(1000000, 5000000);

        return new NewOrder
        {
            AuthToken = 1111111,
            OrderType = OrderType.MARKET,
            PlatformOrderID = rndm,
            UserID = 0,
            UserGroup = 0,
            UserName = "ME",
            Instrument = "DEMO",
            OrderPX = 0,
            OrderAction = 0,
            Quantity = 0
        };
    }   






}
