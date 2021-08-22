using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ServerCore
{
    public abstract class PacketSession : Session
    {
        public static readonly int HeaderSize = 2;
        // [size(2)][packetId(2)][ ... ][size(2)][packetId(2)][ ... ]...
        public sealed override int OnRecv(ArraySegment<byte> buffer) // sealed를 붙이면 자식 클래스에서 override 못함.
        {
            int processLength = 0;
            int packetCount = 0;
            
            while (true)
            {
                // 최소한 헤더는 파싱할 수 있는지 확인.
                if (buffer.Count < HeaderSize)
                    break;

                // 패킷이 완전체로 도착했는지 확인.
                ushort dataSize = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
                if (buffer.Count < dataSize)
                    break;

                // 여기까지 오면 패킷 조립 가능. 해당 패킷을 넘겨준다.
                OnRecvPacket(new ArraySegment<byte>(buffer.Array, buffer.Offset, dataSize));
                packetCount++;

                processLength += dataSize;
                buffer = new ArraySegment<byte>(buffer.Array, buffer.Offset + dataSize, buffer.Count - dataSize);
            }
            if (packetCount > 1)
                System.Console.WriteLine($"패킷 모아보내기 : {packetCount}");
            
            return processLength;
        }

        public abstract void OnRecvPacket(ArraySegment<byte> buffer);
    }
    public abstract class Session
    {
        // class SessionHandler
        // {

        // }

        Socket socket;
        int disconnected = 0;

        RecvBuffer recvBuffer = new RecvBuffer(65535);

        List<ArraySegment<byte>> pendingList = new List<ArraySegment<byte>>();
        SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
        SocketAsyncEventArgs recvArgs = new SocketAsyncEventArgs();

        public abstract void OnConnected(EndPoint endPoint);
        public abstract int OnRecv(ArraySegment<byte> buffer); // 처리된 데이터 크기를 반환.
        public abstract void OnSend(int numOfBytes);
        public abstract void OnDisconnected(EndPoint endPoint);

        void Clear()
        {
            lock(_lock)
            {
                sendQueue.Clear();
                pendingList.Clear();
            }
        }

        object _lock = new object();
        Queue<ArraySegment<byte>> sendQueue = new Queue<ArraySegment<byte>>();
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

        public void Send(List<ArraySegment<byte>> sendBuffList)
        {
            if (sendBuffList.Count == 0)
                return;

            lock (_lock) //한 번에 하나만.
            {
                foreach (var sendBuff in sendBuffList)
                {
                    sendQueue.Enqueue(sendBuff);
                }

                // if (_pending == false) //전송 예약중이 아님.
                if (pendingList.Count == 0) //전송 예약중이 아님.
                {
                    RegisterSend();
                }
            }
        }
        public void Send(ArraySegment<byte> sendBuff)
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

            Clear();
        }

#region 네트워크 통신.
        void RegisterSend()
        {
            if (disconnected == 1)
                return;

            // _pending = true;
            // byte[] buff = sendQueue.Dequeue();
            // sendArgs.SetBuffer(buff, 0, buff.Length);

            //bufferList 버전.
            // List<ArraySegment<byte>> list = new List<ArraySegment<byte>>();
            while(sendQueue.Count > 0)
            {
                ArraySegment<byte> buff = sendQueue.Dequeue();
                pendingList.Add(buff);
            }
            sendArgs.BufferList = pendingList;


            try
            {
                bool pending = socket.SendAsync(sendArgs);
                if (pending == false)
                    OnSendCompleted(null, sendArgs);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"RegisterSend Failed : {e}");
            }
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
                // else
                // {
                //     Disconnect();
                // }
            }
        }

        void RegisterRecv()
        {
            if (disconnected == 1)
                return;

            // 유효 범위를 찝어줌..?
            recvBuffer.Clean();
            ArraySegment<byte> segment = recvBuffer.FreeSegment;
            recvArgs.SetBuffer(segment.Array, segment.Offset, segment.Count);

            try
            {
                bool pending = socket.ReceiveAsync(recvArgs);
                if (pending == false)
                    OnRecvCompleted(null, recvArgs);
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"RegisterRecv Failed : {e}");
            }
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