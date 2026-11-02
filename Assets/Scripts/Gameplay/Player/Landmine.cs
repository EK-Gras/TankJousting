using System;
using System.Collections;
using UnityEngine;

public class Landmine : MonoBehaviour
{
    public float timer;
    [SerializeField] private GameObject explosionPrefab;

    private void OnDestroy()
    {
        Instantiate(explosionPrefab,  transform.position, Quaternion.identity);
    }

    private void Start()
    {
        StartCoroutine("Timebomb");
    }

    private IEnumerator Timebomb()
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }
}
