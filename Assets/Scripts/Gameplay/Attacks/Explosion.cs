using System;
using System.Collections;
using System.Collections.Generic;
using Gameplay;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifetime = .05f;
    [SerializeField] private float targetRadius = 2f;
    [SerializeField] private float radius = .5f;
    [SerializeField] public float explosionPower = 10f;
    [SerializeField] public int explosionDamage = 10;
    [SerializeField] private bool displacesPlayer = false;
    
    public HealthComponent.Team teamToHit;
    private float time;
    private bool didDamage = false;

    private void Awake()
    {
        Hurtbox hb = GetComponent<Hurtbox>();
        hb.damage = explosionDamage;
        hb.team = teamToHit;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (this == null)
        {
            Destroy(this);
            return;
        }

        StartCoroutine("ExplosionStart");
    }
    
    private IEnumerator ExplosionStart()
    {
        float counter = 0f;

        while (counter < lifetime)
        {
            counter += Time.deltaTime;

            float val = Mathf.Lerp(radius, targetRadius, counter / lifetime);
            transform.localScale = new Vector3(val, val, val);
            //Debug.Log("Val: " + val);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !didDamage)
        {
            if (TryGetComponent(out PlayerHealth playerHealth) && teamToHit == HealthComponent.Team.PLAYER)
            {
                playerHealth.healthController.TakeDamage(explosionDamage);
                didDamage = true;
            }

            if (displacesPlayer)
            {
                #region Displacement
                // get difference to determine force vector
                Vector3 distance = other.transform.position - transform.position;
                // formula: direction of distance between explosion and object, multiplied by explosion power,
                // divided by distance from object
                other.attachedRigidbody.AddForce(distance.normalized * explosionPower / distance.magnitude, ForceMode2D.Impulse);
                #endregion
            }
        }
        else if (other.CompareTag("Enemy") && teamToHit == HealthComponent.Team.ENEMY && !didDamage)
        {
            if (TryGetComponent(out EnemyHealth enemyHealth))
            {
                enemyHealth.healthController.TakeDamage(explosionDamage);
                didDamage = true;
            }
            
            #region Displacement
            // get difference to determine force vector
            Vector3 distance = other.transform.position - transform.position;
            // formula: direction of distance between explosion and object, multiplied by explosion power,
            // divided by distance from object
            other.attachedRigidbody.AddForce(distance.normalized * explosionPower / distance.magnitude, ForceMode2D.Impulse);
            #endregion
        }
        
    }
}
