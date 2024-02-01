using OMS.Core.Models;
using OMS.Grains.Interfaces;
using OMS.Core.Common;
using OMS.Services.Trading;

using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace OMS.Grains.Impl;

public class TraderGrain : Grain, ITraderGrain
{
    private ITradingService _traderService;   
    private IAsyncStream<string> _stream = null!;
    private readonly ILogger<TraderGrain> _logger;

    public TraderGrain(ITradingService traderService, ILogger<TraderGrain> logger) 
    {
        _traderService = traderService;
        _logger = logger;

    }

    public async Task<int> AddNewTrader(NewTrader newTrader)
    {
        int userid = await _traderService.AddTraderAsync(newTrader);
        await AddToInfoStream("Trader Added " + userid + "  " + newTrader.DisplayName);  
        return await Task.FromResult(userid); 
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _stream = this.GetStreamProvider(PlatformConstants.InformationStreamProvider)
            .GetStream<string>(PlatformConstants.MemoryStreamNamespace, "/Info");

        await base.OnActivateAsync(cancellationToken);
    }

    public async Task AddToInfoStream(string info)
    {
        await _stream.OnNextAsync(info);
    }




}