using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] float projectileSpeed = 30f;
    [SerializeField] int projectileDamage = 5;
    
    private Rigidbody2D rb;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb != null)
        {
            rb.AddForce(projectileSpeed * transform.up, ForceMode2D.Impulse);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (collision.gameObject.TryGetComponent(out PlayerHealth playerHealth))
            {
                playerHealth.healthController.TakeDamage(projectileDamage);
            }
        }
        if (!collision.gameObject.CompareTag("Enemy"))
            Destroy(gameObject);
    }
}
