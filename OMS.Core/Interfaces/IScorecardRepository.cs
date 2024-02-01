using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface IScorecardRepository
{
    public Task<ScoreCard> GenerateStatisticsAsync(int UserID, int tradesBack, int GroupNumber);
    public Task UpdateTraderStatisticsAsync(int UserID, int GroupNumber);
    public Task<double> GetTotalMAEAsync(int UserID, int duration, int GroupNumber);
    public Task<double> GetTotalMFEAsync(int UserID, int duration, int GroupNumber);
    public Task<double> GetPNLByTraderAsync(int UserID, int GroupNumber);
    public Task<List<double>> GetDataByTraderSparklineAsync(int UserID, int GroupNumber);
    public Task ReRankGroupAsync(int groupNumber);

}
