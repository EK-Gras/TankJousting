using Unity.VisualScripting;
using UnityEngine;

namespace Gameplay
{
    public class Hurtbox : MonoBehaviour
    {
        // damage
        [SerializeField] public int damage;
        // param for team to damage
        [SerializeField] public HealthComponent.Team team;
        
        public void OnTriggerEnter2D(Collider2D other)
        {
            #region Player Check

            if (other.CompareTag("Player") && HealthComponent.Team.PLAYER == team)
            {
                if (other.TryGetComponent(out PlayerHealth playerHealth))
                {
                    // take damage
                    playerHealth.healthController.TakeDamage(damage);
                }
                else
                {
                    Debug.Log(other.gameObject.name + " has no health component");
                }
            }

            #endregion
            
            #region Enemy Check
            
            if (other.CompareTag("Enemy") && HealthComponent.Team.ENEMY == team)
            {
                if (other.TryGetComponent(out EnemyHealth enemyHealth))
                {
                    // take damage
                    enemyHealth.healthController.TakeDamage(damage);
                    // basher shield flip
                    Enemy temp = enemyHealth.gameObject.GetComponent<Enemy>();
                    if (temp.type == Enemy.EnemyType.BASHER)
                    {
                        temp.justHit = true;
                    }
                }
                else
                {
                    Debug.Log(other.gameObject.name + " has no health component");
                }
            }
            
            #endregion
        }
        
    }
}