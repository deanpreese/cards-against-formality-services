using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;

using OMS.Data;
using OMS.Data.Repositories;
using OMS.Core.Interfaces;
using OMS.Core.Common;
using OMS.Core.Models;
using OMS.Services.Trading;    


namespace X_APITester;

public class BasicServiceTests
{
        TradingService _service ;

        // -------------------------------------------------------------
        public BasicServiceTests(TradingService service)
        {
            _service = service;
        }

        public async Task<int> AddAndAuthenticate(int groupNumber)
        {
            int userID = 0;    
            userID = await AddNewTrader(groupNumber);
            await AuthenticateNewTrader(userID, groupNumber);

            return userID;
        }


        public async Task<int> AddNewTrader(int groupNumber)
        {

            Console.WriteLine("Adding New Trader...");

            Random random = new Random();

             NewTrader newTrader = new NewTrader
            {
                UserId = 0,
                Group = groupNumber,
                UserPwd= "abc",
                DisplayName ="Display"+random.Next(10,100),
                FirstName = "Gen First",
                LastName = "Gen Last",
                Email = "gen@example.com"
            };

            int trader_id = await _service.AddTraderAsync(newTrader);
            Console.WriteLine("New Trader ID: " + trader_id);
            return trader_id;
        }


        public async Task<int> AuthenticateNewTrader(int trader_id,int groupNumber)
        {
            Console.WriteLine("Authenticating New Trader...");

            UserInfo userInfo = new UserInfo
            {
                UserID = trader_id,
                GroupNumber = groupNumber,
                Password = "abc"
            };

            int auth_code = await _service.AuthenticateTraderAsync(userInfo);
            Console.WriteLine("Auth Code: " + auth_code);
            return auth_code;
        }

        public List<LiveOrder> GetOrdersByTrader(int UserID, int GroupNumber)
        {
            List<LiveOrder> orders = new List<LiveOrder>();
            /*
            var orders = _service.GetOrdersByTraderAsync(UserID, GroupNumber);
            Console.WriteLine("Orders: " + orders.Result.Count);

            foreach (LiveOrder order in orders.Result)
            {
                Console.WriteLine("OrderID: " + order.OrderManagerID);
            }

            return orders.Result;
            */

            return orders;

        }


}
