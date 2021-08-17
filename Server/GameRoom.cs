using System;
using System.Collections.Generic;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> sessions = new List<ClientSession>();
        JobQueue jobQueue = new JobQueue();

        public void Push(Action job)
        {
            jobQueue.Push(job);
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I Am {packet.playerId}";

            ArraySegment<byte> segment = packet.Write();

            foreach (var otherSession in sessions)
                otherSession.Send(segment);
        }

        public void Enter(ClientSession session)
        {
            sessions.Add(session);
            session.Room = this;   
        }
        public void Leave(ClientSession session)
        {
            sessions.Remove(session);                
        }
    }
}