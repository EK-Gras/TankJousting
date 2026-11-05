using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionManager : MonoBehaviour
{
    
    public static SceneTransitionManager Instance { get; private set; }

    // signal exiting_scene / loading_progress(float) / entered_scene / transition_finished

    public event Action ExitingScene;
    public event Action EnteredScene;
    public event Action TransitionFinished;

    enum State { Idle, Covering, Loading, Swapping, Revealing }

    public readonly List<Scene> overlayScenes = new List<Scene>();
    private State state = State.Idle;
    private int _finishCount;

    public bool isTransitioning => state != State.Idle;

    /*
    =====
    SETUP
    =====
    */

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /*
    ========================
    OVERLAY SCENE MANAGEMENT
    ========================
    */

    // Usage: yield return SceneTransitionManager.Instance.OpenOverlayScene("sceneName", metadata, scene => { ... });
    public void OpenOverlayScene(string sceneName)
    {
        if (isTransitioning) { return; }

        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"Scene '{sceneName}' cannot be loaded. Make sure it is added to the build settings.");
            return;
        }

        StartCoroutine(OpenOverlayRoutine(sceneName));

    }

    private IEnumerator OpenOverlayRoutine(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene overlay = SceneManager.GetSceneByName(sceneName);
        overlayScenes.Add(overlay);
    }

    public bool CloseOverlayScene()
    {
        if (overlayScenes.Count == 0)
        {
            return false;
        }
        return CloseOverlayScene(overlayScenes[overlayScenes.Count - 1]);
    }

    public bool CloseOverlayScene(Scene target)
    {
        if (!overlayScenes.Remove(target)) { return false; }
        if (target.isLoaded){ SceneManager.UnloadSceneAsync(target); }
        return true;
    }

    /*
    ======
    PUBLIC
    ======
    */

    public bool ChangeScene(string sceneName)
    {
        return Play(TransitionPayload.Fade(sceneName));
    }

    // Use for custom play time
    public bool Play(TransitionPayload payload)
    {
        if (isTransitioning) {
            Debug.LogWarning("Scene transition already in progress. Ignoring new request.");
            return false;
        }

        if (!Application.CanStreamedLevelBeLoaded(payload.sceneName))
        {
            Debug.LogError($"Scene '{payload.sceneName}' cannot be loaded. Make sure it is added to the build settings.");
            return false;
        }

        state = State.Covering;
        StartCoroutine(Run(payload));
        return true;
    }

    /*
    ==============
    TRANSITION RUN
    ==============
    */

    private IEnumerator Run(TransitionPayload payload)
    {
        ExitingScene?.Invoke();
        
        // Start fade
        payload.StartFade();
        yield return payload.Cover();

        // Swap scene after fade is complete
        state = State.Loading;
        SceneManager.LoadScene(payload.sceneName);

        // Loading a normal scene unloads everything else, so we clear any overlays
        overlayScenes.Clear();

        // Wait a frame so the scene is fully loaded before we start revealing
        yield return null;

        // Fade back in
        state = State.Revealing;
        yield return payload.Reveal();

        payload.EndFade();
        state = State.Idle;

        EnteredScene?.Invoke();
        TransitionFinished?.Invoke();
    }
}