using System;
using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float lifetime = .05f;
    [SerializeField] private float targetRadius = 2f;
    [SerializeField] private float radius = .5f;
    [SerializeField] private float explosionPower = 10f;
    
    private float time;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
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
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            // get difference to determine force vector
            Vector3 distance = other.transform.position - transform.position;
            // formula: direction of distance between explosion and object, multiplied by explosion power,
            // divided by distance from object
            other.attachedRigidbody.AddForce(distance.normalized * explosionPower / distance.magnitude, ForceMode2D.Impulse);
        }
    }
}
