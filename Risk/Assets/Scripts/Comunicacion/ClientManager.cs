using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ClientManager : MonoBehaviour
{
    private Client localPlayer;
    public Login_manager loginManager;
    public TMP_InputField inputUsername;
    public TMP_InputField inputIp;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async void ConnectToServer()
    {
        string username = inputUsername.text;
        string ip = inputIp.text;

        if (string.IsNullOrWhiteSpace(username))
        {
            loginManager.ShowInputError(inputIp);
            return;
        }

        // Crear el cliente con el username
        localPlayer = new Client(username);

        // Suscribirse a eventos ANTES de intentar conectar
        localPlayer.OnConnected += () =>
        {
            Debug.Log("Conexión exitosa, cambiando a GameRoom...");
            SceneManager.LoadScene("GameRoom");
        };

        localPlayer.OnConnectionError += (msg) =>
        {
            Debug.LogError("Error al conectar: " + msg);
            loginManager.ShowInputError(inputIp);
        };

        // Intentar conectarse
        await localPlayer.Connect(ip, 5000);
    }

    public async void SendMove(TurnInfo action) //Enviar información del movimiento hecho
    {
        if (localPlayer != null)
            await localPlayer.SendAction(action); //Llama a la función en si
        
    }

}
