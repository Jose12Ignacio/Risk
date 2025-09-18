using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

public class ListNode
{
    public Node head;

    public ListNode()
    {
        head = null;
    }
    public int Count()
    {
        if (head == null) return 0;

        int i = 0;

        while (head != null)
        {
            i++;
            head = head.next;
        }
        return i;
    }

    public void addLast(Node newNode)
    {

        while (head.next != null)
        {
            head = head.next;
        }
        head.next = newNode;
    }

    public void remove(TcpClient client)
    {
        Node prev = null;

        while (head != null)
        {
            if (head.client == client)
            {
                if (prev != null)
                {
                    prev.next = head.next;
                    head.next = null;
                }
            }
            if (prev == null)
            {
                prev = head;
            }
            else
            {
                prev = prev.next;
            }
            head = head.next;

        }
    }
}
