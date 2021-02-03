using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

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

            while(true)
            {
                // 휴대폰 설정.
                Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // 입장 문의.
                    socket.Connect(endPoint);
                    Console.WriteLine($"Connected To {socket.RemoteEndPoint.ToString()}");

                    // 보낸다.
                    byte[] sendBuff = Encoding.UTF8.GetBytes("Hello World!");
                    int sendBytes = socket.Send(sendBuff);

                    // 받는다.
                    byte[] recvBuff = new byte[1024];
                    int recvBytes = socket.Receive(recvBuff);
                    string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                    Console.WriteLine($"[From Server] {recvData}");

                    // 나간다.
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                Thread.Sleep(100);
            }

        }
    }
}
