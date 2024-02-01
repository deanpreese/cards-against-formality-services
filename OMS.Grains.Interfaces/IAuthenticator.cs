using OMS.Core.Models;

namespace OMS.Grains.Interfaces;


[Alias("IAuthenticator")]

public interface IAuthenticated : IGrainWithIntegerKey
{
    [Alias("AuthenticateTrader")]
    Task<int> AuthenticateTrader(UserInfo userInfo);
    
    [Alias("AddOrder")]
    Task<int> AddOrder(int traderId);
}