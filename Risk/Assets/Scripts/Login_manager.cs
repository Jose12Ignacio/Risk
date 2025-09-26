using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class Login_manager : MonoBehaviour
{
    public TMP_InputField inputUsername;
    public TMP_InputField inputIp;
    public Color errorColor = Color.red;
    private Color defaultColor;
    public AudioSource audioSource;
    public AudioClip errorSound;
    


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
        User_info.manager = false;
        GameManager.Instance.clientManager.ConnectToServer(); //Establecer informacion del usuario que esta en los inputs
        
    }


    public void OnClickSetServer()
    {
        _ = SetPlayerDataServerAsync(); // Unity ve un void, pero ejecuta la versión async
        
    }

// Método async que hace todo el trabajo
    private async Task SetPlayerDataServerAsync()
    {
        Debug.Log("Intentando conectar");
        if (string.IsNullOrWhiteSpace(inputUsername.text))
        {
            ShowInputError(inputUsername); // Mostrar error visual
            Debug.Log("Error nombre");

        }
        else
        {
            User_info.username = inputUsername.text;
            User_info.ip = inputIp.text;
            User_info.manager = true;
            Debug.Log("Info");


            // Llamada async al server manager
            if (GameRoomManager.Instance != null)
            {
                Debug.Log("Instance existe");
                if (GameManager.Instance.serverManager != null)
                {
                    Debug.Log("Server existe");
                    GameManager.Instance.serverManager.StartServerAndLocalPlayer(); // Debe ser async Task
                }

            }

        }
    }
    

    public void ShowInputError(TMP_InputField field) //Cambia el sonido y color del input al tener un error.
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
