using OMS.Core.Models;
using OMS.Grains.Interfaces;
using OMS.Core.Common;
using OMS.Services.Trading;

using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace OMS.Grains.Impl;

public class Authenticated : Grain, IAuthenticated
{

    private ITradingService _traderService;   
    private IAsyncStream<string> _stream = null!;
    private readonly ILogger<TraderGrain> _logger;

    private int _orderCount;


    public Authenticated(ITradingService traderService, ILogger<TraderGrain> logger) 
    {
        _traderService = traderService;
        _logger = logger;
        _orderCount = 0;
    }

    public async Task<int> AuthenticateTrader(UserInfo userInfo)
    {
        int om_auth = await _traderService.AuthenticateTraderAsync(userInfo);
        await AddToInfoStream("Trader Authenticated: " + om_auth.ToString());     
        return await Task.FromResult(om_auth); 
    }

    public async Task<int> AddOrder(int userid)
    {
        _orderCount =+ 1 ;
        await AddToInfoStream("User Authenticated: " + userid + "  " + _orderCount.ToString());     

        return await Task.FromResult(17); 
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
