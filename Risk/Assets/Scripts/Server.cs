using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class Server
{
    private TcpListener listener;
    private List<TcpClient> clients = new List<TcpClient>(); //Hay que cambiar esto, crear la lista propia
    private bool isRunning = false;

    public async Task StartServer(int port) //Acá se inicia el server, aclarar que aquí el host del server aún no esta en el juego, ocupa crear su propio client
    //Lo que hace async Task es crear hilos, correr código en un segundo plano sin que se congele lo demás
    {
        listener = new TcpListener(IPAddress.Any, port); //Crear el servidor e inciar
        listener.Start();
        isRunning = true;

        Debug.Log("Servidor esperando clientes");

        _ = Task.Run(async () => //Acá corremos este código en segundo plano
        {
            while (isRunning) //Acepta clientes, siempre debe estar corriendo
            {
                if (clients.Count < 3) //Si hay menos de 3 jugadores
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(); //Espera a que alguien se una y crea su objeto cliente
                    clients.Add(client);

                    Debug.Log("Nuevo cliente conectado.");

                    _ = HandleClient(client); //Función que maneja al cliente
                }
                else
                {
                    await Task.Delay(500);
                }
            }
        });
    }

    private async Task HandleClient(TcpClient client) //Va a gestionar los mensajes que lleguen del cliente para enciarlos a otros
    {
        NetworkStream stream = client.GetStream(); //Este es el canal por donde se transmiten datos
        byte[] buffer = new byte[1024];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length); //Recibe un arreglo de bytes del cliente, tal vez tengamos que cambiar esto ya que es una lista
                if (bytesRead == 0) break; //Si llega un 0 significa que el cliente se desconectó, entonces sale del bucle

                //Convertir bytes a un string
                string json = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);

                //Convertir el string, que es un JSON, a un objeto de la información del turno.
                TurnInfo receivedAction = JsonUtility.FromJson<TurnInfo>(json);

                Debug.Log($"Jugador: {receivedAction.playerName}, Acción: {receivedAction.actionType}, De: {receivedAction.fromTerritory}, A: {receivedAction.toTerritory}, Tropas: {receivedAction.troops}");

                //Reenviar a los demás jugadores
                await BroadcastMessage(receivedAction, client); //Espera a recibir un mensaje
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error con cliente: {ex.Message}");
        }
        finally //Cuando el bucle de esta función termina se desconecta
        {
            clients.Remove(client);
            client.Close();
            Debug.Log($"Cliente desconectado. Total: {clients.Count}");
        }
    }

    private async Task BroadcastMessage(TurnInfo action, TcpClient sender)
    {
        string json = JsonUtility.ToJson(action); //Convertir el objeto de TurnInfo a JSON
        byte[] data = System.Text.Encoding.UTF8.GetBytes(json); //Luego a cadena de bytes para que pueda enviarse

        foreach (var client in clients) 
        {
            if (client != sender) //Evita que se envie al emisor.
            { 
                try
                {
                    NetworkStream stream = client.GetStream(); //Busca el canal del respectivo cliente
                    await stream.WriteAsync(data, 0, data.Length); //Espera a que se envie
                }
                catch
                {
                    // Ignorar clientes desconectados
                }
            }
        }
    }

    public void StopServer() //Limpia todo una vez el server cierra
    {
        isRunning = false; //Detiene el bucle del server

        foreach (var client in clients)
        {
            client.Close(); //Eliminar todos los clientes
        }
        clients.Clear();

        listener.Stop();//Cerrar el servidor
        Debug.Log("Servidor cerrado.");
    }
}