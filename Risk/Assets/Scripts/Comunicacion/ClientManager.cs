using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class ClientManager : MonoBehaviour
{
    private Client localPlayer;
    public Login_manager loginManager;
    public TMP_InputField inputUsername;
    public TMP_InputField inputIp;

    public async void ConnectToServer() //Crear un cliente a parte de la clase, este será el jugador local
    {
        string username = inputUsername.text;
        string ip = inputIp.text;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(ip))
        {
            loginManager.ShowInputError(inputIp); // Mostrar error visual si está vacío
            return;
        }

        localPlayer = new Client(username);

        // Suscribirse a eventos
        localPlayer.OnConnected += () =>
        {
            // Cambiar de escena solo si se conectó
            SceneManager.LoadScene("Game_room");
        };

        localPlayer.OnConnectionError += (msg) =>
        {
            Debug.LogError("Error al conectar: " + msg);
            loginManager.ShowInputError(inputIp); // Mostrar error visual
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
