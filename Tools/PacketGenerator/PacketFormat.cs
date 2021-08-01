namespace PacketGenerator
{
    class PacketFormat
    {
        // {0} : 패킷 이름.
        // {1} : 멤버 변수.
        // {2} : 멤버 변수 Read
        // {3} : 멤버 변수 Write
        public static string packetFormat = 
@"
class {0}
{{
    {1}

    public void Read(ArraySegment<byte> segment)
    {{
        ReadOnlySpan<byte> span = new ReadOnlySpan<byte>(segment.Array, segment.Offset, segment.Count);

        ushort count = 0;
        count += sizeof(ushort);
        count += sizeof(ushort);
        
        {2}
    }}

    public ArraySegment<byte> Write()
    {{
        ArraySegment<byte> segment = SendBufferHelper.Open(4096);
        ushort count = 0;
        bool success = true;

        Span<byte> span = new Span<byte>(segment.Array, segment.Offset, segment.Count);

        count += sizeof(ushort);
        success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), (ushort)PacketID.{0});
        count += sizeof(ushort);
        {3}
        success &= BitConverter.TryWriteBytes(segment, count);

        if (success == false)
            return null;
        
        return SendBufferHelper.Close(count);
    }}
}}
";

        // {0} 변수 형식.
        // {1} 변수 이름.
        public static string memberFormat = 
@"public {0} {1}";

        // {0} 변수 이름.
        // {1} To~ 변수형식.
        // {2} 변수 형식
        public static string readFormat =
@"this.{0} = BitConverter.{1}(span.Slice(count, span.Length - count));
count += sizeof({2});
";
        // {0} 변수 이름.
        public static string readStringFormat =
@"ushort {0}Length = BitConverter.ToUInt16(span.Slice(count, span.Length - count));
count += sizeof(ushort);
this.{0} = Encoding.Unicode.GetString(span.Slice(count, {0}Length));
count += {0}Length;
";

        // {0} 변수 이름.
        // {1} 변수 형식.
        public static string writeFormat =
@"success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), this.{0});
count += sizeof({1});
";
        // {0} 변수 이름.
        public static string writeStringFormat =
@"ushort {0}Length = (ushort)Encoding.Unicode.GetBytes(this.{0}, 0, this.{0}.Length, segment.Array, segment.Offset + count + sizeof(ushort)); // nameLength가 들어갈 공간만큼 뒤로 밀어준다.
success &= BitConverter.TryWriteBytes(span.Slice(count, span.Length - count), {0}Length);
count += sizeof(ushort);
count += {0}Length;
";
    }
}