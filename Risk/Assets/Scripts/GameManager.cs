using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Net.Sockets;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ClientManager clientManager;
    public ServerManager serverManager;

    public LinkedList<TcpClient> playersList;
    private int? pendingPlayersUpdate;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        serverManager = serverManager ?? GetComponent<ServerManager>() ?? gameObject.AddComponent<ServerManager>();
        clientManager = clientManager ?? GetComponent<ClientManager>() ?? gameObject.AddComponent<ClientManager>();

        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[GM] GameManager listo (DontDestroyOnLoad).");
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene s, LoadSceneMode m)
    {
        if (s.name == "GameRoom" && pendingPlayersUpdate.HasValue)
        {
            if (GameRoomManager.Instance != null)
                GameRoomManager.Instance.UpdatePlayers(1, serverManager.ip);
            pendingPlayersUpdate = null;
        }
    }

    public void StartGame(TurnInfo message)
    {
        if (serverManager.server.clients.Count() >= 2)
        {
            playersList = serverManager.server.clients; //Lista de los jugadores que van a jugar.
            playersList.head.color = "red";
            playersList.head.next.color = "blue";
            if (playersList.head.next.next != null) playersList.head.next.next.color = "gray";
            SceneManager.LoadScene("Game");
            playersList?.nextPlayer();
        }
        
    }

    public void ManageMessages(TurnInfo turnInfo)
    {
        if (turnInfo.startGame)
        {
            StartGame(turnInfo);
        }
        else if (turnInfo.numPlayers > 0)
        {
            if (GameRoomManager.Instance != null)
                GameRoomManager.Instance.UpdatePlayers(turnInfo.numPlayers, turnInfo.ipCode);
            else
                pendingPlayersUpdate = turnInfo.numPlayers;
        }
        else
        {
            updateGame(turnInfo);
        }
    }

    public void updateGame(TurnInfo turnInfo) //Aca se actualizan las variables propias del jugador
    {


        playersList?.nextPlayer();
    }
}

