using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance;

    // Cache UI
    private Button startGameButton;
    private TextMeshProUGUI playersNumber;

    public int numPlayers = 1;
    private int prevNumPlayers = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("[GRM] Instancia creada y marcada como DontDestroyOnLoad.");
        }
        else { Destroy(gameObject); }
    }

    void OnDestroy()
    {
        if (Instance == this) SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Botón
        var btnGO = GameObject.Find("startGame");
        startGameButton = (btnGO != null) ? btnGO.GetComponent<Button>() : null;

        if (startGameButton != null)
        {
            // OJO: NO agregamos AddListener aquí porque ya lo conecta el Inspector
            startGameButton.gameObject.SetActive(User_info.manager);
            startGameButton.interactable = User_info.manager;
        }

        // Texto jugadores
        var txtGO = GameObject.Find("Text (TMP)  playersNumber"); // dos espacios
        playersNumber = (txtGO != null) ? txtGO.GetComponent<TextMeshProUGUI>() : null;

        if (playersNumber != null)
            playersNumber.text = $"Jugadores conectados: {numPlayers}";
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "GameRoom") return;

        if (User_info.manager && GameManager.Instance?.serverManager != null)
        {
            numPlayers = GameManager.Instance.serverManager.getPlayers();
            if (prevNumPlayers != numPlayers)
            {
                UpdatePlayers(numPlayers);

                var changePlayersNum = new TurnInfo { numPlayers = numPlayers };
                GameManager.Instance.clientManager?.SendMove(changePlayersNum);
            }
        }
    }

    public void UpdatePlayers(int players)
    {
        if (playersNumber != null)
            playersNumber.text = $"Jugadores conectados: {n}";
    }
}
