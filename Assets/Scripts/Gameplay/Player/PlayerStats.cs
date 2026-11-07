using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] public float maxSpeed;
        [SerializeField] public float hp;
        [SerializeField] public float damage;
        [SerializeField] public float acceleration;
        [SerializeField] public float landmineStrength;
        [SerializeField] public float explosionSize;
        

        private void Awake()
        {
            GameManager.Instance.playerStats = this;
        }
    }
}