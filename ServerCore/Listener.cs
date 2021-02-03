using System;
using System.Net;
using System.Net.Sockets;

namespace ServerCore
{
    class Listener
    {
        Socket socket;
        Action<Socket> onAcceptHandler;

        public void Init(IPEndPoint endPoint, Action<Socket> onAcceptHandler)
        {
            socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Tcp 세팅.
            this.onAcceptHandler += onAcceptHandler;

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
            if (pending == false) //바로 완료가 됨.
                OnAcceptCompleted(null, args);
        }

        void OnAcceptCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.SocketError == SocketError.Success)
            {
                onAcceptHandler?.Invoke(args.AcceptSocket);
            }
            else
            {
                Console.WriteLine(args.SocketError.ToString());
            }

            // 다음 접속을 받기 위해 다시 등록.
            RegisterAccept(args);
        }

        public Socket Accept()
        {
            return socket.Accept();
        }
    }
}