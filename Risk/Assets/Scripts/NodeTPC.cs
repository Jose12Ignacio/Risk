using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class NodeTPC : Node
{
    public TcpClient Client;
    
    public NodeTPC(TcpClient client) : base()
    {
        Client = client;
    }
}
