using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("Fullscreen"))
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        if (!PlayerPrefs.HasKey("ResolutionHeight"))
        {
            PlayerPrefs.SetInt("ResolutionHeight", Screen.height);
        }
        if (!PlayerPrefs.HasKey("ResolutionWidth"))
        {
            PlayerPrefs.SetInt("ResolutionWidth", Screen.width);
        }

        Screen.SetResolution(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"), PlayerPrefs.GetInt("Fullscreen") == 1);
    }

    public void Options()
    {
        SceneManager.LoadScene("OptionsMenu");
    }
}
