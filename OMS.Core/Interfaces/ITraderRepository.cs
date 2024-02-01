﻿using OMS.Core.Models;

namespace OMS.Core.Interfaces;

public interface ITraderRepository
{
    Task<int> AddTraderAsync(NewTrader user);
    Task<int> AuthenticateTraderAsync(int userID, string password, int groupNumber);
    Task<List<UserProfile>> GetUserProfileListAsync(int GroupNumber);
    Task<List<UserProfile>> GetUserProfileAsync(int TraderID , int GroupNumber);   
    Task<List<ActivityLog>> GetActivityLogEntriesAsync(int UserID );
    Task AddLogEntryAsync(ActivityLog log);

}
