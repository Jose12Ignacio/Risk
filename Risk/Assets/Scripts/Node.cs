using UnityEngine;
using System.Net.Sockets;

public class Node
{
    public Node next;
    public TcpClient client;

    public Node()
    {
        next = null;
    }
}
