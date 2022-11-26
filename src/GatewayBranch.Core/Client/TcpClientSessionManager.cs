using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace GatewayBranch.Core.Client
{
    internal class TcpClientSessionManager : ITcpClientSessionManager
    {
        private readonly ConcurrentDictionary<string, ISession> sessions = new ConcurrentDictionary<string, ISession>();
        public void Add(ISession session)
        {
            sessions.AddOrUpdate(session.MatchId, session, (_, __) => session);
        }

        public ISession GetSession(string matchId)
        {
            sessions.TryGetValue(matchId, out ISession session);
            return session;
        }

        public ISession GetSessionById(string sessionId)
        {
            return sessions.Values.FirstOrDefault(x => x.Id == sessionId);
        }

        public IEnumerable<ISession> GetSessions() => sessions.Values.ToList();

        public void RemoveById(string sessionId)
        {
            var session = sessions.Values.FirstOrDefault(x => x.Id == sessionId);
            if (session != default)
                sessions.TryRemove(session.MatchId, out session);
        }

        public void RemoveByMatchId(string matchId)
        {
            if (sessions.TryRemove(matchId, out var session))
                session.Dispose();
        }
    }
    internal interface ITcpClientSessionManager
    {
        void Add(ISession session);
        void RemoveByMatchId(string matchId);
        void RemoveById(string sessionId);
        ISession GetSession(string matchId);
        ISession GetSessionById(string sessionId);
        IEnumerable<ISession> GetSessions();
    }
}
