using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;


[RequireComponent(typeof(UIDocument))]
public class MainMenu : MonoBehaviour
{
    private VisualElement root;
    private VisualElement StartButton;
    private VisualElement SettingsButton;
    private VisualElement CreditsButton;
    private VisualElement EndButton;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        root = GetComponent<UIDocument>().rootVisualElement;
        StartButton = root.Q<VisualElement>("StartButton");
        SettingsButton = root.Q<VisualElement>("SettingsButton");
        CreditsButton = root.Q<VisualElement>("CreditsButton");
        EndButton = root.Q<VisualElement>("EndButton");

        StartButton.RegisterCallback<ClickEvent>(ev => StartGame());
        SettingsButton.RegisterCallback<ClickEvent>(ev => Options());
        CreditsButton.RegisterCallback<ClickEvent>(ev => ShowCredits());
        EndButton.RegisterCallback<ClickEvent>(ev => QuitGame());

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
    public void StartGame()
    {
        SceneTransitionManager.Instance.ChangeScene("Arena");
    }

    public void Options()
    {
        SceneTransitionManager.Instance.ChangeScene("OptionsMenu");
    }

    public void ShowCredits()
    {
        SceneTransitionManager.Instance.ChangeScene("CreditsMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
