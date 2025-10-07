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
        // Evitar duplicados persistentes
        if (FindObjectsByType<ServerManager>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        Debug.Log("[SERVERMANAGER] Persistente entre escenas.");
    }

    // 🔹 Método async para iniciar servidor + cliente local
    public async void StartServerAndLocalPlayer()
    {
        ip = GetLocalIPAddress();

        if (!System.Net.IPAddress.TryParse(ip, out _))
        {
            Debug.LogError("[SERVERMANAGER] IP local inválida. No se puede iniciar el servidor.");
            return;
        }

        // === Esperar a que GameManager esté listo ===
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

        // === Iniciar servidor ===
        server = new Server();

        Debug.Log($"[SERVERMANAGER] Iniciando servidor en {ip}:{port} ...");
        await server.StartServer(port);

        Debug.Log("[SERVERMANAGER] Servidor iniciado correctamente.");

        // === Conectar jugador local ===
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

    // 🔹 Iniciar la partida cuando el host presiona "Start Game"
    public async void OnStartGamePressed()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("[SERVER_MANAGER] ❌ GameManager.Instance es null.");
            return;
        }

        if (server == null)
        {
            Debug.LogError("[SERVER_MANAGER] ❌ Servidor no inicializado.");
            return;
        }

        Debug.Log("[SERVER_MANAGER] 🟢 Iniciando juego para todos los jugadores...");

        // Crear mensaje inicial de inicio de juego
        TurnInfo startMessage = new TurnInfo();
        startMessage.startGame = true;
        startMessage.actionType = "startGame";

        // Asignar listas del GameManager actual
        startMessage.playersList = GameManager.Instance.playersList;
        startMessage.territoriesList = GameManager.Instance.territoriesList;

        // Enviar mensaje de inicio a todos los clientes
        await server.BroadcastMessage(startMessage, null);
        Debug.Log("[SERVER_MANAGER] ✅ Mensaje de inicio enviado a los clientes.");

        // Cargar escena localmente para el host
        GameManager.Instance.StartGame(startMessage);
        Debug.Log("[SERVER_MANAGER] 🗺️ Escena 'Game' cargada localmente.");
    }

    // 🔹 Obtener IP local
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

    // 🔹 Obtener cantidad de jugadores conectados
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
