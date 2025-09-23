using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using JetBrains.Annotations;

public class ListNode
{
    public Node head;

    public Node currPlayer;

    public ListNode()
    {
        head = null;
        currPlayer = null;
    }
    public int Count()
    {

        Node curr = head;

        if (curr == null) return 0;

        int i = 0;

        while (curr != null)
        {
            i++;
            curr = curr.next;
        }
        return i;
    }

    public void addLast(Node newNode)
    {
        if (head == null)
        {
            head = newNode;
            return;
        }

        Node curr = head;

        while (curr.next != null)
        {
            curr = curr.next;
        }
        curr.next = newNode;
    }

    public void remove(TcpClient client)
    {
        Node prev = null;
        Node curr = head;

        while (curr != null)
        {
            if (curr.client == client)
            {
                if (prev != null)
                {
                    prev.next = curr.next;
                }
                else
                {

                    head = curr.next;
                }
                curr.next = null;
                return;
            }

            prev = curr;
            curr = curr.next;
        }
    }

    public void clear()
    {
        head = null;
    }
    public void NextPlayer()
    {
        if (currPlayer == null || currPlayer.next == null)
        {
            currPlayer = head;
        }
        else
        {
            currPlayer = currPlayer.next;
        }
    }
}
