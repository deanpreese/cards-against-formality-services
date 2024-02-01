using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.UserSecrets;

using OMS.Core.Common;
using OMS.Core.Models;
using TestRunner;
using OMS.Client;

namespace OMS.TestRunner
{
    class Program
    {
            static async Task Main(string[] args)
            {
                Console.WriteLine("Starting TestRunner");
                Console.WriteLine("0 - AutoGenData");
                Console.WriteLine("1 - Run Standard Order Tests");
                Console.WriteLine("2 - Read CSV");

                int opt = 0;

                try
                {
                    string? input = Console.ReadLine();
                    opt = input != null ? int.Parse(input) : 0;
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid Option");
                    return;
                }

                switch (opt)
                {
                    case 0:
                        Console.WriteLine("Provide 3 numbers CSV- (Traders, Trades, Groups)");
                        string? input = Console.ReadLine();

                        if (input != null)
                        {
                            int[] nums = Array.ConvertAll(input.Split(','), int.Parse);

                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner();
                                await dataRunner.DataRunnerGen(nums[0], nums[1], nums[2]);
                            });
                        }
                        break;

                    case 1:
                        await Task.Run(async () => {
                            StandardOrderTests orderProcessor = new StandardOrderTests();
                            await orderProcessor.RunAll();
                        });

                        break;


                    case 2:
                        Console.WriteLine("Provide CSV file");
                        string? filename = Console.ReadLine();

                        if (filename != null)
                        {
                            await Task.Run(async () => {
                                DataRunner dataRunner = new DataRunner();
                                await dataRunner.ExecuteOrdersFromCsv(filename);
                            });
                        }

                        break;

                    default:
                        break;
                }


                Console.WriteLine("Process Complete - Press any key to exit");
                Console.ReadLine();

            }
    }

    
    
}