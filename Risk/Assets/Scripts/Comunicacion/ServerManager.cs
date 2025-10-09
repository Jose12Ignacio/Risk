using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;

public class ServerManager : MonoBehaviour
{
    public string playerName;
    public TextMeshProUGUI playersNumber;
    public string ip;
    public static int port = 5000;
    public Server server;
    private Client localPlayer;

    void Awake()
    {
        if (FindObjectsByType<ServerManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Debug.Log("[SERVERMANAGER] Persistente entre escenas.");
    }

    public async void StartServerAndLocalPlayer()
    {
        ip = GetLocalIPAddress();

        if (!System.Net.IPAddress.TryParse(ip, out _))
        {
            Debug.LogError("[SERVERMANAGER] IP local inválida. No se puede iniciar el servidor.");
            return;
        }

        int tries = 0;
        while (GameManager.Instance == null && tries < 50)
        {
            await Task.Delay(100);
            tries++;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogError("[SERVERMANAGER] GameManager no se inicializó correctamente.");
            return;
        }

        server = new Server();

        Debug.Log($"[SERVERMANAGER] Iniciando servidor en {ip}:{port} ...");
        await server.StartServer(port);

        Debug.Log("[SERVERMANAGER] Servidor iniciado correctamente.");

        playerName = User_info.username;

        await Task.Delay(300);

        if (GameManager.Instance.clientManager == null)
        {
            Debug.LogError("[SERVERMANAGER] ClientManager es null, no se puede conectar.");
            return;
        }

        GameManager.Instance.clientManager.ConnectToServer(playerName, ip);
        Debug.Log($"[SERVERMANAGER] Cliente local '{playerName}' conectado al servidor en {ip}:{port}");
    }

    public async void OnStartGamePressed()
    {
        if (GameManager.Instance == null || server == null)
        {
            Debug.LogError("[SERVER_MANAGER] No se puede iniciar el juego: dependencias nulas.");
            return;
        }

        Debug.Log("[SERVER_MANAGER] Iniciando juego para todos los jugadores...");

        TurnInfo startMessage = new TurnInfo();
        startMessage.actionType = "startGame";
        startMessage.startGame = true;
        startMessage.playersList = GameManager.Instance.playersList;
        startMessage.territoriesList = GameManager.Instance.territoriesList;

        await server.BroadcastMessage(startMessage, null);
        Debug.Log("[SERVER_MANAGER] Mensaje 'startGame' enviado a los clientes.");

        GameManager.Instance.StartGame(startMessage);
    }

    public static string GetLocalIPAddress()
    {
        string localIP = "";
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530);
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                localIP = endPoint.Address.ToString();
            }
        }
        catch
        {
            Debug.LogError("No se pudo obtener la IP local.");
        }

        return localIP;
    }

    public int getPlayers()
    {
        if (server == null || server.clients == null)
        {
            Debug.LogWarning("[SERVERMANAGER] Servidor o lista de clientes aún no inicializados.");
            return 0;
        }
        return server.clients.Count();
    }
}
