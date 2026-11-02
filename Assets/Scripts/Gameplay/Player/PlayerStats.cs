using System;
using UnityEngine;

namespace Gameplay.Player
{
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] public float speed;
        [SerializeField] public float hp;
        [SerializeField] public float damage;

        private void Awake()
        {
            GameManager.Instance.playerStats = this;
        }
    }
}