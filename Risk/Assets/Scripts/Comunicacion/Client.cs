
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using CrazyRisk.Core;

public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    public event Action OnConnected;
    public event Action<string> OnConnectionError;

    public string playerName;

    public Client(string playerName)
    {
        this.playerName = playerName;
        client = new TcpClient();
    }

    public async Task Connect(string ip, int port)
    {
        try
        {
            await client.ConnectAsync(ip, port);
            stream = client.GetStream();
            await Task.Delay(100);

            Debug.Log($"Conectado como {playerName}");

            byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
            await stream.WriteAsync(nameBytes, 0, nameBytes.Length);

            OnConnected?.Invoke();

            _ = ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"No se pudo conectar: {ex.Message}");
            OnConnectionError?.Invoke(ex.Message);
        }
    }

    public async Task SendAction(TurnInfo action)
    {
        if (stream == null)
        {
            Debug.LogWarning("No hay stream disponible para enviar mensajes.");
            return;
        }

        try
        {
            string json = JsonConvert.SerializeObject(action); // ✅ Newtonsoft
            byte[] data = Encoding.UTF8.GetBytes(json);

            // Enviar longitud primero
            byte[] lengthBytes = BitConverter.GetBytes(data.Length);
            await stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await stream.WriteAsync(data, 0, data.Length);

            Debug.Log($"Enviado: {json}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error enviando mensaje: {ex.Message}");
        }
    }

private async Task ReceiveMessages()
{
    try
    {
        while (client.Connected)
        {
            byte[] lengthBuffer = new byte[4];
            await ReadExactlyAsync(lengthBuffer, 4);
            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            byte[] messageBuffer = new byte[messageLength];
            await ReadExactlyAsync(messageBuffer, messageLength);

            string json = Encoding.UTF8.GetString(messageBuffer);

            // Configuración de deserialización con converters
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new LinkedListConverter<PlayerInfo>());
            settings.Converters.Add(new LinkedListConverter<Territorio>());

            TurnInfo receivedAction = JsonConvert.DeserializeObject<TurnInfo>(json, settings);

            if (receivedAction == null)
            {
                Debug.LogWarning("Mensaje JSON inválido recibido");
                continue;
            }

            Debug.Log("Mensaje recibido (Newtonsoft): " + json);

            // Guardar color propio si el mensaje contiene info de jugadores
            var me = FindPlayer(playerName, receivedAction.playersList);
            if (me != null && !string.IsNullOrEmpty(me.color))
            {
                User_info.color = me.color;
                Debug.Log($"Mi color asignado: {User_info.color}");
            }

            GameManager.Instance.ManageMessages(receivedAction);
        }
    }
    catch (Exception ex)
    {
        Debug.LogWarning($"Error recibiendo mensaje: {ex.Message}");
    }
    finally
    {
        client.Close();
        SceneManager.LoadScene("Login");
        Debug.Log("Conexión cerrada");
    }
}


    // Método seguro para encontrar tu PlayerInfo
    private PlayerInfo? FindPlayer(string playerName, LinkedList<PlayerInfo> list)
    {
        if (list == null || list.Count() == 0) return null;

        for (int i = 0; i < list.Count(); i++)
        {
            var player = list.Get(i);
            if (player != null && !string.IsNullOrEmpty(player.username) && player.username == playerName)
                return player;
        }

        return null;
    }

    private async Task ReadExactlyAsync(byte[] buffer, int length)
    {
        int totalRead = 0;
        while (totalRead < length)
        {
            int bytesRead = await stream.ReadAsync(buffer, totalRead, length - totalRead);
            if (bytesRead == 0)
                throw new Exception("Conexión cerrada por el servidor");
            totalRead += bytesRead;
        }
    }
}
