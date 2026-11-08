using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TransitionPayload
{
    public string sceneName = "";
    public float coverTime = 0.3f;
    public float revealTime = 0.3f;

    private GameObject canvasObj;
    private CanvasGroup fadeGroup;

    /*
    ===============
    FADE TRANSITION
    ===============
    */

    public static TransitionPayload Fade(string sceneName, float coverTime = 0.3f, float revealTime = 0.3f)
    {
        TransitionPayload payload = new TransitionPayload();

        payload.sceneName = sceneName;
        payload.coverTime = coverTime;
        payload.revealTime = revealTime;

        return payload;
    }

    /*
    =======
    FADE UI
    =======
    */

    // Builds a black full-screen image on its own canvas, fully transparent from the start
    // Replaces _spawn_fade in godot
    public void StartFade()
    {
        canvasObj = new GameObject("TransitionFadeCanvas", typeof(RectTransform));

        UnityEngine.Object.DontDestroyOnLoad(canvasObj);

        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999;

        // Blocks clicks when transitioning
        canvasObj.AddComponent<GraphicRaycaster>();

        fadeGroup = canvasObj.AddComponent<CanvasGroup>();

        var imageObj = new GameObject("Fade", typeof(RectTransform));
        imageObj.transform.SetParent(canvasObj.transform, false);

        var image = imageObj.AddComponent<Image>();
        image.color = Color.black;
        image.raycastTarget = true;

        RectTransform imageRect = image.rectTransform;
        imageRect.anchorMin = Vector2.zero;
        imageRect.anchorMax = Vector2.one;
        imageRect.offsetMin = Vector2.zero;
        imageRect.offsetMax = Vector2.zero;

        SetFade(0f);
    }

    public void EndFade()
    {
        if (canvasObj != null)
        {
            UnityEngine.Object.Destroy(canvasObj);
        }
        canvasObj = null;
        fadeGroup = null;
    }

    public IEnumerator Cover()
    {
        return Animate(0f, 1f, coverTime);
    }

    public IEnumerator Reveal()
    {
        return Animate(1f, 0f, revealTime);
    }

    private IEnumerator Animate(float from, float to, float time)
    {
        SetFade(from);

        float elapsed = 0f;
        while (elapsed < time)
        {
            elapsed += Time.unscaledDeltaTime;
            SetFade(Mathf.Lerp(from, to, elapsed / time));
            yield return null;
        }

        SetFade(to);
    }

    private void SetFade(float alpha)
    {
        fadeGroup.alpha = alpha;   
    }
}
