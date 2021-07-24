using System;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
    class Packet
    {
        public ushort size;
        public ushort packetId;
    }

    // 클라에서 서버로 요구.
    class PlayerInfoReq : Packet
    {
        public long playerId;
    }

    // 서버에서 클라로 답변.
    class PlayerInfoOk : Packet
    {
        public int hp;
        public int attack;
    }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }
    
    // 서버의 대리자.
    class ServerSession: Session
    {
        // static unsafe void ToBytes(byte[] array, int offset, ulong value)
        // {
        //     fixed (byte* ptr = &array[offset])
        //         *(ulong*)ptr = value;
        // }

        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            PlayerInfoReq packet = new PlayerInfoReq() { size = 4, packetId = (ushort)PacketID.PlayerInfoReq, playerId = 1001};

            // 보낸다.
            // for (int i = 0; i < 5; i++)
            {
                ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
                ushort count = 0;
                bool success = true;

                // // 안정적이지만 느리다.
                // byte[] size = BitConverter.GetBytes(packet.size);
                // byte[] packetId = BitConverter.GetBytes(packet.packetId);
                // byte[] playerId = BitConverter.GetBytes(packet.playerId);

                // Array.Copy(size, 0, openSegment.Array, openSegment.Offset + count, 2);
                // count += 2;
                // Array.Copy(packetId, 0, openSegment.Array, openSegment.Offset + count, 2);
                // count += 2;
                // Array.Copy(playerId, 0, openSegment.Array, openSegment.Offset + count, 8);
                // count += 8;

                // 빠른 버전.
                // success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), packet.size);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), packet.packetId);
                count += 2;
                success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset + count, openSegment.Count - count), packet.playerId);
                count += 8;

                // 전체 크기를 마지막에 맨 앞에다 넣어준다.
                success &= BitConverter.TryWriteBytes(new Span<byte>(openSegment.Array, openSegment.Offset, openSegment.Count), count);
                
                ArraySegment<byte> sendBuff = SendBufferHelper.Close(count);

                // byte[] sendBuff = Encoding.UTF8.GetBytes($"Hello World! {i}");
                if (success)
                    Send(sendBuff);
            }
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }
        public override int OnRecv(ArraySegment<byte> buffer)
        {
            string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
            Console.WriteLine($"[From Server] {recvData}");
            return buffer.Count;
        }
        public override void OnSend(int numOfBytes)
        {
            System.Console.WriteLine($"[transferred bytes]: {numOfBytes}");
        }
    }
}