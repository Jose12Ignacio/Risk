using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;

public class Login_manager : MonoBehaviour
{
    public TMP_InputField inputUsername;
    public TMP_InputField inputIp;
    public Color errorColor = Color.red;
    private Color defaultColor;
    public AudioSource audioSource;
    public AudioClip errorSound;

    public ClientManager clientManager;

    public ServerManager serverManager;

    void Start()
    {
        defaultColor = inputUsername.image.color;
    }

    public void OnClickSetClient()
    {
        _ = SetPlayerDataClientAsync(); // Llama a la versión async pero Unity ve un void
    }

    private async Task SetPlayerDataClientAsync()
    {
        User_info.username = inputUsername.text;
        User_info.ip = inputIp.text;
        clientManager.ConnectToServer();
    }


    public void OnClickSetServer()
    {
        _ = SetPlayerDataServerAsync(); // Unity ve un void, pero ejecuta la versión async
    }

// Método async que hace todo el trabajo
    private async Task SetPlayerDataServerAsync()
    {
        if (string.IsNullOrWhiteSpace(inputUsername.text))
        {
            ShowInputError(inputUsername); // Mostrar error visual
        }
        else
        {
            User_info.username = inputUsername.text;
            User_info.ip = inputIp.text;

            Debug.Log(User_info.username);
            Debug.Log(User_info.ip);
            Debug.Log("Información seteada");

            // Llamada async al server manager
            if (serverManager != null)
            {
                serverManager.StartServerAndLocalPlayer(); // Debe ser async Task
            }
        }
    }
    

    public void ShowInputError(TMP_InputField field)
    {
        StartCoroutine(InputErrorCoroutine(field));
    }

    private IEnumerator InputErrorCoroutine(TMP_InputField field)
        {
            Image bg = field.image;
            Color original = bg.color;

            bg.color = errorColor;  
            audioSource.PlayOneShot(errorSound);
            yield return new WaitForSeconds(2);
            bg.color = original;               
        }
}
