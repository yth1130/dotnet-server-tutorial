using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket socket;
        Func<Session> sessionFactory;

        public void Init(IPEndPoint endPoint, Func<Session> sessionFactory)
        {
            socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Tcp 세팅.
            this.sessionFactory += sessionFactory;

            //문지기 교육.
            socket.Bind(endPoint);

            //영업 시작.
            socket.Listen(10); //backlog: 최대 대기수. 라이브일 때 조절해야함.

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            RegisterAccept(args);
        }

        void RegisterAccept(SocketAsyncEventArgs args)
        {
            //args를 재사용하기 때문에 초기화를 해줘야 한다.
            args.AcceptSocket = null;

            bool pending = socket.AcceptAsync(args);
            if (pending == false) //바로 완료가 됨. 최대 대기수가 있기 때문에 계속 false가 되진 않음. -> 스택오버플로x
                OnAcceptCompleted(null, args);
        }

        //별도의 스레드에서 실행된다.
        //다른 스레드와 같은 데이터에 접근하면 race condition이 일어난다.
        //danger zone
        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                Session session = sessionFactory.Invoke();
                session.Start(args.AcceptSocket);
                session.OnConnected(args.AcceptSocket.RemoteEndPoint);
                // onAcceptHandler?.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            // 다음 접속을 받기 위해 다시 등록.
            RegisterAccept(args);
        }

    }
}