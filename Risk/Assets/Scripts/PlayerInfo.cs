using System.Net.Sockets;
using CrazyRisk;        // Para Territorio, Ejercito, etc.
using CrazyRisk.Core;   // Si tus estructuras LinkedList están aquí
using Newtonsoft.Json;  // Para ignorar TcpClient en serialización

public class PlayerInfo
{
    // ===============================
    // Datos de red y jugador
    // ===============================

    [JsonIgnore] //  No se serializa (no se puede enviar por red)
    public TcpClient client;   // Conexión del jugador

    public string username;    // Nombre del jugador
    public string color;       // Color asignado al jugador
    public bool bot = false;   // Indica si es un bot o jugador real

    // ===============================
    //  Datos de juego
    // ===============================
    public Ejercito ejercitoPlayer;                 // Ejército asignado al jugador
    public LinkedList<Territorio> myTerritories;    // Territorios que controla

    // ===============================
    // Constructores
    // ===============================
    public PlayerInfo(TcpClient client, string username)
    {
        this.client = client;
        this.username = username;
        this.color = "gray";
        this.bot = false;
        this.ejercitoPlayer = null;
        this.myTerritories = new LinkedList<Territorio>();
    }

    // Alternativo (por compatibilidad con versiones anteriores)
    public PlayerInfo(TcpClient client)
    {
        this.client = client;
        this.username = "Guest";
        this.color = "gray";
        this.bot = false;
        this.ejercitoPlayer = null;
        this.myTerritories = new LinkedList<Territorio>();
    }

    // Para bots o pruebas sin cliente real
    public PlayerInfo(string username)
    {
        this.client = null;
        this.username = username;
        this.color = "gray";
        this.bot = true;
        this.ejercitoPlayer = null;
        this.myTerritories = new LinkedList<Territorio>();
    }

    // Constructor vacío para deserialización
    public PlayerInfo()
    {
        this.client = null;
        this.username = "Unknown";
        this.color = "gray";
        this.bot = false;
        this.ejercitoPlayer = null;
        this.myTerritories = new LinkedList<Territorio>();
    }

    // ===============================
    //  Métodos auxiliares
    // ===============================

    public void AddTerritory(Territorio territorio)
    {
        if (territorio == null) return;
        if (myTerritories == null) myTerritories = new LinkedList<Territorio>();

        // Evita duplicados
        if (!myTerritories.Contains(territorio))
            myTerritories.Add(territorio);
    }

    public void RemoveTerritory(Territorio territorio)
    {
        if (territorio == null || myTerritories == null) return;
        myTerritories.Remove(territorio);
    }

    public override string ToString()
    {
        string tipo = bot ? "BOT" : "PLAYER";
        int count = myTerritories != null ? myTerritories.Count() : 0;
        string ejercito = ejercitoPlayer != null ? ejercitoPlayer.TropasDisponibles.ToString() : "0";
        return $"{username} ({tipo}) - Tropas: {ejercito} - Territorios: {count}";
    }
}
