#nullable enable
using System;
using UnityEngine;


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

    private static System.Random rnd = new System.Random();
    public Node? head;
    private int count = 0;
    public Node? currPlayer;

    public Node? currNode;


    public void Add(T value)
    {
        Debug.Log("anadiendo");
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

    public void nextNode()
    {
        if (currNode == null || currNode.next == null)
        {
            currNode = head;
        }
        currNode = currNode?.next;
    }

    public Node? RandomNode(int elements)
    {
        Node? curr = head;
        int i;
        if (curr == null)
        {
            return null;
        }
        i = rnd.Next(elements);

        for (int j = 0; j < i; j++)
        {
            if (curr == null)
            {
                curr = head;
            }
            else
            {
                curr = curr.next;
            }
        }
        if (curr == null)
        {
            curr = head;
        }
        return curr;
    }

    public void Clear()
    {
        head = null;
    }    
}