using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private EnemyHealth enemyHealth;
    private Slider slider;
    private HealthComponent healthComponent;

    private void Awake()
    {
        if (playerHealth == null)
        {
            enemyHealth = FindAnyObjectByType<EnemyHealth>();
        }
        
        slider = GetComponent<Slider>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerHealth != null)
        {
            healthComponent = playerHealth.healthController;
        }
        else if (enemyHealth != null)
        {
            healthComponent = enemyHealth.healthController;
        }

        slider.maxValue = healthComponent.maxHP;
        slider.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        slider.value = healthComponent.hp;
    }
}
