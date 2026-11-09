using System;
using System.Collections;
using Gameplay;
using UnityEngine;
using UnityEngine.Serialization;

public class Barrel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D playerRef;
    [SerializeField] BarrelPivot barrelPivot;
    [SerializeField] Transform barrelEnd;
    [SerializeField] GameObject projectile;
    [SerializeField] GameObject rocket;
    [SerializeField] GameObject nuke;

    [Header("Settings")]
    [SerializeField] float maxSize = 3f;
    [SerializeField] float minSize = 1f;
    [SerializeField] private float scalingDivider = 10f;
    [SerializeField] private float currentSize;
    [SerializeField] private Vector3 originSize;
    [SerializeField] private Vector2 currentPos;
    [SerializeField] public Vector3 sprayEndPoint;
    [SerializeField] private float shieldHitReboundStrength = 3f;

    private float startX;

    private void Start()
    {
        playerRef = GameManager.Instance.turnManager.playerRef.GetComponent<Rigidbody2D>();
        originSize = transform.localScale;
        startX = transform.localPosition.x;
    }

    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Player"))
            ScaleBySpeed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            print("Shield collided");
            // direction to push player in when colliding with shield
            Vector2 direction = other.transform.position - transform.position;
            float temp = playerRef.linearVelocity.x;
            // stop player momentum, then add force
            playerRef.linearVelocity = new Vector2(0, playerRef.linearVelocity.y);
            playerRef.AddForce(direction.normalized * shieldHitReboundStrength - new Vector2(temp,0), ForceMode2D.Impulse);
            // print(direction.normalized * shieldHitReboundStrength);
            // disable barrel damage just in case
            Hurtbox damage = GetComponent<Hurtbox>();
            damage.enabled = false;
            // barrel 
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            // direction to push player in when colliding with shield
            Vector2 direction = other.transform.position - transform.position;
            float temp = playerRef.linearVelocity.x;
            // stop player momentum, then add force
            playerRef.linearVelocity = new Vector2(0, playerRef.linearVelocity.y);
            // add increasing force to player until they are thrown out of shield radius
            playerRef.AddForce(direction.normalized * shieldHitReboundStrength - new Vector2(temp,0), ForceMode2D.Impulse);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            // can deal damage again
            Hurtbox damage = GetComponent<Hurtbox>();
            damage.enabled = true;
        }
    }

    public void ScaleBySpeed()
    {
        // first, take the size and modify it by player's speed, divided by scaling divider
        currentSize = Mathf.Clamp(playerRef.linearVelocity.magnitude / scalingDivider, minSize, maxSize);
        transform.localScale = new Vector3(originSize.x * currentSize, originSize.y, originSize.z);
        
        // then, take midway point of y scale to find anchor point
        float anchorX = transform.localScale.x / 2;
        transform.localPosition = new Vector3(anchorX + startX, 0, 0);
    }

    public void ReadyFire(Enemy.EnemyAction action, float time)
    {
        switch (action)
        {
            case Enemy.EnemyAction.ROCKET:
                Quaternion temp = new Quaternion(0,0,sprayEndPoint.z,0);
                barrelPivot.StartRotationTowards(temp);
                StartCoroutine(RocketSpread(time));
                break;
            case Enemy.EnemyAction.SHOOT:
                StartCoroutine(FireShot(time));
                break;
            case Enemy.EnemyAction.LASER:
                StartCoroutine(FireShot(time));
                break;
        }
    }

    private IEnumerator RocketSpread(float delay)
    {
        print(delay);
        float count = 0;
        while (count < delay * 5)
        {
            print(count);
            yield return new WaitForSeconds(delay);
            count+=delay;
            Instantiate(rocket, barrelEnd.position, barrelPivot.gameObject.transform.rotation);
        }
    }

    private IEnumerator FireShot(float delay)
    {
        yield return new WaitForSeconds(delay);
        Instantiate(projectile, barrelEnd.position, barrelPivot.gameObject.transform.rotation);
    }
    
}
