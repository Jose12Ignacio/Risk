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
    // 🔹 CONECTAR AL SERVIDOR
    // ===============================
    public async Task Connect(string ip, int port)
    {
        try
        {
            await client.ConnectAsync(ip, port); // Esperar conexión TCP
            stream = client.GetStream();
            await Task.Delay(100);

            Debug.Log($"[CLIENT] ✅ Conectado al servidor en {ip}:{port}");
            Debug.Log($"[CLIENT] Jugador: {playerName}");

            // Enviar el nombre del jugador al servidor
            byte[] nameBytes = Encoding.UTF8.GetBytes(playerName);
            await stream.WriteAsync(nameBytes, 0, nameBytes.Length);
            Debug.Log("[CLIENT] 📤 Nombre de jugador enviado al servidor.");

            OnConnected?.Invoke();

            // Comienza a escuchar mensajes
            _ = ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError($"❌ [CLIENT] No se pudo conectar: {ex.Message}");
            OnConnectionError?.Invoke(ex.Message);
        }
    }

    // ===============================
    // 🔹 ENVIAR MENSAJE (TurnInfo)
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
            string json = action.ToJson();
            byte[] data = Encoding.UTF8.GetBytes(json);

            await stream.WriteAsync(data, 0, data.Length);
            Debug.Log($"[CLIENT] 📤 Acción enviada correctamente: {action.actionType}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[CLIENT] ❌ Error enviando mensaje: {ex.Message}");
        }
    }

    // ===============================
    // 🔹 RECIBIR MENSAJES DEL SERVIDOR
    // ===============================
    private async Task ReceiveMessages()
    {
        byte[] buffer = new byte[8192]; // Buffer grande para mensajes largos

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.LogWarning("[CLIENT] ⚠️ El servidor cerró la conexión.");
                    break;
                }

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Debug.LogWarning("[CLIENT] ⚠️ JSON vacío recibido. Ignorando mensaje.");
                    continue;
                }

                TurnInfo receivedAction = null;
                try
                {
                    receivedAction = TurnInfo.FromJson(json);
                }
                catch (Exception parseEx)
                {
                    Debug.LogWarning($"[CLIENT] ⚠️ Error parseando JSON: {parseEx.Message}\nContenido:\n{json}");
                    continue;
                }

                if (receivedAction == null)
                {
                    Debug.LogWarning("[CLIENT] ⚠️ TurnInfo resultó nulo tras parsear JSON.");
                    continue;
                }

                // Reconstruir listas internas
                try
                {
                    receivedAction.RebuildLinkedLists();
                }
                catch (Exception rebuildEx)
                {
                    Debug.LogWarning($"[CLIENT] ⚠️ Error reconstruyendo LinkedLists: {rebuildEx.Message}");
                }

                // Verificar GameManager
                if (GameManager.Instance == null)
                {
                    Debug.LogWarning("[CLIENT] ⚠️ GameManager.Instance no inicializado, ignorando mensaje recibido.");
                    continue;
                }

                // Procesar mensaje
                try
                {
                    GameManager.Instance.ManageMessages(receivedAction);
                    Debug.Log("[CLIENT] ✅ Mensaje recibido y procesado correctamente.");
                }
                catch (Exception manageEx)
                {
                    Debug.LogError($"[CLIENT] ❌ Error procesando mensaje: {manageEx.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[CLIENT] ⚠️ Error recibiendo mensaje: {ex.Message}");
        }
        finally
        {
            try { client?.Close(); } catch { }
            Debug.Log("[CLIENT] 🔴 Conexión cerrada. Volviendo a la escena de Login...");
            SceneManager.LoadScene("Login");
        }
    }
}
