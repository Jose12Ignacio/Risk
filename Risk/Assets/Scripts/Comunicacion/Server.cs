using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Server
{
    private TcpListener listener;
    public LinkedList<PlayerInfo> clients = new LinkedList<PlayerInfo>();
    public LinkedList<PlayerInfo> players => clients;

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

            Debug.Log($"[SERVER] Escuchando en puerto {port}");
            _ = AcceptLoopAsync(cts.Token);

            await Task.CompletedTask;
        }
        catch (SocketException se)
        {
            isRunning = false;
            Debug.LogError($"[SERVER] No se pudo iniciar en puerto {port}: {se.Message}");
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
                _ = HandleClient(client);
            }
        }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SERVER] AcceptLoop terminó con error: {ex.Message}");
        }
    }

    private async Task HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        StringBuilder sb = new StringBuilder();
        byte[] buffer = new byte[8192];
        PlayerInfo player = null;

        try
        {
            int firstRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (firstRead <= 0) return;

            string username = Encoding.UTF8.GetString(buffer, 0, firstRead).Trim();
            if (username.EndsWith("\n"))
                username = username.Substring(0, username.Length - 1);

            player = new PlayerInfo(client, username);
            clients.Add(player);

            Debug.Log($"[SERVER] Jugador conectado: {username} | Total: {clients.Count()}");

            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;

                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                string allData = sb.ToString();
                int newlineIndex;
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
                            Debug.LogWarning($"[SERVER] JSON inválido de {player.username}, ignorado.");
                            continue;
                        }

                        receivedAction.RebuildLinkedLists();
                        Debug.Log($"[SERVER] Acción recibida de {player.username}: {receivedAction.actionType}");

                        await BroadcastMessage(receivedAction, player);
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"[SERVER] Error con cliente {player.username}: {ex.Message}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[SERVER] Error general con cliente {player?.username ?? "Desconocido"}: {ex.Message}");
        }
        finally
        {
            if (player != null)
            {
                clients.Remove(player);
                Debug.Log($"[SERVER] Jugador desconectado: {player.username} | Total: {clients.Count()}");
            }

            try { client.Close(); } catch { }
        }
    }

    public async Task BroadcastMessage(TurnInfo action, PlayerInfo sender)
    {
        try
        {
            action.PrepareForSend();
            string json = action.ToJson() + "\n";
            byte[] data = Encoding.UTF8.GetBytes(json);

            for (int i = 0; i < clients.Count(); i++)
            {
                PlayerInfo p = clients.Get(i);
                if (p == null || p == sender || p.client == null || !p.client.Connected)
                    continue;

                try
                {
                    NetworkStream s = p.client.GetStream();
                    await s.WriteAsync(data, 0, data.Length);
                }
                catch
                {
                    Debug.LogWarning($"[SERVER] No se pudo enviar mensaje a {p.username}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[SERVER] Error serializando TurnInfo: {ex.Message}");
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

        Debug.Log("[SERVER] Servidor cerrado.");
    }
}
