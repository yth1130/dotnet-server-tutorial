using System;
using ServerCore;

class PacketHandler
{
    public static void C_PlayerInfoReqHandler(PacketSession session, IPacket packet)
    {
        C_PlayerInfoReq p = packet as C_PlayerInfoReq;
        
        Console.WriteLine($"Player Info Req: {p.playerId} {p.name}");

        foreach (var skill in p.skills)
        {
            System.Console.WriteLine($"skill({skill.id})({skill.level})({skill.duration})");
        }
    }
}