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
            Debug.Log("Conectado al servidor");

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
    public async Task SendAction(TurnInfo action) //Va a enviar la jugada que el cliente haga
    {
        if (stream != null) //Verificamos que stream exista, si no el servidor se puede romper
        {
            try
            {
                string json = JsonUtility.ToJson(action); //Convertimos el objeto TurnInfo a Json, luego a bytes
                byte[] data = Encoding.UTF8.GetBytes(json);
                await stream.WriteAsync(data, 0, data.Length); //Mandamos esos bytes
                Debug.Log("Enviando");
                
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error enviando mensaje: {ex.Message}");
            }
        }
    }

    // Recibir mensajes del servidor
    private async Task ReceiveMessages() //Recibe los mensajes que el servidor envia
    {
        byte[] buffer = new byte[1024];
        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); //Espera a recibir información
                if (bytesRead == 0) break; //Si es cero es server terminó, sale del bucle

                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead); //Convertir de bytes a Json y a TurnInfo
                TurnInfo receivedAction = JsonUtility.FromJson<TurnInfo>(json);

                GameManager.Instance.ManageMessages(receivedAction);

                Debug.Log($"Mensaje recibido");
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