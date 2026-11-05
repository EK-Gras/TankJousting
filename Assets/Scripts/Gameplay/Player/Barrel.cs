using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D playerRef;
    [SerializeField] Transform barrelPivot;
    [SerializeField] Transform barrelEnd;
    [SerializeField] GameObject projectile;

    [Header("Settings")]
    [SerializeField] float maxSize = 3f;
    [SerializeField] float minSize = 1f;
    [SerializeField] private float scalingDivider = 10f;
    [SerializeField] private float currentSize;
    [SerializeField] private Vector3 originSize;
    [SerializeField] private Vector2 currentPos;
    
    /// <summary>
    /// Potential scaler for enemy delay before firing
    /// </summary>
    [Range(0f, 1f)] private float shotDelay = 0f;
    

    private void Start()
    {
        playerRef = GameManager.Instance.turnManager.playerRef.GetComponent<Rigidbody2D>();
        originSize = transform.localScale;
    }

    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Player"))
            ScaleBySpeed();
    }

    public void ScaleBySpeed()
    {
        // first, take the size and modify it by player's speed, divided by scaling divider
        currentSize = Mathf.Clamp(playerRef.linearVelocity.magnitude / scalingDivider, minSize, maxSize);
        transform.localScale = new Vector3(originSize.x, originSize.y * currentSize, originSize.z);
        
        // then, take midway point of y scale to find anchor point
        float anchorY = transform.localScale.y / 2;
        transform.localPosition = new Vector3(0, anchorY, 0);
    }

    public void ReadyFire()
    {
        StartCoroutine("FireShot");
    }

    private IEnumerator FireShot()
    {
        yield return new WaitForSeconds(shotDelay);
        Instantiate(projectile, barrelEnd.position, barrelPivot.rotation);
    }
}
