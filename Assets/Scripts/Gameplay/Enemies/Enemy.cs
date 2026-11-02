using System;
using UnityEngine;


public class Enemy: MonoBehaviour
{
    private EnemyHealth health;

    private void Awake()
    {
        health = GetComponent<EnemyHealth>();
    }
}