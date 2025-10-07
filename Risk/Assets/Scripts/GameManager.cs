using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using CrazyRisk.Core;
using CrazyRisk;
using UnityEngine.UI;
using Unity.AppUI.Core;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ClientManager clientManager;
    public ServerManager serverManager;
    private static System.Random rnd = new System.Random();

    public LinkedList<PlayerInfo> playersList;
    public LinkedList<Territorio> territoriesList;

    public Button AddTroop;

    public AudioSource audioSource;
    public AudioClip errorSound;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }

    public void setButtonActive()
    {
        if (AddTroop != null)
            AddTroop.gameObject.SetActive(true);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Inicialización segura
        serverManager = GetComponent<ServerManager>() ?? gameObject.AddComponent<ServerManager>();
        clientManager = GetComponent<ClientManager>() ?? gameObject.AddComponent<ClientManager>();

        if (playersList == null) playersList = new LinkedList<PlayerInfo>();
        if (territoriesList == null) territoriesList = new LinkedList<Territorio>();

        SceneManager.sceneLoaded += OnSceneLoaded_GameManager;
        Debug.Log("[GM] GameManager listo (DontDestroyOnLoad).");
    }

    private void OnSceneLoaded_GameManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;

        SceneManager.sceneLoaded -= OnSceneLoaded_GameManager;

        var go = GameObject.Find("AddTroop");
        if (go != null)
        {
            AddTroop = go.GetComponent<Button>();
        }
        else
        {
            Debug.LogWarning("[GM] AddTroop no encontrado en la escena Game");
        }
    }

    // ===============================
    // 🔹 Iniciar juego (desde TurnInfo)
    // ===============================
    public void StartGame(TurnInfo message)
    {
        Debug.Log("Iniciando juego...");

        // Protección contra null
        if (message == null)
        {
            Debug.LogError("[GM] El mensaje TurnInfo recibido es null.");
            return;
        }

        if (message.playersList == null)
            Debug.LogWarning("⚠️ La lista de jugadores está vacía");
        if (message.territoriesList == null)
            Debug.LogWarning("⚠️ La lista de territorios está vacía");

        // Asignar listas recibidas
        playersList = message.playersList ?? new LinkedList<PlayerInfo>();
        territoriesList = message.territoriesList ?? new LinkedList<Territorio>();

        SceneManager.LoadScene("Game");
    }

    // ===============================
    // 🔹 Gestionar mensajes recibidos
    // ===============================
    public void ManageMessages(TurnInfo turnInfo)
    {
        if (turnInfo == null)
        {
            Debug.LogWarning("[GM] TurnInfo recibido es null.");
            return;
        }

        if (turnInfo.startGame)
        {
            StartGame(turnInfo);
        }
        else if (turnInfo.setTropsFase)
        {
            updateTroopFase(turnInfo);
        }
    }

    // ===============================
    // 🔹 Actualizar fase de tropas
    // ===============================
    public void updateTroopFase(TurnInfo turnInfo)
    {
        playersList = turnInfo.playersList ?? playersList;
        territoriesList = turnInfo.territoriesList ?? territoriesList;

        if (playersList.currPlayer != null &&
            playersList.currPlayer.data.username == User_info.username)
        {
            AddTroop?.gameObject.SetActive(true);
        }
    }

    // ===============================
    // 🔹 Generar territorios
    // ===============================
    public void setTerritories()
    {
        var territorios = (CrazyRisk.TerritorioId[])Enum.GetValues(typeof(CrazyRisk.TerritorioId));
        int territoriosNum = 5;

        foreach (var id in territorios)
        {
            Territorio newTerritorio = new Territorio(id, GetContinenteFor(id));
            territoriesList.Add(newTerritorio);
        }

        int num = territoriesList.Count();

        for (int i = 0; i < territoriosNum; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var randomTerritory = territoriesList.RandomNode(num);
                while (randomTerritory.data.Duenio != null)
                    randomTerritory = territoriesList.RandomNode(num);

                randomTerritory.data.CambiarDuenio(playersList.currPlayer.data.ejercitoPlayer);
                randomTerritory.data.AgregarTropas(1);
                playersList.currPlayer.data.myTerritories.Add(randomTerritory.data);
                playersList.nextPlayer();
            }
        }
    }

    // ===============================
    // 🔹 Determinar continente
    // ===============================
    private CrazyRisk.Continente GetContinenteFor(TerritorioId id)
    {
        switch (id)
        {
            case TerritorioId.Alaska:
            case TerritorioId.NWTerritory:
            case TerritorioId.Groenlandia:
            case TerritorioId.Alberta:
            case TerritorioId.Ontario:
            case TerritorioId.Quebec:
            case TerritorioId.OesteEEUU:
            case TerritorioId.EsteEEUU:
            case TerritorioId.CentroAmerica:
                return Continente.AmericaNorte;

            case TerritorioId.Venezuela:
            case TerritorioId.Peru:
            case TerritorioId.Brasil:
            case TerritorioId.Argentina:
                return Continente.AmericaSur;

            case TerritorioId.Islandia:
            case TerritorioId.GranBretana:
            case TerritorioId.Escandinavia:
            case TerritorioId.EuropaNorte:
            case TerritorioId.EuropaOccidental:
            case TerritorioId.EuropaSur:
            case TerritorioId.Ucrania:
                return Continente.Europa;

            case TerritorioId.AfricaNorte:
            case TerritorioId.Egipto:
            case TerritorioId.AfricaEste:
            case TerritorioId.Congo:
            case TerritorioId.AfricaSur:
            case TerritorioId.Madagascar:
                return Continente.Africa;

            case TerritorioId.Ural:
            case TerritorioId.Siberia:
            case TerritorioId.Yakutsk:
            case TerritorioId.Kamchatka:
            case TerritorioId.Irkutsk:
            case TerritorioId.Mongolia:
            case TerritorioId.Japon:
            case TerritorioId.China:
            case TerritorioId.MedioOriente:
            case TerritorioId.India:
            case TerritorioId.Siam:
            case TerritorioId.Afganistan:
                return Continente.Asia;

            case TerritorioId.Indonesia:
            case TerritorioId.NuevaGuinea:
            case TerritorioId.AustraliaOccidental:
            case TerritorioId.AustraliaOriental:
                return Continente.Oceania;

            default:
                throw new ArgumentOutOfRangeException(nameof(id), $"Territorio desconocido: {id}");
        }
    }

    // ===============================
    // 🔹 Crear ejércitos iniciales
    // ===============================
    public void setEjercito()
    {
        for (int i = 0; i < 3; i++)
        {
            playersList.currPlayer.data.ejercitoPlayer = new Ejercito(playersList.currPlayer.data.username, playersList.currPlayer.data.color, 37);
            playersList.nextPlayer();
        }
    }

    // ===============================
    // 🔹 Agregar tropas a territorio
    // ===============================
    public void addTroopToTerritory(TerritorioId territorio, PlayerInfo player)
    {
        var t = searchTerritory(territorio, player.myTerritories);
        t?.AgregarTropas(1);
    }

    public void addTroopToTerritoryBot()
    {
        var random = playersList.currPlayer.data.myTerritories.RandomNode(playersList.currPlayer.data.myTerritories.Count());
        random.data.AgregarTropas(1);
        playersList.currPlayer.data.ejercitoPlayer.removeTrop();
    }

    // ===============================
    // 🔹 Buscar territorio específico
    // ===============================
    public Territorio searchTerritory(TerritorioId territorio, LinkedList<Territorio> territoriesToLook)
    {
        territoriesToLook.currNode = territoriesToLook.head;

        while (territoriesToLook.currNode != null)
        {
            if (territoriesToLook.currNode.data.Id == territorio)
                return territoriesToLook.currNode.data;

            territoriesToLook.nextNode();
        }

        return null;
    }

    // ===============================
    // 🔹 Botón agregar tropa
    // ===============================
    public void addTropButton()
    {
        string[] territoriostring = TerritoryNode.ObtenerSeleccionados();

        if (territoriostring.Length > 1)
        {
            audioSource.PlayOneShot(errorSound);
            return;
        }

        Territorio territorio = FindTerritory(territoriostring[0], territoriesList);

        if (territorio == null || territorio.Duenio.Alias != playersList.currPlayer.data.username)
        {
            audioSource.PlayOneShot(errorSound);
            return;
        }

        addTroopToTerritory(territorio.Id, playersList.currPlayer.data);
        playersList.currPlayer.data.ejercitoPlayer.removeTrop();
        AddTroop.gameObject.SetActive(false);
        playersList.nextPlayer();

        if (playersList.currPlayer.data.bot)
        {
            addTroopToTerritoryBot();
            playersList.nextPlayer();
        }

        TurnInfo newMessage = new TurnInfo
        {
            playersList = playersList,
            territoriesList = territoriesList,
            setTropsFase = true
        };

        if (playersList.currPlayer.next == null && playersList.currPlayer.data.ejercitoPlayer.TropasDisponibles == 0)
            newMessage.normalGame = true;

        clientManager.SendMove(newMessage);
    }

    // ===============================
    // 🔹 Buscar territorio por nombre
    // ===============================
    public Territorio FindTerritory(string territory, LinkedList<Territorio> playerTerritories)
    {
        playerTerritories.currNode = playerTerritories.head;

        while (playerTerritories.currNode != null)
        {
            if (playerTerritories.currNode.data.Nombre == territory)
                return playerTerritories.currNode.data;

            playerTerritories.nextNode();
        }

        return null;
    }
}
