using System.Collections.Generic;

namespace Server
{
    class SessionManager
    {
        static SessionManager _session = new SessionManager();
        public static SessionManager Instance { get => _session; }

        int sessionId = 0;
        Dictionary<int, ClientSession> sessions = new Dictionary<int, ClientSession>();

        object _lock = new object();

        public ClientSession Generate()
        {
            lock (_lock)
            {
                int sessionId = ++this.sessionId;

                ClientSession session = new ClientSession();
                session.SessionId = sessionId;
                sessions.Add(sessionId, session);

                System.Console.WriteLine($"Connected : {sessionId}");

                return session;
            }
        }

        public ClientSession Find(int id)
        {
            lock (_lock)
            {
                ClientSession session = null;
                sessions.TryGetValue(id, out session);
                return session;
            }
        }

        public void Remove(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session.SessionId);
            }
        }
    }
}