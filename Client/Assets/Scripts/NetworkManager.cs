using System.Collections;
using System.Collections.Generic;
using System.Net;
using DummyClient;
using ServerCore;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    ServerSession session = new ServerSession();

    void Start()
    {
        string host = Dns.GetHostName();
        print(host);
        IPHostEntry ipHost = Dns.GetHostEntry(host); //네트워크 망 안의 dns서버가 알려줌?
        // System.Console.WriteLine("--------------");
        // foreach (var ad in ipHost.AddressList)
        // {
        //     System.Console.WriteLine(ad.ToString());
        // }
        // System.Console.WriteLine("--------------");
        // IPAddress address = ipHost.AddressList[0]; //식당 주소.
        IPAddress address = IPAddress.Loopback;
        print(address.ToString());
        IPEndPoint endPoint = new IPEndPoint(address, 7777); //식당 정문? 문의 번호. 문지기 번호.
        // print(endPoint.ToString());

        Connector connector = new Connector();

        connector.Connect(endPoint, () => session, count: 1);    
    }

    void Update()
    {
        
    }
}
