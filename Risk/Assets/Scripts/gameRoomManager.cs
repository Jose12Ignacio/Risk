using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameRoomManager : MonoBehaviour
{
    public static GameRoomManager Instance;
    public Button startGameButton;
    public TextMeshProUGUI playersNumber;
    public TextMeshProUGUI ipCode;

    public int numPlayers = 1;
    public int prevNumPlayers = -2;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Busca los objetos UI en la nueva escena y los asigna
        startGameButton = GameObject.Find("startGame")?.GetComponent<Button>();
        if (User_info.manager == true && startGameButton != null)
            startGameButton.gameObject.SetActive(true);
        else if (startGameButton != null)
            startGameButton.gameObject.SetActive(false);

        playersNumber = GameObject.Find("Text (TMP)  playersNumber")?.GetComponent<TextMeshProUGUI>();
        ipCode = GameObject.Find("Text (TMP)  ipCode")?.GetComponent<TextMeshProUGUI>();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "GameRoom")
        {
            if (User_info.manager == true)
            {
                numPlayers = GameManager.Instance.serverManager.getPlayers();
                if (prevNumPlayers != numPlayers)
                {
                    playersNumber.text = $"Jugadores conectados: {numPlayers}";
                    prevNumPlayers = numPlayers;
                    TurnInfo changePlayersNum = new TurnInfo();
                    changePlayersNum.numPlayers = numPlayers;
                    changePlayersNum.ipCode = GameManager.Instance.serverManager.ip;
                    changePlayersNum.gameRoom = true;
                    GameManager.Instance.clientManager.SendMove(changePlayersNum);
                }
                UpdatePlayers(numPlayers, GameManager.Instance.serverManager.ip);
            }
        }
    }

    public void UpdatePlayers(int players, string ip)
    {
        playersNumber.text = $"Jugadores conectados: {players}";
        ipCode.text = "Ip: " + ip;
        Debug.Log(ip);
        Debug.Log("hola");
    }

    public void startGame()
    {
        if (numPlayers >= 2)
        {
            TurnInfo startGameMessage = new TurnInfo(); //Enivar el mensaje a todos de que el juego inicia.
            startGameMessage.startGame = true;
            GameManager.Instance.clientManager.SendMove(startGameMessage); //Tal vez agregar mensaje de error si no se cumple.
            GameManager.Instance.StartGame(startGameMessage);
        }
    }
}
