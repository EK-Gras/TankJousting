using System;
using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] GameObject landminePrefab;
    [SerializeField] public BarrelPivot barrelPivot;
    [SerializeField] Barrel barrel;
    
    TurnManager.TurnAction currentTurnAction;
    private float actionFloat;
    private bool startedLook = false;
    
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
                // TODO potentially update to work with sticking to terrain and also go up and down terrain
                rb.linearVelocity = new Vector2(GameManager.Instance.playerStats.speed * actionFloat, 0);
                break;
            case TurnManager.TurnAction.ANGLE:
                if (startedLook) return;
                Quaternion look = new Quaternion(0,0,actionFloat, 0);
                print(look);
                barrelPivot.StartRotationTowards(look);
                startedLook = true;
                break;
        }
    }

    public void TakeAction(TurnManager.TurnAction action, float input)
    {
        currentTurnAction = action;
        actionFloat = input;
        startedLook = false;

        if (action == TurnManager.TurnAction.LANDMINE)
        {
            StartCoroutine("PlaceLandmine");
        }
    }

    private IEnumerator PlaceLandmine()
    {
        // place landmine at end of turn
        yield return new WaitForSeconds(1);
        Vector2 landminePosition = new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2);
        Instantiate(landminePrefab, landminePosition, Quaternion.identity);
        landminePrefab.GetComponent<Landmine>().timer = actionFloat;
    }
}
