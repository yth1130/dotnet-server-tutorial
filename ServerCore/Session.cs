using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket socket;
        int disconnected = 0;

        public void Start(Socket socket)
        {
            this.socket = socket;
            
            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // args.UserToken = this; //원하는 값을 넣을 수 있다.?
            args.SetBuffer(new byte[1024], 0, 1024);

            RegisterRecv(args);
        }

        public void Send(byte[] sendBuff)
        {
            socket.Send(sendBuff);
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1) //Disconnect 두번 불렸을 때 에러 방지.
                return; //이미 다른코드가 1로 세팅함.
            
            socket.Shutdown(SocketShutdown.Both); //듣기도 싫고 말하기도 싫다.
            socket.Close(); //연결 끊기.
        }

#region 네트워크 통신.
        void RegisterRecv(SocketAsyncEventArgs args)
        {
            bool pending = socket.ReceiveAsync(args);
            if (pending == false)
                OnRecvCompleted(null, args);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    string recvData = Encoding.UTF8.GetString(args.Buffer, args.Offset, args.BytesTransferred);
                    //TODO
                    Console.WriteLine($"[From Client] {recvData}");

                    RegisterRecv(args);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"OnRecvCompeted failed {e.ToString()}");
                }
            }
            else
            {
                //TODO Disconnect
            }

        }
#endregion

    }
}