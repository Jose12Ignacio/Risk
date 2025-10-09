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
    // Datos principales del turno
    // ===============================
    // Estas variables indican el tipo de acción y el estado actual del turno en el juego.
    public string actionType = "none";   // Tipo de acción actual (por ejemplo: "attack", "move", "reinforce").
    public bool startGame;               // Indica si se está iniciando la partida.
    public bool setTropsFase;            // Indica si el turno está en la fase de colocar tropas.
    public bool normalGame;              // Indica si el turno pertenece a la fase de juego normal.

    // ===============================
    // Listas del juego
    // ===============================
    // Estas listas contienen los datos de los jugadores y territorios.
    // Se marcan con [JsonIgnore] porque las LinkedList personalizadas no pueden serializarse directamente a JSON.
    [JsonIgnore] public LinkedList<PlayerInfo> playersList;     // Lista enlazada de jugadores.
    [JsonIgnore] public LinkedList<Territorio> territoriesList; // Lista enlazada de territorios.

    // Versiones serializables
    // Estas listas se usan para enviar los datos por red, ya que las LinkedList no son compatibles con JSON.
    public List<PlayerInfo> playersArray;           // Copia serializable de la lista de jugadores.
    public List<TerritorioDTO> territoriesArray;    // Copia serializable de la lista de territorios (en formato DTO).

    
    // Serialización / reconstrucción
  
    // Este método prepara los datos para ser enviados por red.
    // Convierte las estructuras LinkedList en listas normales (List<T>) que pueden serializarse.
    public void PrepareForSend()
    {
        try
        {
            // --- Jugadores ---
            if (playersList != null)
            {
                playersArray = new List<PlayerInfo>();
                var node = playersList.head;

                // Recorre cada nodo de la lista enlazada y lo agrega al array serializable.
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

                // Convierte cada territorio en un objeto DTO (versión serializable).
                while (node != null)
                {
                    territoriesArray.Add(new TerritorioDTO(node.data));
                    node = node.next;
                }
            }
        }
        catch (Exception ex)
        {
            // Si ocurre un error durante la conversión, se muestra en la consola.
            Debug.LogError($"[TurnInfo] Error serializando: {ex.Message}");
        }
    }

    // Este método reconstruye las listas enlazadas a partir de los datos deserializados.
    // Se usa cuando se recibe un mensaje desde el servidor o desde otro cliente.
    public void RebuildLinkedLists()
    {
        try
        {
            // --- Jugadores ---
            playersList = new LinkedList<PlayerInfo>();
            if (playersArray != null)
            {
                foreach (var p in playersArray)
                    playersList.Add(p);  // Se reconstruye la lista enlazada de jugadores.
            }

            // --- Territorios ---
            territoriesList = new LinkedList<Territorio>();
            if (territoriesArray != null)
            {
                foreach (var dto in territoriesArray)
                    territoriesList.Add(dto.ToTerritorio());  // Se convierte el DTO nuevamente en un objeto Territorio.
            }
        }
        catch (Exception ex)
        {
            // Si ocurre un error durante la reconstrucción, se muestra en la consola.
            Debug.LogError($"[TurnInfo] Error reconstruyendo listas: {ex.Message}");
        }
    }

    // Convierte el objeto TurnInfo completo en una cadena JSON.
    // Se llama cuando se va a enviar el turno por red o guardar en archivo.
    public string ToJson()
    {
        PrepareForSend(); // Se asegura de convertir las listas antes de serializar.
        return JsonConvert.SerializeObject(this);
    }

    // Crea un objeto TurnInfo a partir de una cadena JSON recibida.
    // También llama a RebuildLinkedLists() para restaurar las estructuras originales.
    public static TurnInfo FromJson(string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        TurnInfo info = JsonConvert.DeserializeObject<TurnInfo>(json);
        info.RebuildLinkedLists();
        return info;
    }
}
