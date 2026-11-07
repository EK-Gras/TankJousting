using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float projectileSpeed = 30f;
    [SerializeField] int projectileDamage = 5;
    [SerializeField] HealthComponent.Team teamToDamage = HealthComponent.Team.PLAYER;
    [SerializeField] public bool parriable = true;
    [SerializeField] private float lifetime = 1f;
    [SerializeField] private bool hasLifetime = false;
    [SerializeField] private float knockbackforce = .3f;
    
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb;
    
    private void Awake()
    {
        // either rigidbody or collider with trigger
        if (TryGetComponent(out Rigidbody2D rigidbody))
            rb = rigidbody;
        else
        {
            boxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb != null)
        {
            rb.AddForce(projectileSpeed * transform.up, ForceMode2D.Impulse);
        }

        if (hasLifetime)
        {
            StartCoroutine("DestroyProjectile");
        }
    }

    private IEnumerator DestroyProjectile()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && teamToDamage == HealthComponent.Team.PLAYER)
        {
            if (collision.gameObject.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.healthController.TakeDamage(projectileDamage);
                if (!hasLifetime)
                    Destroy(gameObject);
            }

            else if (collision.gameObject.TryGetComponent(out Barrel barrel))
            {
                Parried();
            }
        }
        else if (collision.gameObject.CompareTag("Enemy") && teamToDamage == HealthComponent.Team.ENEMY)
        {
            if (collision.gameObject.TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.healthController.TakeDamage(projectileDamage);
                if (!hasLifetime)
                    Destroy(gameObject);
            }
        }
        // ground or shield
        else if (!collision.gameObject.CompareTag("Enemy") && !hasLifetime)
            Destroy(gameObject);
    }

    // knockback on player for juice on the big laser
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.TryGetComponent(out Rigidbody2D rigidbody))
            {
                // direction is difference between collider and start of laser
                Vector3 dir = collision.gameObject.transform.position - GetComponentInParent<Transform>().position;
                // direction * knockback force along the x axis
                rigidbody.AddForce(new Vector2(dir.normalized.x * knockbackforce, 0), ForceMode2D.Impulse);
            }
        }
    }
    
    private void Parried()
    {
        if (!parriable)
        {
            return;
        }
        
        print("parried!");
        
        // TODO add juice to the parry
        
        // locate enemy, swap team damage, and send it back with some added speed (maybe negate its gravity for nearly guaranteed hit)
        teamToDamage = HealthComponent.Team.ENEMY;
        
        GameObject returnToSender = FindAnyObjectByType<Enemy>().gameObject;
        Vector3 returnTo = returnToSender.transform.position;
        print(returnTo);
        // negate velocity
        rb.linearVelocity = Vector2.zero;
        // direction
        Vector2 direction = returnTo - transform.position;
        print(direction);
        // add force
        rb.AddForce(direction.normalized * Math.Abs(projectileSpeed) * 2, ForceMode2D.Impulse);
        print(rb.linearVelocity);
        // optional gravity
        rb.gravityScale = 0;

        if (TryGetComponent(out Rocket rocket))
        {
            rocket.teamToHit = teamToDamage;
            print("Rocket parried");
        }
    }
}
