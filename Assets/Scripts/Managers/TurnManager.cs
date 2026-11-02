using System;
using UnityEngine;



public class TurnManager : MonoBehaviour
{
    // reference to player and enemy
    [Header("References")]
    [SerializeField] public Player playerRef;
    [SerializeField] public Enemy sceneEnemy;
    
    [Header("Turn Settings")]
    [SerializeField] float turnSpacing = 1f;
    public int turns = 0;
    public bool turnBeingTaken = true;
    
    // private variables
    private float time;
    private float nextTime = 0f;
    
    public enum TurnAction
    {
        CHARGE,
        ANGLE,
        LANDMINE
    }
    
    void Awake()
    {
        GameManager.Instance.turnManager = this;
        playerRef = FindAnyObjectByType<Player>();
        sceneEnemy = FindAnyObjectByType<Enemy>();
    }

    private void Update()
    {
        // next turn moment reached
        if (time >= nextTime && !turnBeingTaken)
        {
            turnBeingTaken = true;
            Time.timeScale = 0;
        }
        
        time += Time.deltaTime;
    }

    // turn finished being taken, play out next moment
    public void TurnTrigger(TurnAction action, float input)
    {
        Time.timeScale = 1;
        turns++;
        turnBeingTaken = false;
        nextTime = time + turnSpacing;

        playerRef.TakeAction(action, input);
    }
    
}
