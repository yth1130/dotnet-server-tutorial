dotnet run --project ./Tools/PacketGenerator ./Tools/PacketGenerator/PDL.xml
cp ./GenPackets.cs ./DummyClient/Packet
cp ./GenPackets.cs ./Server/Packet
cp ./GenPackets.cs ./Client/Assets/Scripts/Packet
cp ./ClientPacketManager.cs ./DummyClient/Packet
cp ./ClientPacketManager.cs ./Client/Assets/Scripts/Packet
cp ./ServerPacketManager.cs ./Server/Packet
