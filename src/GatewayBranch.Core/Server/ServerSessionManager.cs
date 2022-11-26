using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GatewayBranch.Core.Server
{
    public class ServerSessionManager : IServerSessionManager
    {
        private readonly ConcurrentDictionary<string, ISession> sessions = new ConcurrentDictionary<string, ISession>();
        public void Add(ISession session)
        {
            sessions.AddOrUpdate(session.Id, session, (_, __) => session);
        }

        public ISession GetSession(string sessionId)
        {
            sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public ISession GetSessionById(string sessionId)
        {
            sessions.TryGetValue(sessionId, out var session);
            return session;
        }

        public IEnumerable<ISession> GetSessions() => sessions.Values;

        public void RemoveById(string sessionId)
        {
            if (sessions.TryRemove(sessionId, out var session))
                session.CloseAsync();
        }

        public Task Send(string sessionId, byte[] data)
        {
            if (sessions.TryGetValue(sessionId, out var session))
                return session.Send(data);
            else
                throw new NullReferenceException($"session {sessionId} is not fond");
        }
    }
    public interface IServerSessionManager
    {
        void Add(ISession session);
        void RemoveById(string sessionId);
        Task Send(string sessionId, byte[] data);
        ISession GetSession(string sessionId);
        ISession GetSessionById(string sessionId);
        IEnumerable<ISession> GetSessions();
    }
}
