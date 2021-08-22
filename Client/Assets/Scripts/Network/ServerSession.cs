using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ServerCore;
using UnityEngine;

namespace DummyClient
{
    // 서버의 대리자.
    class ServerSession: PacketSession
    {
        public override void OnConnected(EndPoint endPoint)
        {
            Debug.Log($"OnConnected: {endPoint}");
        }
        public override void OnDisconnected(EndPoint endPoint)
        {
            Debug.Log($"OnDisconnected: {endPoint}");
        }
        public override void OnRecvPacket(ArraySegment<byte> buffer)
        {
            PacketManager.Instance.OnRecvPacket(this, buffer);
        }
        public override void OnSend(int numOfBytes)
        {
            // System.Console.WriteLine($"[transferred bytes]: {numOfBytes}");
        }
    }
}