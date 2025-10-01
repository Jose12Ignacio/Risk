using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public ClientManager clientManager;
    public ServerManager serverManager;

    public ListNode playersList;
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
                GameRoomManager.Instance.UpdatePlayers(pendingPlayersUpdate.Value);
            pendingPlayersUpdate = null;
        }
    }

    public void StartGame(TurnInfo message)
    {
        if (serverManager != null && serverManager.server != null)
            playersList = serverManager.server.clients;
        else
            Debug.LogWarning("[GM] StartGame: serverManager o server nulo; se omite playersList.");

        Debug.Log("[GM] Juego iniciado para todos los jugadores.");
        SceneManager.LoadScene("Game");

        playersList?.NextPlayer();
    }

    public void ManageMessages(TurnInfo turnInfo)
    {
        if (turnInfo.startGame)
        {
            StartGame(turnInfo);
            return;
        }

        if (turnInfo.numPlayers > 0)
        {
            if (GameRoomManager.Instance != null)
                GameRoomManager.Instance.UpdatePlayers(turnInfo.numPlayers);
            else
                pendingPlayersUpdate = turnInfo.numPlayers;

        }
        Debug.Log("manageMesagges");
    }
}

