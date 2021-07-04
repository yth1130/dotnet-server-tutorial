using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{    
    // 서버에 연결하는 놈.
    // 리시브, 센드를 공용으로 사용하기 위해.
    // 서버가 하는 일에 따라 여러가지로 나눠 만들어 분산 처리 될 수 있다. 몬스터 관리, 필드관리 npc관리 등으로.
    // 메인 서버가 다른 서버와 통신하기 위해 연결이 필요.
    public class Connector
    {
       Func<Session> sessionFactory;

        public void Connect(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            // 휴대폰 설정.
            Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this.sessionFactory = sessionFactory;

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += OnConnectCompleted;
            args.RemoteEndPoint = endPoint;
            args.UserToken = socket;

            RegisterConnect(args);
        }

        void RegisterConnect(SocketAsyncEventArgs args)
        {
            Socket socket = args.UserToken as Socket;
            if (socket == null)
                return;
            bool pending = socket.ConnectAsync(args);
            if (pending == false)
                OnConnectCompleted(null, args);
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = sessionFactory.Invoke();
                session.Start(args.ConnectSocket);
                session.OnConnected(args.RemoteEndPoint);
            }
            else
            {
                Console.WriteLine($"OnConnectCompleted Fail: {args.SocketError}");
            }

        }
    }
}