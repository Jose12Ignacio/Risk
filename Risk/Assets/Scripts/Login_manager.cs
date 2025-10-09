using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Login_manager : MonoBehaviour
{
    // Campos de entrada para el nombre de usuario y la dirección IP.
    public TMP_InputField inputUsername;
    public TMP_InputField inputIp;

    // Colores para indicar error en el campo de texto.
    public Color errorColor = Color.red;   // Color usado cuando hay un error.
    private Color defaultColor;            // Color original del campo, guardado para restaurarlo después.

    // Sonido de error opcional.
    public AudioSource audioSource;        // Fuente de audio.
    public AudioClip errorSound;           // Clip de sonido reproducido al haber error.

    void Start()
    {
        // Guarda el color original del campo de texto (si tiene imagen de fondo).
        defaultColor = inputUsername.image != null ? inputUsername.image.color : Color.white;
    }

   
    // Evento: clic en el botón "Cliente"
    // Se ejecuta cuando el usuario selecciona unirse como cliente.
    public void OnClickSetClient()
    {
        _ = SetPlayerDataClientAsync(); // Llama a la tarea asíncrona sin bloquear el hilo principal.
    }

    // Lógica asíncrona para configurar el cliente.
    private async Task SetPlayerDataClientAsync()
    {
        // Cede un ciclo de ejecución para evitar bloqueos.
        await Task.Yield();

        // Obtiene y limpia el texto del campo de nombre.
        var nombre = inputUsername.text?.Trim();

        // Si el campo está vacío, muestra error visual y detiene el proceso.
        if (string.IsNullOrEmpty(nombre))
        {
            ShowInputError(inputUsername);
            return;
        }

        // Guarda los datos del usuario en la clase estática User_info.
        User_info.username = nombre;
        User_info.ip       = inputIp.text;
        User_info.manager  = false; // false indica que este usuario no es el servidor.

        // Verifica si el GameManager y su ClientManager existen antes de conectarse.
        if (GameManager.Instance != null && GameManager.Instance.clientManager != null)
        {
            // Llama al método de conexión al servidor usando el nombre e IP.
            GameManager.Instance.clientManager.ConnectToServer(nombre, inputIp.text);
        }
        else
        {
            // Si no existen las referencias necesarias, muestra error en consola y visualmente.
            Debug.LogError("GameManager.Instance o clientManager es null.");
            ShowInputError(inputUsername);
        }
    }


    // Evento: clic en el botón "Servidor"
    // Se ejecuta cuando el usuario elige crear una partida como servidor.
    public void OnClickSetServer()
    {
        _ = SetPlayerDataServerAsync(); // Llama al método asíncrono para iniciar el servidor.
    }

    // Lógica asíncrona para configurar el servidor.
    private async Task SetPlayerDataServerAsync()
    {
        await Task.Yield();

        // Obtiene el nombre de usuario ingresado.
        var nombre = inputUsername.text?.Trim();

        // Valida que no esté vacío.
        if (string.IsNullOrEmpty(nombre))
        {
            ShowInputError(inputUsername);
            return;
        }

        // Guarda la información global del usuario.
        User_info.username = nombre;
        User_info.ip       = inputIp.text;
        User_info.manager  = true; // true indica que este jugador es el anfitrión/servidor.

        // Verifica que el GameManager y su ServerManager existan antes de iniciar.
        if (GameManager.Instance != null && GameManager.Instance.serverManager != null)
        {
            // Inicia el servidor y crea al jugador local.
            GameManager.Instance.serverManager.StartServerAndLocalPlayer();

            // Espera brevemente para asegurar que el servidor esté listo antes de cambiar de escena.
            await Task.Delay(100);

            // Cambia a la escena "GameRoom" una vez el servidor está configurado.
            SceneManager.LoadScene("GameRoom");
        }
        else
        {
            // Si no hay referencias válidas, muestra error en consola y visualmente.
            Debug.LogError("GameManager.Instance o serverManager es null.");
            ShowInputError(inputUsername);
        }
    }

    // Indicador visual de error
  
    // Muestra un cambio de color temporal en el campo de entrada y reproduce un sonido de error.
    public void ShowInputError(TMP_InputField field)
    {
        StartCoroutine(InputErrorCoroutine(field)); // Llama a la corrutina para manejar el efecto visual.
    }

    // Corrutina que cambia el color del campo por 2 segundos y luego lo restaura.
    private IEnumerator InputErrorCoroutine(TMP_InputField field)
    {
        var bg = field.image;                        // Imagen de fondo del campo.
        var original = bg != null ? bg.color : Color.white; // Guarda el color original.

        // Cambia el color a rojo (u otro color definido) para indicar error.
        if (bg != null) bg.color = errorColor;

        // Si existe un sonido de error, se reproduce una vez.
        if (audioSource != null && errorSound != null)
            audioSource.PlayOneShot(errorSound);

        // Espera 2 segundos antes de restaurar el color original.
        yield return new WaitForSeconds(2);
        if (bg != null) bg.color = original;
    }
}
