using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cliente TCP que maneja la comunicación con el servidor.
/// Envía y recibe objetos TurnInfo serializados en JSON.
/// </summary>
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
        client = new TcpClient(); // Crear el socket TCP
    }

    // ===============================
    //  CONECTAR AL SERVIDOR
    // ===============================
    public async Task Connect(string ip, int port)
    {
        try
        {
            await client.ConnectAsync(ip, port); // Esperar conexión TCP
            stream = client.GetStream();
            await Task.Delay(100);

            Debug.Log($"[CLIENT] Conectado al servidor en {ip}:{port}");
            Debug.Log($"[CLIENT] Jugador: {playerName}");

            // Enviar el nombre del jugador al servidor
            byte[] nameBytes = Encoding.UTF8.GetBytes(playerName + "\n");
            await stream.WriteAsync(nameBytes, 0, nameBytes.Length);
            Debug.Log("[CLIENT]  Nombre de jugador enviado al servidor.");

            OnConnected?.Invoke();

            // Comienza a escuchar mensajes
            _ = ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($" [CLIENT] No se pudo conectar: {ex.Message}");
            OnConnectionError?.Invoke(ex.Message);
        }
    }

    // ===============================
    //  ENVIAR MENSAJE (TurnInfo)
    // ===============================
    public async Task SendAction(TurnInfo action)
    {
        if (stream == null)
        {
            Debug.LogWarning("[CLIENT] Stream no disponible, no se puede enviar mensaje.");
            return;
        }

        try
        {
            // Serializar el TurnInfo en JSON antes de enviar
            string json = action.ToJson() + "\n"; // 🔹 Agregamos el delimitador
            byte[] data = Encoding.UTF8.GetBytes(json);

            await stream.WriteAsync(data, 0, data.Length);
            Debug.Log($"[CLIENT]  Acción enviada correctamente: {action.actionType}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CLIENT]  Error enviando mensaje: {ex.Message}");
        }
    }

    // ===============================
    // 🔹 RECIBIR MENSAJES DEL SERVIDOR
    // ===============================
    private async Task ReceiveMessages()
    {
        byte[] buffer = new byte[8192];
        StringBuilder sb = new StringBuilder();

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.LogWarning("[CLIENT] El servidor cerró la conexión.");
                    break;
                }

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                string allData = sb.ToString();
                int newlineIndex;

                // Procesar todos los JSON delimitados por '\n'
                while ((newlineIndex = allData.IndexOf('\n')) >= 0)
                {
                    string oneJson = allData.Substring(0, newlineIndex).Trim();
                    allData = allData.Substring(newlineIndex + 1);
                    sb.Clear();
                    sb.Append(allData);

                    if (string.IsNullOrWhiteSpace(oneJson))
                        continue;

                    try
                    {
                        TurnInfo receivedAction = TurnInfo.FromJson(oneJson);
                        if (receivedAction == null)
                        {
                            Debug.LogWarning("[CLIENT]  JSON inválido recibido, ignorando...");
                            continue;
                        }

                        if (GameManager.Instance == null)
                        {
                            Debug.LogWarning("[CLIENT]  GameManager.Instance no inicializado.");
                            continue;
                        }

                        GameManager.Instance.ManageMessages(receivedAction);
                        Debug.Log($"[CLIENT] Acción recibida: {receivedAction.actionType}");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"[CLIENT]  Error procesando mensaje: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[CLIENT]  Error recibiendo mensaje: {ex.Message}");
        }
        finally
        {
            try { client?.Close(); } catch { }
            Debug.Log("[CLIENT]  Conexión cerrada. Volviendo a la escena de Login...");
            SceneManager.LoadScene("Login");
        }
    }
}
