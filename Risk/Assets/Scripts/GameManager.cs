using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; //Instanciar el GameManager
    public ClientManager clientManager; //Estos son la base que manejan la comunicacion, siempre se deben llamar para enviar mensajes
    public ServerManager serverManager;

    public ListNode playersList;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartGame(TurnInfo message) //Inicia el juego
    {
        playersList = serverManager.server.clients; //Copia la lista de jugadores conectados
        Debug.Log("Juego iniciado para todos los jugadores.");
        SceneManager.LoadScene("Game");
        playersList.NextPlayer(); //Selecciona el primer jugador en la lista
    }

    public void manageMessages(TurnInfo turnInfo)
    {
        if (turnInfo.startGame == true)
        { //Revisa que si se de la alerta de iniciar todo por primera vez
            Instance.StartGame(turnInfo);
        }
        else if (turnInfo.numPlayers != 0) {
            GameRoomManager.Instance.UpdatePlayers(turnInfo.numPlayers);
        }
    }
}
