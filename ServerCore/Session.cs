using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    class Session
    {
        Socket socket;
        int disconnected = 0;
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();

        object _lock = new object();
        Queue<byte[]> sendQueue = new Queue<byte[]>();
        bool _pending = false;

        public void Start(Socket socket)
        {
            this.socket = socket;
            
            SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();
            recvArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // args.UserToken = this; //원하는 값을 넣을 수 있다.?
            recvArgs.SetBuffer(new byte[1024], 0, 1024);

            sendArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv(recvArgs);
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock) //한 번에 하나만.
            {
                sendQueue.Enqueue(sendBuff);
                if (_pending == false) //전송 예약중이 아님.
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1) //Disconnect 두번 불렸을 때 에러 방지.
                return; //이미 다른코드가 1로 세팅함.
            
            socket.Shutdown(SocketShutdown.Both); //듣기도 싫고 말하기도 싫다.
            socket.Close(); //연결 끊기.
        }

#region 네트워크 통신.
        void RegisterSend()
        {
            _pending = true;
            byte[] buff = sendQueue.Dequeue();
            sendArgs.SetBuffer(buff, 0, buff.Length);

            bool pending = socket.SendAsync(sendArgs);
            if (pending == false)
                OnSendCompleted(null, sendArgs);
        }

        void OnSendCompleted(object sender, SocketAsyncEventArgs args)
        {
            lock (_lock)
            {
                if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
                {
                    try
                    {
                        if (sendQueue.Count > 0)
                            RegisterSend();
                        else
                            _pending = false;
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine($"OnSendCompleted failed {e.ToString()}");
                    }
                }
                else
                {
                    Disconnect();
                }
            }
        }

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
                Disconnect();
            }

        }
#endregion

    }
}