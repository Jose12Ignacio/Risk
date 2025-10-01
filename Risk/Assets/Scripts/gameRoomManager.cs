using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-900)]
public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance;
    private Button startGameButton;
    private TextMeshProUGUI playersNumber;
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
        var txtGO = GameObject.Find("playersNumber");
        var txtGO2 = GameObject.Find("ipCode");

        startGameButton = btnGO ? btnGO.GetComponent<Button>() : null;
        playersNumber = txtGO ? txtGO.GetComponent<TextMeshProUGUI>() : null;
        ipCode = txtGO2 ? txtGO.GetComponent<TextMeshProUGUI>() : null;

        if (startGameButton != null)
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(() =>
            {
                Debug.Log("[GRM] StartGame clicado.");
                GameManager.Instance.ManageMessages(new TurnInfo { startGame = true });
            });
        }
        UpdatePlayers(1, GameManager.Instance.serverManager.ip);
    }

    public void UpdatePlayers(int n, string ip)
    {
        if (playersNumber != null)
            playersNumber.text = $"Jugadores conectados: {n}";
        ipCode.text = $"Ip: {ip}";
    }
}
