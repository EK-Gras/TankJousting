using System;
using System.Collections;
using UnityEngine;


public class Enemy: MonoBehaviour
{
    [Header("References")]
    [SerializeField] Barrel barrel;
    
    private EnemyHealth health;
    private Rigidbody2D rb;
    public bool takingTurn;
    
    bool directionLastMoved = true;
    private bool turnEnder = false;

    public enum EnemyAction
    {
        MOVE,
        ROTATE,
        SHOOT,
        IDLE
    }

    private EnemyAction _enemyAction;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
    }

    private void FixedUpdate()
    {
        if (takingTurn)
        {
            switch (_enemyAction)
            {
                case EnemyAction.MOVE:
                    // forwards (their left)
                    if (directionLastMoved)
                    {
                        rb.linearVelocity = Vector2.left * 10;
                    }
                    // backwards
                    else
                    {
                        rb.linearVelocity = Vector2.right * 10;
                    }
                    break;
                case EnemyAction.SHOOT:
                    if (!turnEnder)
                    {
                        barrel.ReadyFire();
                        turnEnder = true;
                    }
                    break;
            }
        }
    }

    public void TakeTurn(int turnNumber)
    {
        takingTurn = true;
        
        EnemyAction action = EnemyAction.IDLE;
        
        // turret decision tree, add specific trees for specific enemies
        // explanation: enemies take even numbered turns, so every other turn they take (%4), make them alternate btwn
        // idle and shooting
        switch (turnNumber%4)
        {
            case 0:
                action = EnemyAction.IDLE;
                break;
            case 2:
                action = EnemyAction.SHOOT;
                break;
        }
        
        _enemyAction = action;

        StartCoroutine("TurnTimer");
    }

    private IEnumerator TurnTimer()
    {
        yield return new WaitForSeconds(1);
        directionLastMoved = !directionLastMoved;
        takingTurn = false;
    }
}