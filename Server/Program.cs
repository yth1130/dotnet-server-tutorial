using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    class GameSession: Session
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            Send(sendBuff);

            Thread.Sleep(1000);

            Disconnect();
            // Disconnect();
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
        public override void OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Client] {recvData}");
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