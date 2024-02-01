
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;


using OMS.Core.Models;

namespace OMS.Client;

public static class OMSClient
{

    // -------------------------------------------------------------
    public static async Task<int> AddTraderAsync(NewTrader newTrader)
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:8786/api/order/add-new-trader", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Add Result: {result}");
                    return int.Parse(result); 
                }
                else
                {
                    Console.WriteLine($"Add Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
            }
    }



    // -------------------------------------------------------------
    public static async Task<int> AuthenticateTraderAsync(UserInfo newTrader)
    {
            using (HttpClient client = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(newTrader), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("http://localhost:8786/api/order/authenticate", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Auth Result: {result}");
                    return int.Parse(result); 
                }
                else
                {
                    Console.WriteLine($"Auth Failed. Status Code: {response.StatusCode}");
                    return 0;
                }
                
            }
    }


    // -------------------------------------------------------------
    public static async Task<int> SendOrderAsync(NewOrder newOrder)
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("http://localhost:8786/api/order/process-order", jsonContent);

            var result = await response.Content.ReadAsStringAsync();
            int code = int.Parse(result);

            /*
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"ProcessOrder Result: {result}");
                
                int code = int.Parse(result);
                
                if (code == 0)
                {
                    Console.WriteLine("Retrying ....");
                    var second_try = await client.PostAsync("http://localhost:8786/api/order/process-order", jsonContent);

                    var second_result = await second_try.Content.ReadAsStringAsync();
                    Console.WriteLine($"Second Try ProcessOrder Result: {second_result}");
                    
                    return int.Parse(second_result);
                }
                else
                {
                    return code;
                }
            

            }
            else
            {
                Console.WriteLine($"ProcessOrder Failed. Status Code: {response.StatusCode}");
                var error_result = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {error_result}");

                return 0; // Add return statement
            }
            */
            return code;

        }
    }


    // -------------------------------------------------------------
    public static int SendMLOrderAsync(NewOrder newOrder)
    {
        using (HttpClient client = new HttpClient())
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(newOrder), Encoding.UTF8, "application/json");
            var response = client.PostAsync("http://localhost:8786/api/order/process-ml-order", jsonContent);


            if (!response.IsCompletedSuccessfully)
            {
                Console.WriteLine($"ProcessOrder ML Result: {response.Result}");
                return 0; // Parse the result string to an integer before returning
            }
            else
            {
                Console.WriteLine($"ProcessOrder ML Failed. Status Code: {response.Exception}");
                var result = response.Result.Content.ReadAsStringAsync().Result;
                return Int32.Parse(result); // Add return statement
            }
        }
    }


}
