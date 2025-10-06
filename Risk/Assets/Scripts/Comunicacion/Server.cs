using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Server
{
    private TcpListener listener;
    private bool isRunning = false;
    private CancellationTokenSource cts;

    // Lista de jugadores y sockets separados
    public LinkedList<PlayerInfo> players = new LinkedList<PlayerInfo>();
    public  Dictionary<PlayerInfo, TcpClient> clientSockets = new Dictionary<PlayerInfo, TcpClient>();

    public async Task StartServer(int port)
    {
        if (isRunning) return;

        try
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            isRunning = true;
            cts = new CancellationTokenSource();

            _ = AcceptLoopAsync(cts.Token);

            Debug.Log($"Servidor escuchando en puerto {port}");
            await Task.CompletedTask;
        }
        catch (SocketException se)
        {
            isRunning = false;
            Debug.LogError($"No se pudo iniciar el servidor en el puerto {port}: {se.Message}");
            throw;
        }
    }

    private async Task AcceptLoopAsync(CancellationToken token)
    {
        try
        {
            while (isRunning && !token.IsCancellationRequested)
            {
                if (players.Count() >= 3)
                {
                    await Task.Delay(250, token);
                    continue;
                }

                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _ = HandleClient(client);
            }
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            Debug.LogWarning($"AcceptLoop terminó con error: {ex.Message}");
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        PlayerInfo player = null;

        try
        {
            // Primer mensaje: username
            int firstRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (firstRead <= 0) return;

            string username = Encoding.UTF8.GetString(buffer, 0, firstRead).Trim();
            player = new PlayerInfo(username);
            players.Add(player);
            clientSockets[player] = client;

            Debug.Log($"Jugador conectado: {username} | Total: {players.Count()}");

            while (client.Connected)
            {
                // Leer longitud
                byte[] lengthBuffer = new byte[4];
                int readLen = await stream.ReadAsync(lengthBuffer, 0, 4);
                if (readLen == 0) break;

                int messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                byte[] messageBuffer = new byte[messageLength];
                int totalRead = 0;

                while (totalRead < messageLength)
                {
                    int bytesRead = await stream.ReadAsync(messageBuffer, totalRead, messageLength - totalRead);
                    if (bytesRead == 0) break;
                    totalRead += bytesRead;
                }

                string json = Encoding.UTF8.GetString(messageBuffer);
                TurnInfo receivedAction = JsonConvert.DeserializeObject<TurnInfo>(json);

                await BroadcastMessage(receivedAction, player);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error con cliente {player?.username ?? "?"}: {ex.Message}");
        }
        finally
        {
            if (player != null)
            {
                players.Remove(player);
                clientSockets.Remove(player);
                Debug.Log($"Jugador desconectado: {player.username} | Total: {players.Count()}");
            }

            try { client.Close(); } catch { }
        }
    }

    private async Task BroadcastMessage(TurnInfo action, PlayerInfo sender)
    {
        // ✅ Preparar las estructuras para envío
        action.PrepareForSend();

        string json = JsonUtility.ToJson(action);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        foreach (var kvp in clientSockets)
        {
            PlayerInfo p = kvp.Key;
            TcpClient client = kvp.Value;

            if (p == sender || !client.Connected) continue;

            try
            {
                NetworkStream s = client.GetStream();
                await s.WriteAsync(lengthBytes, 0, lengthBytes.Length);
                await s.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error enviando mensaje a {p.username}: {ex.Message}");
            }
        }
    }

    public void StopServer()
    {
        if (!isRunning) return;

        isRunning = false;
        try { cts?.Cancel(); } catch { }
        try { listener?.Stop(); } catch { }

        foreach (var kvp in clientSockets)
        {
            try { kvp.Value.Close(); } catch { }
        }

        clientSockets.Clear();
        players.Clear();

        Debug.Log("Servidor cerrado.");
    }
}
