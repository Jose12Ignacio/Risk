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
        defaultColor = inputUsername.image != null ? inputUsername.image.color : Color.white;
    }

    public void OnClickSetClient()
    {
        _ = SetPlayerDataClientAsync();
    }

    private async Task SetPlayerDataClientAsync()
    {
        await Task.Yield();

        var nombre = inputUsername.text?.Trim();

        if (string.IsNullOrEmpty(nombre))
        {
            ShowInputError(inputUsername);
            return;
        }

        User_info.username = nombre;
        User_info.ip       = inputIp.text;
        User_info.manager  = false;
        

        if (GameManager.Instance != null && GameManager.Instance.clientManager != null)
        {
            GameManager.Instance.clientManager.ConnectToServer(nombre, inputIp.text);
        }
        else
        {
            Debug.LogError("GameManager.Instance o clientManager es null.");
            ShowInputError(inputUsername);
        }
    }

    public void OnClickSetServer()
    {
        _ = SetPlayerDataServerAsync();
    }

    private async Task SetPlayerDataServerAsync()
    {
        await Task.Yield();

        var nombre = inputUsername.text?.Trim();
        if (string.IsNullOrEmpty(nombre))
        {
            ShowInputError(inputUsername);
            return;
        }

        User_info.username = nombre;
        User_info.ip       = inputIp.text;
        User_info.manager  = true;
        
        if (GameManager.Instance != null && GameManager.Instance.serverManager != null)
        {
            GameManager.Instance.serverManager.StartServerAndLocalPlayer();
            // Cuando el server esté listo, pasa a la sala
            await Task.Delay(100);
            SceneManager.LoadScene("GameRoom");
        }
        else
        {
            Debug.LogError("GameManager.Instance o serverManager es null.");
            ShowInputError(inputUsername);
        }
    }

    public void ShowInputError(TMP_InputField field)
    {
        StartCoroutine(InputErrorCoroutine(field));
    }

    private IEnumerator InputErrorCoroutine(TMP_InputField field)
    {
        var bg = field.image;
        var original = bg != null ? bg.color : Color.white;

        if (bg != null) bg.color = errorColor;
        if (audioSource != null && errorSound != null)
            audioSource.PlayOneShot(errorSound);

        yield return new WaitForSeconds(2);
        if (bg != null) bg.color = original;
    }
}
