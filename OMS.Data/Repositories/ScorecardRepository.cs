using OMS.Core.Interfaces;
using OMS.Core.Models;

namespace OMS.Data;

public class ScorecardRepository : IScorecardRepository
{
    public Task<ScoreCard> GenerateStatisticsAsync(int UserID, int tradesBack, int GroupNumber)
    {
        throw new NotImplementedException();
    }

    public Task<List<double>> GetDataByTraderSparklineAsync(int UserID, int GroupNumber)
    {
        throw new NotImplementedException();
    }

    public Task<double> GetPNLByTraderAsync(int UserID, int GroupNumber)
    {
        throw new NotImplementedException();
    }

    public Task<double> GetTotalMAEAsync(int UserID, int duration, int GroupNumber)
    {
        throw new NotImplementedException();
    }

    public Task<double> GetTotalMFEAsync(int UserID, int duration, int GroupNumber)
    {
        throw new NotImplementedException();
    }

    public Task ReRankGroupAsync(int groupNumber)
    {
        throw new NotImplementedException();
    }

    public Task UpdateTraderStatisticsAsync(int UserID, int GroupNumber)
    {
        throw new NotImplementedException();
    }
}
