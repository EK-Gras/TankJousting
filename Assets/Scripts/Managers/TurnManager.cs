using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class TurnManager : MonoBehaviour
{
    // reference to player and enemy
    [Header("References")]
    [SerializeField] public Player playerRef;
    [SerializeField] public Enemy sceneEnemy;
    [SerializeField] public GameObject turnPanel;
    [SerializeField] public Button beginTurnButton;
    
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
        GameManager.Instance.roundOver = false;
        Time.timeScale = 1;

        playerRef = FindAnyObjectByType<Player>();
        sceneEnemy = FindAnyObjectByType<Enemy>();
        beginTurnButton = FindAnyObjectByType<BeginTurn>()._button;
    }

    private void Update()
    {
        if (GameManager.Instance.roundOver) { return; }

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
        
        if (GameManager.Instance.roundOver) { return; }
        
        Time.timeScale = 1;
        turns++;
        turnBeingTaken = false;
        nextTime = time + turnSpacing;

        playerRef.TakeAction(action, input);
    }

    public void EnemyTurnTrigger()
    {

        if (GameManager.Instance.roundOver) { return; }

        turns++;
        nextTime = time + turnSpacing;

        sceneEnemy.TurnOver();
        
        sceneEnemy.TakeTurn(turns);
        turnPanel.SetActive(false);
        StartCoroutine("EnemyTurn");
        
    }

    private IEnumerator EnemyTurn()
    {
        yield return new WaitForSeconds(1);

        if (GameManager.Instance.roundOver) { yield break; }

        turnPanel.SetActive(true);
        beginTurnButton.enabled = true;
    }
}
