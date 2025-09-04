using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

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

    public void SetPlayerData()
    {

        if (string.IsNullOrWhiteSpace(inputUsername.text) || string.IsNullOrWhiteSpace(inputIp.text))
        {
            Debug.Log("Ip o nombre invalido");
            if (string.IsNullOrWhiteSpace(inputUsername.text))
            {
                ShowInputError(inputUsername);
            }
            if (string.IsNullOrWhiteSpace(inputIp.text))
            {
                ShowInputError(inputIp);
            }
        }
        else
        {
            User_info.username = inputUsername.text;
            User_info.ip = inputIp.text;
            Debug.Log(User_info.username);
            Debug.Log(User_info.ip);
            Debug.Log("Informacion seteada");
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
