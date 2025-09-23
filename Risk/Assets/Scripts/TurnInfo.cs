using System;
using UnityEngine;

[Serializable]
public class TurnInfo //La finalidad de la clase es crear una base para la información que se comparte a las otras computadoras.
{
    //Esta información se convierte a Json y se envia al socket, luego la otra computadora lo interpreta
    public string color;
    public string actionType;
    public string fromTerritory;
    public string toTerritory;
    public int troops;

    public bool startGame;

    public int numPlayers;      
}