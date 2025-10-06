using System;
using UnityEngine;
using System.Net.Sockets;
using CrazyRisk.Core;

[Serializable]
public class TurnInfo //La finalidad de la clase es crear una base para la información que se comparte a las otras computadoras.
{
    //Esta información se convierte a Json y se envia al socket, luego la otra computadora lo interpreta
    public string color;
    public int troops;
    public bool startGame;
    public int numPlayers;
    public string playerName;
    public string ipCode;
    public bool gameRoom = false;

    public LinkedList<PlayerInfo> playersList = new LinkedList<PlayerInfo>();
    public LinkedList<Territorio> territoriesList = new LinkedList<Territorio>();

    public bool setTropsFase;

    public bool normalGame;
}