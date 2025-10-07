using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using CrazyRisk;
using CrazyRisk.Core;

[Serializable]
public class TurnInfo
{
    // ===============================
    // 🔹 Datos principales del turno
    // ===============================
    public string actionType = "none";
    public bool startGame;
    public bool setTropsFase;
    public bool normalGame;

    // ===============================
    // 🔹 Listas del juego
    // ===============================
    [JsonIgnore] public LinkedList<PlayerInfo> playersList;
    [JsonIgnore] public LinkedList<Territorio> territoriesList;

    // Versiones serializables
    public List<PlayerInfo> playersArray;
    public List<TerritorioDTO> territoriesArray;

    // ===============================
    // 🔹 Serialización / reconstrucción
    // ===============================

    /// <summary>
    /// Convierte las estructuras internas (LinkedList) en listas serializables
    /// </summary>
    public void PrepareForSend()
    {
        try
        {
            // --- Jugadores ---
            if (playersList != null)
            {
                playersArray = new List<PlayerInfo>();
                var node = playersList.head;
                while (node != null)
                {
                    playersArray.Add(node.data);
                    node = node.next;
                }
            }

            // --- Territorios ---
            if (territoriesList != null)
            {
                territoriesArray = new List<TerritorioDTO>();
                var node = territoriesList.head;
                while (node != null)
                {
                    territoriesArray.Add(new TerritorioDTO(node.data));
                    node = node.next;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TurnInfo] Error serializando: {ex.Message}");
        }
    }

    /// <summary>
    /// Reconstruye las listas enlazadas a partir de las listas JSON recibidas
    /// </summary>
    public void RebuildLinkedLists()
    {
        try
        {
            // --- Jugadores ---
            playersList = new LinkedList<PlayerInfo>();
            if (playersArray != null)
            {
                foreach (var p in playersArray)
                    playersList.Add(p);
            }

            // --- Territorios ---
            territoriesList = new LinkedList<Territorio>();
            if (territoriesArray != null)
            {
                foreach (var dto in territoriesArray)
                    territoriesList.Add(dto.ToTerritorio());
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[TurnInfo] Error reconstruyendo listas: {ex.Message}");
        }
    }

    // ===============================
    // 🔹 Utilidades JSON
    // ===============================
    public string ToJson()
    {
        PrepareForSend();
        return JsonConvert.SerializeObject(this);
    }

    public static TurnInfo FromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        TurnInfo info = JsonConvert.DeserializeObject<TurnInfo>(json);
        info.RebuildLinkedLists();
        return info;
    }
}
