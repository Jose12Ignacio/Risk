using UnityEngine;

public class ClientManager : MonoBehaviour
{
    private Client localPlayer;

    public async void ConnectToServer() //Crear un cliente a parte de la clase, este será el jugador local
    {
        localPlayer = new Client(User_info.username);
        await localPlayer.Connect(User_info.ip, 5000); //Conecta al servidor, el puerto es fijo, pueden surgir errores por esto, sería bueno cambiarlo o hacer una validación

    }

    public async void SendMove(TurnInfo action) //Enviar información del movimiento hecho
    {
        if (localPlayer != null)
            await localPlayer.SendAction(action); //Llama a la función en si
    }

}
