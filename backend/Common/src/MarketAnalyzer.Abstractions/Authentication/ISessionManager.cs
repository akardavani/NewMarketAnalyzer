using System.Collections.Generic;

namespace MarketAnalyzer.Abstractions
{
    public interface ISessionManager
    {
        List<UserSession> GetSessions();
        void UpsertSession(UserSession session);
        List<UserSession> GetUserSessions(int userId);
    }
}
