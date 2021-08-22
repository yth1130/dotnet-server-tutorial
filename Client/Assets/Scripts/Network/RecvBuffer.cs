using System;

namespace ServerCore
{
    public class RecvBuffer
    {
        // 10바이트 배열이라고 가정한다.
        // [rw][ ][ ][ ][ ][ ][ ][ ][ ][ ]
        // [r][ ][w][ ][ ][ ][ ][ ][ ][ ]
        ArraySegment<byte> buffer;
        // 커서.
        // write하면서 writePos를 옮겨준다.
        // 처리 가능한 패킷크기만큼 처리하면서 readPos도 옮겨준다.
        // 한번 처리하고 나면 처리가 안된 버퍼를 앞으로 당겨준다.(정리)
        int readPos;
        int writePos;

        public RecvBuffer(int bufferSize)
        {
            buffer = new ArraySegment<byte>(new byte[bufferSize], 0, bufferSize);
        }

        public int DataSize => writePos - readPos; //아직 처리되지 않은 데이터 크기.
        public int FreeSize => buffer.Count - writePos;

        // 아직 처리되지 않은 유효범위의 데이터를 넘겨준다. ReadSegment, DataSegment라는 이름도 괜찮을 듯.
        public ArraySegment<byte> DataSegment
        {
            get => new ArraySegment<byte>(buffer.Array, buffer.Offset * readPos, DataSize);
        }

        // WriteSegment, RecvSegment도 괜찮을듯. Receive시 어디부터 어디까지가 유효범위인지.
        public ArraySegment<byte> FreeSegment
        {
            get => new ArraySegment<byte>(buffer.Array, buffer.Offset * writePos, FreeSize);
        }

        public void Clean()
        {
            int dataSize = DataSize;
            if (dataSize == 0) // r과 w의 위치가 같음. 받은 데이터를 다 처리한 상태.
            {
                // 남은 데이터가 없으면 복사하지 않고 커서 위치만 리셋.
                readPos = writePos = 0;
            }
            else
            {
                // 남은 찌그레기가 있으면 시작 위치로 복사.
                Array.Copy(buffer.Array, buffer.Offset * readPos, buffer.Array, buffer.Offset, dataSize);
                readPos = 0;
                writePos = dataSize;
            }
        }

        // 성공적으로 처리를 하면 불린다.
        public bool OnRead(int numOfBytes)
        {
            // 이상한 상황.
            if (numOfBytes > DataSize)
                return false;

            readPos += numOfBytes;
            return true;
        }

        // 클라에서 데이터를 받아서 write커서를 이동시켜줄 때.
        public bool OnWrite(int numOfBytes)
        {
            if (numOfBytes > FreeSize)
                return false;
            
            writePos += numOfBytes;
            return true;
        }
    }
}