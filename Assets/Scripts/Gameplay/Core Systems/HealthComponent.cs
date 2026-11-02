using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Stores an entity's team type and HP. Come to definition to find
/// all teams available and maybe add more. Also stores the entity
/// owner itself
/// </summary>
public class HealthComponent : Component
{
    public enum Team
    {
        PLAYER,
        ENEMY
    }

    public Team team;
    public int hp;
    public int maxHP;
    public GameObject owner;

    /// <param name="hp"> Integer value describing hitpoints </param>
    /// <param name="team"> Can be from {Team.PLAYER, Team.ENEMY, Team.DESTRUCTIBLE} </param>
    /// <param name="owner"> GameObject uses this component</param>
    public HealthComponent(int hp, Team team, GameObject owner)
    {
        this.hp = hp;
        maxHP = hp;
        this.team = team;
        this.owner = owner;
    }

    /// <summary>
    /// Subtract damage.amount from the owner's hp. Can also heal through negative numbers.
    /// Entity will die when health reaches 0 and will not overheal.
    /// </summary>
    /// <param name="damage"> int of damage taken </param>
    /// <returns> True if the healthowner dies as a result of this damage </returns>
    public bool TakeDamage(int damage)
    {
        // HANDLE dmg & heal on negatives & account for overhealth
        if (hp - damage > maxHP)
        {
            hp = maxHP;
        }
        else
        {
            Debug.Log($"[DAMAGE] something on team {team} took {damage} of damage");
            hp -= damage;
        }

        if (team == Team.PLAYER)
        {
            //EventBus.Instance.HitPlayer();
        }

        if (hp > 0) return false;

        // HANDLE death 
        hp = 0;

        if (team == Team.PLAYER)
        {
            //SceneManager.LoadScene("Game Over");
            return true;
        }

        if (team == Team.ENEMY)
        {
            //EventBus.Instance.EnemyDead(owner);
        }

        Destroy(owner);
        return true;
    }
}
