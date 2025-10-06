using UnityEngine;
using System.Net.Sockets;
using CrazyRisk.Core;
using CrazyRisk;

public class PlayerInfo
{
    public string username;
    public string color;
    public bool bot;
    public TcpClient client;

    public Ejercito ejercitoPlayer;

    public LinkedList<Territorio> myTerritories;

    public bool firstTurn;

    public PlayerInfo(TcpClient c, string name)
    {
        myTerritories = new LinkedList<Territorio>();
        client = c;
        username = name;
        color = ""; // se asigna más tarde
        firstTurn = true;
    }

    public int ContinentsDomained()
    {
        int num = 0;

        int AmericaNorte = 0;
        int AmericaSur = 0;
        int Europa = 0;
        int Asia = 0;
        int Oceania = 0;
        int Africa = 0;

        int totalAmericaNorte = 9;
        int totalAmericaSur   = 4;
        int totalEuropa       = 7;
        int totalAsia         = 12;
        int totalOceania      = 4;
        int totalAfrica       = 6;

        var current = myTerritories.head;
        while (current != null)
        {
            Territorio t = current.data;
            switch (t.Continente)
            {
                case Continente.AmericaNorte: AmericaNorte++; break;
                case Continente.AmericaSur:   AmericaSur++; break;
                case Continente.Europa:       Europa++; break;
                case Continente.Asia:         Asia++; break;
                case Continente.Oceania:      Oceania++; break;
                case Continente.Africa:       Africa++; break;
            }
            current = current.next;
        }

        if (AmericaNorte == totalAmericaNorte) num++;
        if (AmericaSur   == totalAmericaSur)   num++;
        if (Europa       == totalEuropa)       num++;
        if (Asia         == totalAsia)         num++;
        if (Oceania      == totalOceania)      num++;
        if (Africa       == totalAfrica)       num++;

        return num;
    }
}
