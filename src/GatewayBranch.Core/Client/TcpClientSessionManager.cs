using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GatewayBranch.Core.Client
{
    internal class TcpClientSessionManager : ITcpClientSessionManager
    {
        readonly ConcurrentDictionary<string, ISession> sessions = new ConcurrentDictionary<string, ISession>();
        public void Add(ISession session)
        {
            sessions.AddOrUpdate(session.PhoneNumber, session, (key, value) => session);
        }

        public ISession GetSession(string phoneNumber)
        {
            sessions.TryGetValue(phoneNumber, out ISession session);
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
            sessions.TryRemove(session.PhoneNumber, out session);
        }

        public void RemoveByPhoneNumber(string phoneNumber)
        {
            if (sessions.TryRemove(phoneNumber, out var session))
                session.Dispose();
        }
    }
    public interface ITcpClientSessionManager
    {
        void Add(ISession session);
        void RemoveByPhoneNumber(string phoneNumber);
        void RemoveById(string sessionId);
        ISession GetSession(string phoneNumber);
        ISession GetSessionById(string sessionId);
        IEnumerable<ISession> GetSessions();
    }
}
