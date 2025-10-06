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
        AddTroop.gameObject.SetActive(true);
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        serverManager = serverManager ?? GetComponent<ServerManager>() ?? gameObject.AddComponent<ServerManager>();
        clientManager = clientManager ?? GetComponent<ClientManager>() ?? gameObject.AddComponent<ClientManager>();

        playersList = playersList ?? new LinkedList<PlayerInfo>();
        territoriesList = territoriesList ?? new LinkedList<Territorio>();

        // NO hacer GameObject.Find de UI aquí
        SceneManager.sceneLoaded += OnSceneLoaded_GameManager;

        Debug.Log("[GM] GameManager listo (DontDestroyOnLoad).");
    }

    private void OnSceneLoaded_GameManager(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;
        SceneManager.sceneLoaded -= OnSceneLoaded_GameManager;

        // Busca UI segura y asigna
        var go = GameObject.Find("AddTroop");
        if (go != null)
        {
            AddTroop = go.GetComponent<Button>();
            // no asumir que AddTroop != null
        }
        else
        {
            Debug.LogWarning("AddTroop no encontrado en la escena Game");
        }
    }



    public void StartGame(TurnInfo message) // Iniciar el juego cuando se recibe el mensaje de inicio
    {
        Debug.Log("Inciando juego");
        playersList = message.playersList;
        SceneManager.LoadScene("Game"); //Poner al erjercito neutro si son dos
        AddTroop.gameObject.SetActive(false);
        territoriesList = message.territoriesList;
    }

    public void ManageMessages(TurnInfo turnInfo)
    {
        if (turnInfo.startGame)
        {
            StartGame(turnInfo);
        }
        else if (turnInfo.setTropsFase == true)
        {
            updateTroopFase(turnInfo);
        }
    }

    public void updateTroopFase(TurnInfo turnInfo)
    {
        playersList = turnInfo.playersList;
        territoriesList = turnInfo.territoriesList;
        if (playersList.currPlayer.data.username == User_info.username)
        {
            AddTroop.gameObject.SetActive(true);
        }
    }


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

                LinkedList<Territorio>.Node randomTerritory = territoriesList.RandomNode(num);
                while (randomTerritory.data.Duenio != null)
                {
                    randomTerritory = territoriesList.RandomNode(num);
                }
                randomTerritory.data.CambiarDuenio(playersList.currPlayer.data.ejercitoPlayer);
                randomTerritory.data.AgregarTropas(1);
                playersList.currPlayer.data.myTerritories.Add(randomTerritory.data);
                playersList.nextPlayer();
            }
        }
    }

    private CrazyRisk.Continente GetContinenteFor(TerritorioId id)
    {
        switch (id)
        {
            // América del Norte
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

            // Sudamérica
            case TerritorioId.Venezuela:
            case TerritorioId.Peru:
            case TerritorioId.Brasil:
            case TerritorioId.Argentina:
                return Continente.AmericaSur;

            // Europa
            case TerritorioId.Islandia:
            case TerritorioId.GranBretana:
            case TerritorioId.Escandinavia:
            case TerritorioId.EuropaNorte:
            case TerritorioId.EuropaOccidental:
            case TerritorioId.EuropaSur:
            case TerritorioId.Ucrania:
                return Continente.Europa;

            // África
            case TerritorioId.AfricaNorte:
            case TerritorioId.Egipto:
            case TerritorioId.AfricaEste:
            case TerritorioId.Congo:
            case TerritorioId.AfricaSur:
            case TerritorioId.Madagascar:
                return Continente.Africa;

            // Asia
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

            // Oceanía
            case TerritorioId.Indonesia:
            case TerritorioId.NuevaGuinea:
            case TerritorioId.AustraliaOccidental:
            case TerritorioId.AustraliaOriental:
                return Continente.Oceania;

            default:
                throw new ArgumentOutOfRangeException(nameof(id), $"Territorio desconocido: {id}");
        }
    }

    public void setEjercito()
    {
        for (int i = 0; i < 3; i++)
        {
            playersList.currPlayer.data.ejercitoPlayer = new Ejercito(playersList.currPlayer.data.username, playersList.currPlayer.data.color, 37);
            playersList.nextPlayer();
        }
    }

    public void addTroopToTerritory(TerritorioId territorio, PlayerInfo player)
    {
        searchTerritory(territorio, player.myTerritories).AgregarTropas(1);
    }

    public void addTroopToTerritoryBot()
    {
        playersList.currPlayer.data.myTerritories.RandomNode(playersList.currPlayer.data.myTerritories.Count()).data.AgregarTropas(1);
        playersList.currPlayer.data.ejercitoPlayer.removeTrop();
    }

    public Territorio searchTerritory(TerritorioId territorio, LinkedList<Territorio> territoriesToLook)
    {
        territoriesToLook.currNode = territoriesToLook.head;
        TerritorioId currTerr = territoriesToLook.head.data.Id;
        while (territoriesToLook.head.data.Id != territorio)
        {
            territoriesList.nextNode();
        }
        return territoriesToLook.currNode.data;
    }
    public void addTropButton()
    {
        string[] territoriostring = TerritoryNode.ObtenerSeleccionados();
        if (territoriostring.Length > 1)
        {
            audioSource.PlayOneShot(errorSound);
        }
        else
        {
            Territorio territorio = FindTerritory(territoriostring[0], territoriesList);
            if (territorio == null || territorio.Duenio.Alias != playersList.currPlayer.data.username)
            {
                audioSource.PlayOneShot(errorSound);
            }
            else
            {
                addTroopToTerritory(territorio.Id, playersList.currPlayer.data);
                playersList.currPlayer.data.ejercitoPlayer.removeTrop();
                AddTroop.gameObject.SetActive(false);
                playersList.nextPlayer();
                if (playersList.currPlayer.data.bot == true)
                {
                    addTroopToTerritoryBot();
                    playersList.nextPlayer();
                }
                TurnInfo newMessage = new TurnInfo();
                newMessage.playersList = playersList;
                newMessage.territoriesList = territoriesList;
                if (playersList.currPlayer.next == null && playersList.currPlayer.data.ejercitoPlayer.TropasDisponibles == 0)
                {
                    newMessage.normalGame = true;
                }
                else
                {
                    newMessage.setTropsFase = true;
                }
                clientManager.SendMove(newMessage);
            }
        }


    }

    public Territorio FindTerritory(string territory, LinkedList<Territorio> playerTerritories)
    {
        playerTerritories.currNode = playerTerritories.head;
        while (playerTerritories.currNode != null && playerTerritories.currNode.data.Nombre != territory)
        {
            playerTerritories.nextNode();
        }
        if (playerTerritories.currNode == null)
        {
            return null;
        }
        return playerTerritories.currNode.data;
    }   
}

