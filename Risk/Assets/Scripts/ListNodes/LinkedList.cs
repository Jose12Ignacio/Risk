#nullable enable
using System;


public class LinkedList<T>
{
    public class Node
    {
        public T data;
        public Node? next;

        public Node(T data)
        {
            this.data = data;
            next = null;
        }
    }

    public Node? head;
    private int count = 0;
    public Node? currPlayer;


    public void Add(T value)
    {
        Node newNode = new Node(value);
        if (head == null)
        {
            head = newNode;
        }
        else
        {
            Node curr = head;
            while (curr.next != null) curr = curr.next;
            curr.next = newNode;
        }
        count++;
    }

    public int Count() => count;

    public T Get(int index)
    {
        if (index < 0 || index >= count)
            throw new ArgumentOutOfRangeException(nameof(index));

        Node? curr = head;
        for (int i = 0; i < index; i++)
            curr = curr!.next;

        return curr!.data;
    }

    public bool Contains(T value)
    {
        Node? curr = head;
        while (curr != null)
        {
            if (curr.data!.Equals(value)) return true;
            curr = curr.next;
        }
        return false;
    }

    public void Remove(T data)
    {
        Node? prev = null;
        Node? curr = head;

        while (curr != null)
        {
            if ((curr.data == null && data == null) || (curr.data != null && curr.data.Equals(data)))
            {
                if (prev != null)
                    prev.next = curr.next;
                else
                    head = curr.next;

                curr.next = null; // opcional
                Count();
                return;
            }

            prev = curr;
            curr = curr.next;
        }
    }
    public void nextPlayer()
    {
        if (currPlayer == null || currPlayer.next == null)
        {
            currPlayer = head;
        }
        currPlayer = currPlayer?.next;
    }
}