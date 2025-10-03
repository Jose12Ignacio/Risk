using UnityEngine;
using System.Net.Sockets;

public class PlayerInfo
{
    public string username;
    public string color;
    public bool firstTurn;
    public TcpClient client;

    public PlayerInfo(TcpClient c, string name)
    {
        client = c;
        username = name;
        color = ""; // se asigna más tarde
        firstTurn = true;
    }
}
