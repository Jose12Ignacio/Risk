using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Login_manager : MonoBehaviour
{
    public TMP_InputField username;
    public TMP_InputField ip;


    public void SetPlayerData()
    {

        if (string.IsNullOrWhiteSpace(username.text))
        {
            Debug.Log("Ip o nombre invalido");
        }
        else
        {
            User_info.username = username.text;
            User_info.ip = ip.text;
            Debug.Log(User_info.username);
            Debug.Log(User_info.ip);
            Debug.Log("Informacion seteada");

            SceneManager.LoadScene("Game_room");
        }
    }
}
