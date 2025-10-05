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


    public void sendStartMessage()//Va a enviar una el mensaje de incio y pasa a la nueva escena
    {
        if (User_info.manager == true && GameManager.Instance.serverManager.server.clients.Count() >= 2)
        {
            TurnInfo message = new TurnInfo();
            message.startGame = true;
            GameManager.Instance.playersList = GameManager.Instance.serverManager.server.clients;
            GameManager.Instance.playersList.head.data.color = "red";
            GameManager.Instance.playersList.head.next.data.color = "blue";
            if (GameManager.Instance.playersList.head.next.next == null)
            {
                PlayerInfo bot = new PlayerInfo(null, "bot");
                bot.bot = true;
                GameManager.Instance.playersList.Add(bot);
            }
            GameManager.Instance.playersList.head.next.next.data.color = "gray";
            GameManager.Instance.playersList.nextPlayer();

            GameManager.Instance.setEjercito();
            GameManager.Instance.setTerritories();
            message.playersList = GameManager.Instance.playersList;
            message.territoriesList = GameManager.Instance.territoriesList;
            Debug.Log("Cargando escena");
            SceneManager.LoadScene("Game");
            GameManager.Instance.clientManager.SendMove(message);
            GameManager.Instance.addTroop = GameObject.Find("AddTroop").GetComponent<Button>();
            GameManager.Instance.addTroop.gameObject.SetActive(true);
            GameManager.Instance.setButtonActive();
            Debug.Log(GameManager.Instance.playersList.Count() + "escena cargada");
        }

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