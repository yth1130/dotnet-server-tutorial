using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCore
{
    class Program
    {
        static Listener listener = new Listener();

        static void OnAcceptHandler(Socket clientSocket)
        {
            try
            {
                //받는다.
                byte[] recvBuff = new byte[1024];
                int recvBytes = clientSocket.Receive(recvBuff);
                string recvData = Encoding.UTF8.GetString(recvBuff, 0, recvBytes);
                Console.WriteLine($"[From Client] {recvData}");

                //보낸다.
                byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
                clientSocket.Send(sendBuff);

                //쫓아낸다.
                clientSocket.Shutdown(SocketShutdown.Both); //듣기도 싫고 말하기도 싫다.
                clientSocket.Close(); //연결 끊기.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        
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

            listener.Init(endPoint, OnAcceptHandler);
            Console.WriteLine("Listening...");

            while(true)
            {
                
            }
        }
    }
}
