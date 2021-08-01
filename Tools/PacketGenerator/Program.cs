using System;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using(XmlReader reader = XmlReader.Create("PDL.xml", settings)) // reader.Dispose(); 가 알아서 불린다.
            {
                reader.MoveToContent();

                while(reader.Read())
                {
                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element) // 패킷의 정보 시작.
                        ParsePacket(reader);
                    // System.Console.WriteLine(reader.Name+" "+reader["name"]);
                }


            }
            
        }

        public static void ParsePacket(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.EndElement)
                return;
            if (reader.Name.ToLower() != "packet")
            {
                System.Console.WriteLine("Invalid packet node");
                return;
            }
            string packetName = reader["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                System.Console.WriteLine("Packet without name");
                return;
            }
            
            ParseMembers(reader);
        }
        public static void ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            int depth = reader.Depth + 1;
            while (reader.Read())
            {
                if (reader.Depth != depth)
                    break;

                string memberName = reader["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    System.Console.WriteLine("Member without name");
                    return;
                }
                string memberType = reader.Name.ToLower();
                switch (memberType)
                {
                    case "bool":
                    case "byte":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                    case "string":
                    case "list":
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
