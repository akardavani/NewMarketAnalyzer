using MarketAnalyzer.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MarketAnalyzer.Infrastructure.Authentication
{
    internal class SessionManager : ISessionManager, IDisposable
    {
        private readonly Timer _timer;
        private readonly TimeSpan _lifetime = TimeSpan.FromMinutes(10);
        /// <summary>
        /// The key is UserId
        /// </summary>
        private readonly Dictionary<int, List<UserSession>> _sessions = new(500);
        private readonly ReaderWriterLockSlim _locker = new(LockRecursionPolicy.NoRecursion);

        public SessionManager()
        {
            _timer = new Timer(
                callback: PurgeExpiredSesssions,
                state: this,
                dueTime: _lifetime,
                period: _lifetime);
        }

        public void UpsertSession(UserSession session)
        {
            _locker.EnterWriteLock();
            try
            {
                if (TryGetUnsafe(session.UserId, out var sessions))
                {
                    var oldSession = sessions.FirstOrDefault(e => e.Ip == session.Ip);
                    if (oldSession is not null)
                    {
                        oldSession.LastActivityTime = DateTime.UtcNow;
                    }
                    else
                    {
                        sessions.Add(session);
                    }
                }
                else
                {
                    _sessions[session.UserId] = [session];
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }
        }

        public List<UserSession> GetUserSessions(int userId)
        {
            _locker.EnterReadLock();
            try
            {
                if (TryGetUnsafe(userId, out var sessions))
                {
                    var now = DateTime.Now;
                    sessions.RemoveAll(e => now.Subtract(e.LastActivityTime) > _lifetime);
                    return sessions;
                }
                else
                {
                    return [];
                }
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public List<UserSession> GetSessions()
        {
            _locker.EnterReadLock();
            try
            {
                return _sessions
                    .SelectMany(e => e.Value)
                    .ToList();
            }
            finally
            {
                _locker.ExitReadLock();
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        private bool TryGetUnsafe(int userId, out List<UserSession> session)
        {
            return _sessions.TryGetValue(userId, out session);
        }

        private static void PurgeExpiredSesssions(object state)
        {
            var now = DateTime.Now;
            var sessionMenager = (SessionManager)state;
            sessionMenager._locker.EnterWriteLock();

            try
            {
                var logoutUsers = new List<int>(sessionMenager._sessions.Count);
                foreach (var pair in sessionMenager._sessions)
                {
                    var expiredSessions = pair.Value
                        .Where(e => now.Subtract(e.LastActivityTime) > sessionMenager._lifetime);
                    pair.Value.RemoveAll(e => now.Subtract(e.LastActivityTime) > sessionMenager._lifetime);
                    if (pair.Value.Count == 0)
                    {
                        logoutUsers.Add(pair.Key);
                    }
                }

                if (logoutUsers.Count > 0)
                {
                    foreach (var userId in logoutUsers)
                    {
                        sessionMenager._sessions.Remove(userId);
                    }
                }
            }
            finally
            {
                sessionMenager._locker.ExitWriteLock();
            }
        }


    }
}
