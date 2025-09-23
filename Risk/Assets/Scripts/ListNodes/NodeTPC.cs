using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class NodeTPC : Node
{
    public TcpClient Client;

    public string color;
    
    public NodeTPC(TcpClient client) : base()
    {
        Client = client;
    }
}
