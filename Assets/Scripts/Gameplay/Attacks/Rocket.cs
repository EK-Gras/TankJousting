using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private GameObject explosion;
    [SerializeField] public HealthComponent.Team teamToHit;
    
    private void OnDestroy()
    {
        GameObject exp = Instantiate(explosion, transform.position, transform.rotation);
        Explosion e = exp.GetComponent<Explosion>();
        
        e.teamToHit = teamToHit;
    }
}
