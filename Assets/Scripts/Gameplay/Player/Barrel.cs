using System;
using System.Collections;
using Gameplay;
using Unity.VisualScripting;
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
    [SerializeField] private float shieldBreakDelay = .2f;

    private BarrelShieldNegotiator disableParent;
    private Hurtbox damage;
    private bool shieldLocked;
    private float startX;

    private void Start()
    {
        if (TryGetComponent(out BarrelShieldNegotiator shield))
        {
            disableParent = shield;
            disableParent.barrelToDisable = this;
        }
        damage = GetComponent<Hurtbox>();
        playerRef = GameManager.Instance.turnManager.playerRef.GetComponent<Rigidbody2D>();
        originSize = transform.localScale;
        startX = transform.localPosition.x;
    }

    private void OnEnable()
    {
        if (disableParent == null)
        {
            return;
        }
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), .1f,
                LayerMask.GetMask("Shield")) != null)
        {
            disableParent.RunShieldProtocol();
        }
        else
        {
            damage.enabled = true;
        }
    }

    private void OnDisable()
    {
        // safety
        if (this == null || damage == null)
            return;
        damage.enabled = false;
    }

    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Player") && !shieldLocked)
            ScaleBySpeed();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Shield"))
        {
            print("Shield collided");
            // direction to push player in when colliding with shield
            float x = other.transform.position.x > transform.position.x ? -1 : 1;
            print(x);
            playerRef.linearVelocity = new Vector2(x * shieldHitReboundStrength, 0);
            playerRef.AddForce(new Vector2(x * shieldHitReboundStrength, 0), ForceMode2D.Impulse);
            // print(direction.normalized * shieldHitReboundStrength);
            // barrel disable
            disableParent.RunShieldProtocol();
        }
    }

    public void ScaleBySpeed()
    {
        // first, take the size and modify it by player's speed, divided by scaling divider
        currentSize = Mathf.Clamp(playerRef.linearVelocity.magnitude / scalingDivider, minSize, maxSize);
        transform.localScale = new Vector3(originSize.x * currentSize, originSize.y, originSize.z);
        
        // then, take midway point of y scale to find anchor point
        float anchorX = transform.localScale.x;
        transform.localPosition = new Vector3(anchorX * startX, 0, 0);
        damage.damage = (int) (GameManager.Instance.playerStats.damage * currentSize);
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
