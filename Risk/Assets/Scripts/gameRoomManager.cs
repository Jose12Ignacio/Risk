using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance;
    private Button startGameButton;
    private TextMeshProUGUI ipCode;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Bootstrap()
    {
        if (Instance == null)
        {
            var go = new GameObject("GameRoomManager");
            go.AddComponent<GameRoomManager>(); // Awake corre aquí
        }
    }

    void Awake()
    {

        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[GRM] listo (DDL).");

    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "GameRoom") return;

        var btnGO = GameObject.Find("startGame");
        var txtGO = GameObject.Find("ipCode");

        startGameButton = btnGO ? btnGO.GetComponent<Button>() : null;
        ipCode = txtGO ? txtGO.GetComponent<TextMeshProUGUI>() : null;

        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(sendStartMessage);
        }
    }


    public void sendStartMessage()
    {
        if (!User_info.manager) return;

        if (GameManager.Instance == null || GameManager.Instance.serverManager?.server == null)
        {
            Debug.LogError("GameManager o server no inicializados");
            return;
        }

        var clients = GameManager.Instance.serverManager.server.players;
        if (clients == null || clients.Count() < 2)
        {
            Debug.LogError("clients es null o < 2");
            return;
        }

        // Asignar playersList (datos)
        GameManager.Instance.playersList = clients;

        // Validar antes de acceder a head/data
        if (clients.head == null || clients.head.data == null) { Debug.LogError("clients.head o head.data null"); return; }
        clients.head.data.color = "red";

        if (clients.head.next == null || clients.head.next.data == null) { Debug.LogError("clients.head.next o next.data null"); return; }
        clients.head.next.data.color = "blue";

        // Añadir bot si hace falta
        if (clients.head.next.next == null)
        {
            PlayerInfo bot = new PlayerInfo("bot");
            bot.bot = true;
            GameManager.Instance.playersList.Add(bot);
        }

        if (GameManager.Instance.playersList.head.next.next?.data != null)
            GameManager.Instance.playersList.head.next.next.data.color = "gray";

        GameManager.Instance.playersList.nextPlayer();

        // Inicializaciones de datos que no dependen de la UI
        GameManager.Instance.setEjercito();
        GameManager.Instance.setTerritories();

        // Preparar y enviar mensaje
        TurnInfo message = new TurnInfo
        {
            startGame = true,
            playersList = GameManager.Instance.playersList,
            territoriesList = GameManager.Instance.territoriesList
        };

        Debug.Log("mi nombre");
        Debug.Log(message.playersList.head.data.username);
        

        // Registrar callback y cargar escena; la inicialización de UI se hará en OnGameSceneLoaded
        SceneManager.sceneLoaded += OnGameSceneLoaded;
        SceneManager.LoadScene("Game");
        Debug.Log(message.playersList == null);

        // Enviar al servidor (puedes hacerlo antes o después según tu protocolo)
        GameManager.Instance.clientManager?.SendMove(message);
    }

    private void OnGameSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Game") return;
        SceneManager.sceneLoaded -= OnGameSceneLoaded;

        var go = GameObject.Find("AddTroop");
        if (go != null)
        {
            GameManager.Instance.AddTroop = go.GetComponent<Button>();
            if (GameManager.Instance.AddTroop != null)
                GameManager.Instance.setButtonActive();
            else
                Debug.LogWarning("AddTroop existe pero no tiene componente Button");
        }
        else
        {
            Debug.LogWarning("AddTroop no encontrado en la escena Game");
        }

        Debug.Log($"Jugadores: {GameManager.Instance.playersList?.Count()} - escena inicializada");
    }


    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameRoom" && User_info.manager == true)
        {
            if (ipCode != null && GameManager.Instance.serverManager != null)
            {
                ipCode.text = $"Ip: {GameManager.Instance.serverManager.ip}";
                //Debug.Log(GameManager.Instance.serverManager.server.clients.Count());
            }
        }
        
        
    }
}