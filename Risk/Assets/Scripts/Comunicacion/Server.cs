using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;


public class Server
{
    private TcpListener listener;
    public LinkedList<PlayerInfo> clients = new LinkedList<PlayerInfo>();
    private bool isRunning = false;
    private CancellationTokenSource cts;

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
                if (clients.Count() >= 3)
                {
                    await Task.Delay(250, token);
                    continue;
                }

                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                _ = HandleClient(client); // no agregamos aún a la lista, lo hace HandleClient
            }
        }
        catch (ObjectDisposedException)
        {
            // listener detenido
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"AcceptLoop terminó con error: {ex.Message}");
        }
    }

    public async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];

        PlayerInfo player = null;

        try
        {
            // Primer mensaje: username
            int firstRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (firstRead <= 0) return;

            string username = System.Text.Encoding.UTF8.GetString(buffer, 0, firstRead).Trim();
            player = new PlayerInfo(client, username);

            clients.Add(player);
            Debug.Log($"Jugador conectado: {username} | Total: {clients.Count()}");

            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                string json = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                TurnInfo receivedAction = JsonUtility.FromJson<TurnInfo>(json);

                Debug.Log($"Acción recibida de {player.username}: {receivedAction.actionType}");

                await BroadcastMessage(receivedAction, player);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error con cliente: {ex.Message}");
        }
        finally
        {
            if (player != null)
            {
                clients.Remove(player);
                Debug.Log($"Jugador desconectado: {player.username} | Total: {clients.Count()}");
            }

            try { client.Close(); } catch { }
        }
    }

    private async Task BroadcastMessage(TurnInfo action, PlayerInfo sender)
    {
        string json = JsonUtility.ToJson(action);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        for (int i = 0; i < clients.Count(); i++)
        {
            PlayerInfo p = clients.Get(i);
            if (p == null || p == sender || p.client == null || !p.client.Connected) continue;

            try
            {
                NetworkStream s = p.client.GetStream();
                await s.WriteAsync(data, 0, data.Length);
            }
            catch
            {
                // ignorar clientes caídos
            }
        }
    }

    public void StopServer()
    {
        if (!isRunning) return;

        isRunning = false;
        try { cts?.Cancel(); } catch { }
        try { listener?.Stop(); } catch { }

        for (int i = 0; i < clients.Count(); i++)
        {
            try { clients.Get(i)?.client?.Close(); } catch { }
        }

        Debug.Log("Servidor cerrado.");
    }
}