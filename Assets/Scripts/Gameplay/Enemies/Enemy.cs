using System;
using System.Collections;
using UnityEngine;


public class Enemy: MonoBehaviour
{
    private EnemyHealth health;
    private Rigidbody2D rb;
    public bool takingTurn;
    
    bool directionLastMoved = true;

    public enum EnemyAction
    {
        MOVE,
        ROTATE
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
            }
        }
    }

    public void TakeTurn(EnemyAction action)
    {
        takingTurn = true;
        // TODO decision tree code here to decide proper action

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