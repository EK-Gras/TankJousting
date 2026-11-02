using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D playerRef;

    [Header("Settings")]
    [SerializeField] float maxSize = 3f;
    [SerializeField] float minSize = 1f;
    [SerializeField] private float currentSize;
    [SerializeField] private Vector3 originSize;
    [SerializeField] private Vector2 currentPos;
    
    private void Start()
    {
        playerRef = GameManager.Instance.turnManager.playerRef.GetComponent<Rigidbody2D>();
        originSize = transform.localScale;
    }

    private void FixedUpdate()
    {
        ScaleBySpeed();
    }

    public void ScaleBySpeed()
    {
        // first, take the size and modify it by player's speed
        currentSize = Mathf.Clamp(playerRef.linearVelocity.magnitude, minSize, maxSize);
        transform.localScale = new Vector3(originSize.x, originSize.y * currentSize, originSize.z);
        
        // then, take midway point of y scale to find anchor point
        float anchorY = transform.localScale.y / 2;
        transform.localPosition = new Vector3(0, anchorY, 0);
    }
}
