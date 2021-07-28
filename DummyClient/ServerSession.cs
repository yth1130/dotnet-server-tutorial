using System;
using System.Net;
using System.Text;
using ServerCore;

namespace DummyClient
{
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

            // string
            // c++에서는 NULL 0x00 00 으로 끝남.
            // c#은 아님.
            // string의 길이를 먼저 보내고 그다음 실제 그 크기만큼 byte[]로 보내준다.
            
            // ushort nameLength = (ushort)Encoding.Unicode.GetByteCount(this.name);
            // success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLength);
            // count += sizeof(ushort);
            // Array.Copy(Encoding.Unicode.GetBytes(this.name), 0, segment.Array, count, nameLength);
            // count += nameLength;

            ushort nameLength = (ushort)Encoding.Unicode.GetBytes(this.name, 0, this.name.Length, segment.Array, segment.Offset + count + sizeof(ushort)); // nameLength가 들어갈 공간만큼 뒤로 밀어준다.
            success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), nameLength);
            count += sizeof(ushort);
            count += nameLength;

            success &= BitConverter.TryWriteBytes(segment, count);

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

            PlayerInfoReq packet = new PlayerInfoReq() { playerId = 1001, name = "ABCD" };

            // 보낸다.
            {
                ArraySegment<byte> s = packet.Write();
                if (s != null)
                    Send(s);
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