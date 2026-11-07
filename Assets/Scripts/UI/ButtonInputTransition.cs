using UnityEngine;
using UnityEngine.UI;

public class SceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    [SerializeField] private bool isOverlay = false;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        button.onClick.AddListener(OnPressed);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(OnPressed);
    }

    private void OnPressed()
    {
        if (SceneTransitionManager.Instance == null)
        {
            Debug.LogWarning("SceneButton on '" + gameObject.name + "' cannot transition to scene '" + sceneName + "' because the scene name is invalid");
            return;
        }

        if (isOverlay) { SceneTransitionManager.Instance.OpenOverlayScene(sceneName); }
        else { SceneTransitionManager.Instance.ChangeScene(sceneName); }
    }

}
