using System;
using UnityEngine;

namespace Gameplay.Player
{
    [Serializable]
    public class PlayerStatsData
    {
        public float maxSpeed;
        public float hp;
        public float damage;
        public float acceleration;
        public float landmineStrength;
        public float explosionSize;
    }

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

            if (GameManager.Instance.savedStats == null)
            {
                GameManager.Instance.savedStats = new PlayerStatsData
                {
                    maxSpeed = maxSpeed,
                    hp = hp,
                    damage = damage,
                    acceleration = acceleration,
                    landmineStrength = landmineStrength,
                    explosionSize = explosionSize
                };
            }
            else
            {
                PlayerStatsData savedStats = GameManager.Instance.savedStats;
                maxSpeed = savedStats.maxSpeed;
                hp = savedStats.hp;
                damage = savedStats.damage;
                acceleration = savedStats.acceleration;
                landmineStrength = savedStats.landmineStrength;
                explosionSize = savedStats.explosionSize;
            }
        }
    }
}