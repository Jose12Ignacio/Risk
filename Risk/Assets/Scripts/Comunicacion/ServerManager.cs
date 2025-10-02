using UnityEngine;
using System.Net;
using System.Net.Sockets;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;


public class ServerManager : MonoBehaviour //Creamos esta clase porque el script de server no está integrado en Unity, es C# puro
{
    public string playerName;
    public TextMeshProUGUI playersNumber;
    public string ip;
    public static int port = 5000;
    public Server server;
    private Client localPlayer;

    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public async void StartServerAndLocalPlayer()
    {
        ip = GetLocalIPAddress();
        if (System.Net.IPAddress.TryParse(ip, out _))
        {
            server = new Server();
            _ = server.StartServer(port);

            playerName = User_info.username;

            localPlayer = new Client(playerName);
            await localPlayer.Connect(ip, port);

            Debug.Log("Cliente local conectado al servidor");
        }
    }

    // Enviar un movimiento desde el host
    public async void SendHostMove(TurnInfo action)
    {
        if (localPlayer != null)
            await localPlayer.SendAction(action);
    }

    public static string GetLocalIPAddress() //Funcion para obtener la ip como string
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
        return server.clients.Count();
    }

}

