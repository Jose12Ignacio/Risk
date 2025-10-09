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
        localPlayer = new Client(username);

        localPlayer.OnConnected += () =>
        {
            SceneManager.LoadScene("GameRoom");
        };

        localPlayer.OnConnectionError += (msg) =>
        {
            Debug.LogError("Error al conectar: " + msg);
        };

        await localPlayer.Connect(inputIp, 5000);
    }

    public async void SendMove(TurnInfo action)
    {
        Debug.Log("LocalPlayer" + localPlayer != null);
        if (localPlayer != null)
        {
            await localPlayer.SendAction(action);
            Debug.Log("Mensaje enviado");
        }
    }
}
