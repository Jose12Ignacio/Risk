using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        client = new TcpClient(); //Crear el objeto client
    }

    // Conectarse al servidor
    public async Task Connect(string ip, int port)
    {
        try
        {
            await client.ConnectAsync(ip, port); //Intenta y espera conectarse a la dirección dada
            stream = client.GetStream(); //Establece el canal de comunicación
            await Task.Delay(100);
            Debug.Log("Conectado");
            Debug.Log(playerName);
            
            byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
            await stream.WriteAsync(nameBytes, 0, nameBytes.Length);

            Debug.Log("Enviado");

            OnConnected?.Invoke();

            // Empezar a recibir mensajes
            _ = ReceiveMessages();
        }
        catch (Exception ex) //Avisar si hubo un error en la conexión
        {
            Debug.LogError($"No se pudo conectar: {ex.Message}");
            OnConnectionError?.Invoke(ex.Message);
        }
    }

    // Enviar un movimiento al servidor en el formaro de TurnInfo
    public async Task SendAction(TurnInfo action)
    {
        if (stream != null)
        {
            try
            {
                // ✅ Prepara los datos para enviar (convierte LinkedLists → arrays)
                action.PrepareForSend();

                string json = JsonUtility.ToJson(action);
                byte[] data = Encoding.UTF8.GetBytes(json);

                await stream.WriteAsync(data, 0, data.Length);
                Debug.Log("Enviando");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error enviando mensaje: {ex.Message}");
            }
        }
    }


    // Recibir mensajes del servidor
    private async Task ReceiveMessages()
    {
        byte[] buffer = new byte[1024];
        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                TurnInfo receivedAction = JsonUtility.FromJson<TurnInfo>(json);
                receivedAction.RebuildLinkedLists();

                // Guardar color propio si el mensaje contiene info de jugadores
                if (receivedAction.startGame && receivedAction.playersList != null)
                {
                    var current = receivedAction.playersList.head;
                    while (current != null)
                    {
                        if (current.data.username == playerName)
                        {
                            User_info.color = current.data.color;
                            Debug.Log($"Mi color asignado: {User_info.color}");
                            break;
                        }
                        current = current.next;
                    }
                }

                GameManager.Instance.ManageMessages(receivedAction);
                Debug.Log("Mensaje recibido");
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
}