using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneCanvasScalar : MonoBehaviour
{
    private CanvasScaler canvasScaler;
    
    private void Awake()
    {
        canvasScaler = GetComponent<CanvasScaler>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvasScaler.referenceResolution = new Vector2(PlayerPrefs.GetInt("ResolutionWidth"), PlayerPrefs.GetInt("ResolutionHeight"));
    }
}
