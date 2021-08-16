using System;
using System.Collections.Generic;

namespace DummyClient
{
    class SessionManager
    {
        static SessionManager session = new SessionManager();
        public static SessionManager Instance { get => session; }

        List<ServerSession> sessions = new List<ServerSession>();
        object _lock = new object();

        public void SendForEach()
        {
            lock (_lock)
            {
                foreach (var session in sessions)
                {
                    C_Chat chatPacket = new C_Chat();
                    chatPacket.chat = $"Hello Server!";
                    ArraySegment<byte> segment = chatPacket.Write();

                    session.Send(segment);
                }
            }
        }

        public ServerSession Generate()
        {
            lock (_lock)
            {
                ServerSession session = new ServerSession();
                sessions.Add(session);
                return session;
            }
        }
    }
}