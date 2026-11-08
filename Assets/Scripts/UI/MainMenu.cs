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
        if (PlayerPrefs.HasKey("ResolutionHeight"))
        {
            PlayerPrefs.SetInt("ResolutionHeight", 1080);
        }
        if (PlayerPrefs.HasKey("ResolutionWidth"))
        {
            PlayerPrefs.SetInt("ResolutionWidth", 1920);
        }
    }

    public void Options()
    {
        SceneManager.LoadScene("OptionsMenu");
    }
}
