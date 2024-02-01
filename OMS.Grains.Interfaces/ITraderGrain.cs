
using OMS.Core.Models;

namespace OMS.Grains.Interfaces;

[Alias("ITraderGrain")]
public interface ITraderGrain : IGrainWithGuidKey
{
    [Alias("AddNewTrader")]
    Task<int> AddNewTrader(NewTrader newTrader);
}