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

    public void StartServerAndLocalPlayer()
    {
        ip = GetLocalIPAddress();
        if (System.Net.IPAddress.TryParse(ip, out _))
        {
            server = new Server();
            _ = server.StartServer(port);

            playerName = User_info.username;

            GameManager.Instance.clientManager.ConnectToServer(playerName, ip);
        }
    }

    // Enviar un movimiento desde el host


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
        return server.players.Count();
    }

}

