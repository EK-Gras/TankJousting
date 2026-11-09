using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class CreditsUI : MonoBehaviour
{
    private VisualElement root;
    private VisualElement ReturnButton;

    void Start()
    {
        Debug.Log("Credits UI started");
        root = GetComponent<UIDocument>().rootVisualElement;
        ReturnButton = root.Q<VisualElement>("ReturnButton");

        ReturnButton.RegisterCallback<ClickEvent>(OnReturnButtonClicked);
    }

    private void OnReturnButtonClicked(ClickEvent evt)
    {
        Debug.Log("Return button clicked");
        SceneTransitionManager.Instance.ChangeScene("MainMenu");
    }
}
