dotnet run --project ./Tools/PacketGenerator ./Tools/PacketGenerator/PDL.xml
cp ./GenPackets.cs ./DummyClient/Packet
cp ./GenPackets.cs ./Server/Packet
cp ./ClientPacketManager.cs ./DummyClient/Packet
cp ./ServerPacketManager.cs ./Server/Packet