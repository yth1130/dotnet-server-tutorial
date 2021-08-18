using System;
using System.Threading;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => null);
        public static int ChunkSize { get; set; } = 65535 * 100;

        public static ArraySegment<byte> Open(int reserveSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);
            
            if (CurrentBuffer.Value.FreeSize < reserveSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserveSize);
        }
        public static ArraySegment<byte> Close(int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
    }
    public class SendBuffer
    {
        // [w][][][][][][][][][] 10바이트짜리.
        byte[] buffer;
        int writePos = 0;

        public int FreeSize => buffer.Length - writePos;

        // chunk는 뭉탱이 같은 느낌. 해당 사이즈가 꽤 큰 것을 의미.
        public SendBuffer(int chunkSize)
        {
            buffer = new byte[chunkSize]; 
        }

        // 사용할 범위를 반환(?)
        public ArraySegment<byte> Open(int reserveSize)
        { 
            if (reserveSize > FreeSize)
                return null;

            return new ArraySegment<byte>(buffer, writePos, reserveSize);
        }
        // 사용한 범위를 반환해준다.
        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(buffer, writePos, usedSize);
            writePos += usedSize;
            return segment;
        }

    }
}