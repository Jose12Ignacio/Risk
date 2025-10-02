using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ClientManager : MonoBehaviour
{
    private Client localPlayer;
    public Login_manager loginManager;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public async void ConnectToServer(string username, string inputIp)
    {
        
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
        };

        // Intentar conectarse
        await localPlayer.Connect(inputIp, 5000);
    }

    public async void SendMove(TurnInfo action) //Enviar información del movimiento hecho
    {
        if (localPlayer != null)
            await localPlayer.SendAction(action); //Llama a la función en si
        
    }

}
