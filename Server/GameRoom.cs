using System;
using System.Collections.Generic;

namespace Server
{
    class GameRoom
    {
        List<ClientSession> sessions = new List<ClientSession>();
        object _lock = new object();

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = chat;

            ArraySegment<byte> segment = packet.Write();

            lock (_lock)
            {
                foreach (var otherSession in sessions)
                    otherSession.Send(segment);
            }
        }

        public void Enter(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Add(session);
                session.Room = this;   
            }
        }
        public void Leave(ClientSession session)
        {
            lock (_lock)
            {
                sessions.Remove(session);                
            }

        }
    }
}