using System;
using CrazyRisk.Core;
using UnityEngine;

[Serializable]
public class TurnInfo
{
    // === Datos básicos del turno ===
    public string color;
    public string actionType;
    public string fromTerritory;
    public string toTerritory;
    public int troops;
    public bool startGame;
    public int numPlayers;
    public string playerName;
    public string ipCode;
    public bool gameRoom = false;

    // === Estructuras personalizadas ===
    public LinkedList<PlayerInfo> playersList;
    public LinkedList<Territorio> territoriesList;

    // === Copias serializables (para JSON) ===
    public PlayerInfo[] playersArray;
    public Territorio[] territoriesArray;

    // === Convertir antes de enviar ===
    public void PrepareForSend()
    {
        if (playersList != null)
            playersArray = LinkedListToArray(playersList);

        if (territoriesList != null)
            territoriesArray = LinkedListToArray(territoriesList);
    }

    // === Reconstruir al recibir ===
    public void RebuildLinkedLists()
    {
        if (playersArray != null && playersArray.Length > 0)
            playersList = ArrayToLinkedList(playersArray);

        if (territoriesArray != null && territoriesArray.Length > 0)
            territoriesList = ArrayToLinkedList(territoriesArray);
    }

    // === Utilidades ===
    private static T[] LinkedListToArray<T>(LinkedList<T> list)
    {
        int count = list.Count();
        T[] arr = new T[count];
        for (int i = 0; i < count; i++)
            arr[i] = list.Get(i);
        return arr;
    }

    private static LinkedList<T> ArrayToLinkedList<T>(T[] arr)
    {
        var l = new LinkedList<T>();
        for (int i = 0; i < arr.Length; i++)
            l.Add(arr[i]);
        return l;
    }
}
