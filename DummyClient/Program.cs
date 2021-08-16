using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServerCore;

namespace DummyClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string host = Dns.GetHostName();
            Console.WriteLine($"host:{host}");
            IPHostEntry ipHost = Dns.GetHostEntry(host); //네트워크 망 안의 dns서버가 알려줌?
            IPAddress address = ipHost.AddressList[0]; //식당 주소.
            IPEndPoint endPoint = new IPEndPoint(address, 7777); //식당 정문? 문의 번호. 문지기 번호.

            Connector connector = new Connector();

            connector.Connect(endPoint, () => SessionManager.Instance.Generate(), count: 10);

            while(true)
            {
                try
                {
                    SessionManager.Instance.SendForEach();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(250); //mmorpg에서 일반적으로 이동 패킷을 1초에 네번정도 보냄.
            }

        }
    }
}
