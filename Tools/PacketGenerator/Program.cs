using System;
using System.IO;
using System.Xml;

namespace PacketGenerator
{
    class Program
    {
        static string genPackets;

        static void Main(string[] args)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };

            using (XmlReader reader = XmlReader.Create("PDL.xml", settings)) // reader.Dispose(); 가 알아서 불린다.
            {
                reader.MoveToContent();

                while(reader.Read())
                {
                    if (reader.Depth == 1 && reader.NodeType == XmlNodeType.Element) // 패킷의 정보 시작.
                        ParsePacket(reader);
                    // System.Console.WriteLine(reader.Name+" "+reader["name"]);
                }

                File.WriteAllText("GenPackets.cs", genPackets);
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
            
            // Tuple은 여러개를 묶는 용도?
            Tuple<string, string, string> tuple = ParseMembers(reader);
            genPackets = string.Format(PacketFormat.packetFormat,
                packetName, tuple.Item1, tuple.Item2, tuple.Item3);
        }

        // {1} : 멤버 변수.
        // {2} : 멤버 변수 Read
        // {3} : 멤버 변수 Write
        public static Tuple<string, string, string> ParseMembers(XmlReader reader)
        {
            string packetName = reader["name"];

            string memberCode = "";
            string readCode = "";
            string writeCode = "";

            int depth = reader.Depth + 1;
            while (reader.Read())
            {
                if (reader.Depth != depth)
                    break;

                string memberName = reader["name"];
                if (string.IsNullOrEmpty(memberName))
                {
                    System.Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(memberCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    memberCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    memberCode += Environment.NewLine;

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
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, GetToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        memberCode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> tuple = ParseList(reader);
                        memberCode += tuple.Item1;
                        readCode += tuple.Item2;
                        writeCode += tuple.Item3;
                        break;
                    default:
                        break;
                }
            }

            memberCode = memberCode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\n\t\t");
            writeCode = writeCode.Replace("\n", "\n\t\t");
            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static Tuple<string, string, string> ParseList(XmlReader reader)
        {
            string listName = reader["name"];
            if (string.IsNullOrEmpty(listName))
            {
                System.Console.WriteLine("List without name");
                return null;
            }

            Tuple<string, string, string> tuple = ParseMembers(reader);
            string memberCode = string.Format(
                PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                tuple.Item1,
                tuple.Item2,
                tuple.Item3);
            
            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));
                
            string writeCode = string.Format(PacketFormat.writeListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }

        public static string GetToMemberType(string memberType)
        {
            switch (memberType)
            {
                case "bool":    return "ToBoolean";
                case "short":   return "ToInt16";
                case "ushort":  return "ToUInt16";
                case "int":     return "ToInt32";
                case "long":    return "ToInt64";
                case "float":   return "ToSingle";
                case "double":  return "ToDouble";
                default: return "";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToUpper() + input.Substring(1);
        }
        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";
            return input[0].ToString().ToLower() + input.Substring(1);
        }
    }
}
