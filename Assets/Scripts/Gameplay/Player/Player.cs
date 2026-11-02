using System;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    
    TurnManager.TurnAction currentTurnAction;
    private float actionFloat;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        switch (currentTurnAction)
        {
            case TurnManager.TurnAction.CHARGE:
                
                rb.linearVelocity = new Vector2(GameManager.Instance.playerStats.speed * actionFloat, 0);
                break;
        }
    }

    public void TakeAction(TurnManager.TurnAction action, float input)
    {
        currentTurnAction = action;
        actionFloat = input;
    }
}
