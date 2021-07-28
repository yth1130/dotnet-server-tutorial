using System;
using System.Net;
using System.Text;
using System.Threading;
using ServerCore;

namespace Server
{
    // ushort는 2바이트. 0~64k
    // 패킷의 종류만 보고 크기를 파악하기 힘들기 때문에 첫 인자로 크기를 넣어준다. 자신의 크기를 포함할 지는 정해야 함.
    
    public abstract class Packet
    {
        public ushort size;
        public ushort packetId;

        public abstract ArraySegment<byte> Write();
        public abstract void Read(ArraySegment<byte> s);
    }

    // 클라에서 서버로 요구.
    class PlayerInfoReq : Packet
    {
        public long playerId;
        public string name; // 크기가 정해져있지 않음.

        public PlayerInfoReq()
        {
            packetId = (ushort)PacketID.PlayerInfoReq;
        }

        public override void Read(ArraySegment<byte> segment)
        {
            ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

            ushort count = 0;
            count += sizeof(ushort);
            count += sizeof(ushort);
            this.playerId = BitConverter.ToInt64(span.Slice(count, span.Length - count));
            count += sizeof(long);

            // string
            ushort nameLength = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
            count += sizeof(ushort);
            this.name = Encoding.Unicode.GetString(span.Slice(count, nameLength));
        }

        public override ArraySegment<byte> Write()
        {
            ArraySegment<byte> segment = SendBufferHelper.Open(4096);
            ushort count = 0;
            bool success = true;

            Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.packetId);
            count += sizeof(ushort);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.playerId);
            count += sizeof(long);
            success &= BitConverter.TryWriteBytes(segment, count);

            // string
            // c++에서는 NULL 0x00 00 으로 끝남.
            // c#은 아님.
            // string의 길이를 먼저 보내고 그다음 실제 그 크기만큼 byte[]로 보내준다.
            ushort nameLength = (ushort)Encoding.Unicode.GetByteCount(this.name);
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLength);
            count += sizeof(ushort);
            Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLength);
            count += nameLength;

            if (success == false)
                return null;
            
            return SendBufferHelper.Close(count);
        }
    }

    // // 서버에서 클라로 답변.
    // class PlayerInfoOk : Packet
    // {
    //     public int hp;
    //     public int attack;
    // }

    public enum PacketID
    {
        PlayerInfoReq = 1,
        PlayerInfoOk = 2,
    }

    // 클라이언트의 대리자.
    class ClientSession: PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnConnected: {endPoint}");

            // Packet packet = new Packet() { size = 100, packetId = 10 };

            // [ 100 ] [ 10 ]
            // byte[] sendBuff = new byte[4096];
            // byte[] buffer = BitConverter.GetBytes(knight.hp);
            // byte[] buffer2 = BitConverter.GetBytes(knight.attack);
            // Array.Copy(buffer, 0, sendBuff, 0, buffer.Length);
            // Array.Copy(buffer2, 0, sendBuff, buffer.Length, buffer2.Length);

            // ArraySegment<byte> openSegment = SendBufferHelper.Open(4096);
            // byte[] buffer = BitConverter.GetBytes(packet.size);
            // byte[] buffer2 = BitConverter.GetBytes(packet.packetId);
            // Array.Copy(buffer, 0, openSegment.Array, openSegment.Offset, buffer.Length);
            // Array.Copy(buffer2, 0, openSegment.Array, openSegment.Offset + buffer.Length, buffer2.Length);
            // ArraySegment<byte> sendBuff = SendBufferHelper.Close(buffer.Length + buffer2.Length);

            // byte[] sendBuff = Encoding.UTF8.GetBytes("Welcome to MMORPG Server!");
            // Send(sendBuff);

            Thread.Sleep(5000);

            Disconnect();
            // Disconnect();
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Console.WriteLine($"OnDisconnected: {endPoint}");
        }

        // 이중 패킷. ( (3,2) 좌표로 이동하고 싶다. )
        // 15(이동) 3 2(좌표)
        // public override int OnRecv(ArraySegment<byte> buffer)
        // {
        //     string recvData = Encoding.UTF8.GetString(buffer.Array, buffer.Offset, buffer.Count);
        //     Console.WriteLine($"[From Client] {recvData}");
        //     return buffer.Count;
        // }

        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            ushort count = 0;
            ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;
            ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset + count);
            count += 2;

            switch ((PacketID)id)
            {
                case PacketID.PlayerInfoReq:
                    PlayerInfoReq packet = new PlayerInfoReq();
                    packet.Read(buffer);
                    Console.WriteLine($"Player Info Req: {packet.playerId} {packet.name}");
                    break;
            }

            Console.WriteLine($"RecvPacketId:{id}/Size:{size}");
        }

        public override void OnSend(int numOfBytes)
        {
            System.Console.WriteLine($"[transferred bytes]: {numOfBytes}");
        }
    }

}