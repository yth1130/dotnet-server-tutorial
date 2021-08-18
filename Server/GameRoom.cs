using System;
using System.Collections.Generic;
using ServerCore;

namespace Server
{
    class GameRoom : IJobQueue
    {
        List<ClientSession> sessions = new List<ClientSession>();
        JobQueue jobQueue = new JobQueue();
        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();

        public void Push(Action job)
        {
            jobQueue.Push(job);
        }

        // jobQueue안에서 하나의 스레드만 실행한다는게 보장되기 때문에 따로 락을 걸지 않는다. 
        public void Flush()
        {
            foreach (var otherSession in sessions)
                otherSession.Send(pendingList);

            System.Console.WriteLine($"Flushed {pendingList.Count} items");
            pendingList.Clear();
        }

        public void Broadcast(ClientSession session, string chat)
        {
            S_Chat packet = new S_Chat();
            packet.playerId = session.SessionId;
            packet.chat = $"{chat} I Am {packet.playerId}";

            ArraySegment<byte> segment = packet.Write();

            pendingList.Add(segment);
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