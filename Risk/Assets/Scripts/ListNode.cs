using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ListNodeTPC
{
    public TcpClient Client;
    public ListNodeTPC Next;
    
    public ListNodeTPC(TcpClient client)
    {
        Client = client;
        Next = null;
    }
    public static int Count(ListNodeTPC head)
    {
        if (head == null) return 0;

        int i = 0;

        while (head != null)
        {
            i++;
            head = head.Next;
        }
        return i;
    }

    public static void addLast(ListNodeTPC head, ListNodeTPC newNode)
    {

        while (head.Next != null)
        {
            head = head.Next;
        }
        head.Next = newNode;
    }
}
