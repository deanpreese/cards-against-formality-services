

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Orleans.Streams;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;

using OMS.Core.Common;
using Spectre.Console;

namespace  OMS.Algo;

class ClientProgram
{
    static async Task Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .UseOrleansClient(client =>
            {
                client.UseLocalhostClustering();
                client.AddMemoryStreams(PlatformConstants.OrderStreamProvider);
                client.AddMemoryStreams(PlatformConstants.InformationStreamProvider);
                
            })
            .UseConsoleLifetime()
            .Build();

        await host.StartAsync();

        var client = host.Services.GetRequiredService<IClusterClient>();
        var infoStreamProvider = client.GetStreamProvider(PlatformConstants.InformationStreamProvider)
                .GetStream<string>(PlatformConstants.MemoryStreamNamespace, "/Info");

        var orderStreamProvider = client.GetStreamProvider(PlatformConstants.OrderStreamProvider)
                    .GetStream<string>(PlatformConstants.MemoryStreamNamespace, "/Orders");

        await Task.WhenAll(

            orderStreamProvider.SubscribeAsync(
                async (data, token) =>
                {
                    await ProcessInfoDataAsync(AlgoMessageType.Order,data);
                }),
            infoStreamProvider.SubscribeAsync(
                async (data, token) =>
                {
                    await ProcessInfoDataAsync(AlgoMessageType.Trader,data);
                })
            
        );

        Console.WriteLine("Press Enter to terminate...");
        Console.ReadLine();
    }

    private static async Task ProcessInfoDataAsync(AlgoMessageType messageType, string data)
    {
        await Task.Run(() =>
        {
            if (messageType == AlgoMessageType.Order)
                AnsiConsole.MarkupLine("[green] "+ messageType +"[/]" + data);
            else
               AnsiConsole.MarkupLine("[white] "+ messageType +"[/]" + data);

        });
    }


}