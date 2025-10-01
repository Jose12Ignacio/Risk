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

    public LinkedList<PlayerInfo> playersList;
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

        Debug.Log("[GM] GameManager listo (DontDestroyOnLoad).");
    }

    public void StartGame(TurnInfo message) // Iniciar el juego cuando se recibe el mensaje de inicio
    {
        playersList = message.playersList;
        SceneManager.LoadScene("Game");
        playersList?.nextPlayer();
        
    }

    public void ManageMessages(TurnInfo turnInfo)
    {
        if (turnInfo.startGame)
        {
            StartGame(turnInfo);
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

