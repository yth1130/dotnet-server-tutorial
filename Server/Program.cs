using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class Packet
    {
        // ushort는 2바이트. 0~64k
        public ushort size; // 패킷의 종류만 보고 크기를 파악하기 힘들기 때문에 첫 인자로 크기를 넣어준다. 자신의 크기를 포함할 지는 정해야 함.
        public ushort packetId; // 패킷의 종류.
        // public string name;
        // public List<int> skills = new List<int>();
    }
    class LoginOkPacket : Packet
    {
        
    }
    class GameSession: PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // Packet packet = new Packet() { size = 100, packetId = 10 };

            // [ 100 ] [ 10 ]
            // byte[] sendBuff = new byte[4096];
            // byte[] buffer = BitConverter.GetBytes(knight.hp);
            // byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            // Array.Copy(buffer, 0, sendBuff, 0, buffer.Length);
            // Array.Copy(buffer2, 0, sendBuff, buffer.Length, buffer2.Length);

            // ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            // byte[] buffer = BitConverter.GetBytes(packet.size);
            // byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            // Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            // Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            // ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            // byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            // Send(sendBuff);

            Thread.Sleep(5000);

            Disconnect();
            // Disconnect();
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
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
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + 2);
            Console.WriteLine($"RecvPacketId:{id}/Size:{size}");
        }

        public override void OnSend(int numOfBytes)
        {
            System.Console.WriteLine($"[transferred bytes]: {numOfBytes}");
        }
    }


    class Program
    {
        static Listener listener = new Listener();
        
        static void Main(string[] args)
        {
            //DNS(Domain Name System)
            //172.1.2.3 이라고 놓으면? 서버가 바뀌었을 때 아이피주소가 바뀔수 있다.
            //도메인을 등록. www.rookiss.com -> 123.123.123.12 이런식으로 하면 관리가 쉬워짐.
            string host = Dns.GetHostName();
            Console.WriteLine($"host:{host}");
            IPHostEntry ipHost = Dns.GetHostEntry(host); //네트워크 망 안의 dns서버가 알려줌?
            IPAddress address = ipHost.AddressList[0]; //식당 주소.
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //식당 정문? 문의 번호. 문지기 번호.

            listener.Init(endPoint, () => { return new GameSession(); });
            Console.WriteLine("Listening...");

            // 주 스레드는 여기가 실행되지만 리스너의 콜백함수는 별도의 스레드에서 실행됨.
            while(true)
            {
                
            }
        }
    }
}