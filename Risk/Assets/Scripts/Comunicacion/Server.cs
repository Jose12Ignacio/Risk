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
    public LinkedList<TcpClient> clients = new LinkedList<TcpClient>();
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

            // Lanza el aceptador; no esperes aquí
            _ = AcceptLoopAsync(cts.Token);

            await Task.CompletedTask; // explícito: método async “completado” tras iniciar
            Debug.Log($"Servidor escuchando en puerto {port}");
        }
        catch (SocketException se)
        {
            isRunning = false;
            Debug.LogError($"No se pudo iniciar el servidor en el puerto {port}: {se.Message}");
            throw; // o maneja según tu flujo
        }
    }


    private async Task AcceptLoopAsync(CancellationToken token)
    {
        try
        {
            while (isRunning && !token.IsCancellationRequested)
            {
                // Limite de jugadores (3) — ajusta a tu gusto
                if (clients.Count() >= 3)
                {
                    await Task.Delay(250, token);
                    continue;
                }

                TcpClient client = await listener.AcceptTcpClientAsync().ConfigureAwait(false);

                // Agregar a la lista correctamente
                clients.Add(client);

                Debug.Log($"Cliente conectado. Total: {clients.Count()}");

                _ = HandleClient(client); // atender cliente en segundo plano
            }
        }
        catch (ObjectDisposedException)
        {
            // listener detenido — salir tranquilo
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"AcceptLoop terminó con error: {ex.Message}");
        }
    }

    
    public async Task HandleClient(TcpClient client)
    {
        using NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead <= 0) break;


                string json = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                TurnInfo receivedAction = JsonUtility.FromJson<TurnInfo>(json);


                // OJO: si TurnInfo no tiene playerName, usa otra propiedad
                Debug.Log($"Acción: {receivedAction.actionType} | De: {receivedAction.fromTerritory} -> {receivedAction.toTerritory} | Tropas: {receivedAction.troops}");

                await BroadcastMessage(receivedAction, client);
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error con cliente: {ex.Message}");
        }
        finally
        {
            clients.Remove(client);
            try { client.Close(); } catch { }
            Debug.Log($"Cliente desconectado. Total: {clients.Count()}");
        }
    }

    
    private async Task BroadcastMessage(TurnInfo action, TcpClient sender)
    {
        string json = JsonUtility.ToJson(action);
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json);

        for (int i = 0; i < clients.Count(); i++)
    {
        TcpClient c = clients.Get(i);
        if (c == null || c == sender || !c.Connected) continue;

        try
        {
            NetworkStream s = c.GetStream();
            await s.WriteAsync(data, 0, data.Length);
        }
        catch
        {
            // ignorar clientes caídos; los limpiará HandleClient/StopServer
        }
    }
    }


    public void StopServer()
{
    if (!isRunning) return;

    isRunning = false;
    try { cts?.Cancel(); } catch { }
    try { listener?.Stop(); } catch { }

    // cerrar clientes
    for (int i = 0; i < clients.Count(); i++)
    {
        try { clients.Get(i)?.Close(); } catch { }
    }

    Debug.Log("Servidor cerrado.");
}
}