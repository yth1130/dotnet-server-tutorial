using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class Session
    {
        // class SessionHandler
        // {

        // }

        Socket socket;
        int disconnected = 0;

        RecvBuffer recvBuffer = new RecvBuffer(1024);


        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer); // 처리된 데이터 크기를 반환.
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        object _lock = new object();
        Queue<byte[]> sendQueue = new Queue<byte[]>();
        // bool _pending = false;

        public void Start(Socket socket)
        {
            this.socket = socket;
            
            recvArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnRecvCompleted);
            // args.UserToken = this; //원하는 값을 넣을 수 있다.?
            // recvArgs.SetBuffer(new byte[1024], 0, 1024);

            sendArgs.Completed += new System.EventHandler<SocketAsyncEventArgs>(OnSendCompleted);

            RegisterRecv();
        }

        public void Send(byte[] sendBuff)
        {
            lock (_lock) //한 번에 하나만.
            {
                sendQueue.Enqueue(sendBuff);
                // if (_pending == false) //전송 예약중이 아님.
                if (pendingList.Count == 0) //전송 예약중이 아님.
                {
                    RegisterSend();
                }
            }
        }

        public void Disconnect()
        {
            if (Interlocked.Exchange(ref disconnected, 1) == 1) //Disconnect 두번 불렸을 때 에러 방지.
                return; //이미 다른코드가 1로 세팅함.
            
            OnDisconnected(socket.RemoteEndPoint);
            socket.Shutdown(SocketShutdown.Both); //듣기도 싫고 말하기도 싫다.
            socket.Close(); //연결 끊기.
        }

#region 네트워크 통신.
        void RegisterSend()
        {
            // _pending = true;
            // byte[] buff = sendQueue.Dequeue();
            // sendArgs.SetBuffer(buff, 0, buff.Length);

            //bufferList 버전.
            // List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
            while(sendQueue.Count > 0)
            {
                byte[] buff = sendQueue.Dequeue();
                pendingList.Add(new ArraySegment<byte>(buff, 0, buff.Length));
            }
            sendArgs.BufferList = pendingList;

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
                        sendArgs.BufferList = null;
                        pendingList.Clear();

                        OnSend(sendArgs.BytesTransferred);

                        if (sendQueue.Count > 0)
                            RegisterSend();
                        // else
                        //     _pending = false;
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

        void RegisterRecv()
        {
            // 유효 범위를 찝어줌..?
            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.FreeSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            bool pending = socket.ReceiveAsync(recvArgs);
            if (pending == false)
                OnRecvCompleted(null, recvArgs);
        }

        void OnRecvCompleted(object sender, SocketAsyncEventArgs args)
        {
            if (args.BytesTransferred > 0 && args.SocketError == SocketError.Success)
            {
                try
                {
                    // write커서 이동.
                    if (recvBuffer.OnWrite(args.BytesTransferred) == false)
                    {
                        Disconnect();
                        return;
                    }

                    // OnRecv(new ArraySegment<byte>(args.Buffer, args.Offset, args.BytesTransferred));
                    // 컨텐츠 쪽으로 데이터를 넘겨주고 얼마나 처리했는지 받는다.
                    int processLen = OnRecv(recvBuffer.DataSegment);
                    if (processLen < 0 || recvBuffer.DataSize < processLen)
                    {
                        Disconnect();
                        return;
                    }

                    // Read커서 이동.
                    if (recvBuffer.OnRead(processLen) == false)
                    {
                        Disconnect();
                        return;
                    }


                    RegisterRecv();
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