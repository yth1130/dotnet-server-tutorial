using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    // 클라이언트의 대리자.
    class ClientSession: PacketSession
    {
        public int SessionId { get; set; }
        public GameRoom Room { get; set; }


        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // Thread.Sleep(5000);
            // Disconnect();

            // Program.Room.Enter(this);
            Program.Room.Push(() => Program.Room.Enter(this));
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            SessionManager.Instance.Remove(this);
            if (Room != null)
            {
                // Room.Leave(this);
                // Room = null;
                GameRoom room = Room;
                room.Push(() => room.Leave(this));
                Room = null;
            }
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        // 이중 패킷. ( (3,2) 좌표로 이동하고 싶다. )
        // 15(이동) 3 2(좌표)
        // public override int OnRecv(ArraySegment<byte> buffer)
        // {
        //     string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //     Console.WriteLine($"[From Client] {recvData}");
        //     return buffer.Count;
        // }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }

        public override void OnSend(int numOfBytes)
        {
            // System.Console.WriteLine($"[transferred bytes]: {numOfBytes}");
        }
    }

}