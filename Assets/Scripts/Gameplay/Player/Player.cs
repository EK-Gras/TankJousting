using System;
using System.Collections;
using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    private CapsuleCollider2D capsuleCollider;
    
    [SerializeField] GameObject landminePrefab;
    [SerializeField] public BarrelPivot barrelPivot;
    [SerializeField] Barrel barrel;

    [Header("Movement")] 
    [SerializeField] Transform groundCheck;
    [SerializeField] private float groundCheckRadius = .01f;
    [SerializeField] private float slopeCheckDistance;
    [SerializeField] private float gravityScale = 1f;
    
    
    // Private variables
    private LayerMask groundLayer;
    
    TurnManager.TurnAction currentTurnAction;
    
    private float actionFloat;
    private float slopeDownAngle;
    private float slopeDownAngleOld;
    private float slopeSideAngle;

    private Vector2 slopeNormalPerp;
    
    private bool startedLook = false;
    public bool isGrounded;
    public bool isOnSlope;
    public bool rightSlope;
    
    private Vector2 colliderSize;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        colliderSize = capsuleCollider.size;
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void FixedUpdate()
    {
        // stability check
        transform.rotation = new Quaternion(0,0,0,0);
        
        GroundCheck();

        float maxSpeed = GameManager.Instance.playerStats.maxSpeed;
        
        // switch for action
        switch (currentTurnAction)
        {
            case TurnManager.TurnAction.CHARGE:
                
                // check for terrain slopes
                SlopeCheck();

                // delete gravity while not airborne
                rb.gravityScale = 0;

                if (isGrounded && !isOnSlope)
                {
                    // not max speed
                    if (rb.linearVelocity.magnitude <= maxSpeed)
                    {
                        // move left and right, no change to y
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x + actionFloat * Time.deltaTime * GameManager.Instance.playerStats.acceleration, rb.linearVelocity.y);
                    }

                    // max speed limit
                    if (rb.linearVelocity.magnitude > maxSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                    }
                }
                else if (isGrounded && isOnSlope)
                {
                    // not max speed
                    if (rb.linearVelocity.magnitude <= maxSpeed)
                    {
                        // ok this is painful math, but it's basically: take normal ground movement formula -> multiply by the x vector of the normal perpendicular
                        // -> multiply by negative one because the direction needs inversion 
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x + actionFloat * Time.deltaTime * GameManager.Instance.playerStats.acceleration * -slopeNormalPerp.x,
                            0);
                        
                        // check for left or right slope and adjust y
                        if (rightSlope)
                        {
                            // when moving up and to the right
                            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.x);
                        }
                        
                        else
                        {
                            // when moving up and to the left
                            rb.linearVelocity = new  Vector2(rb.linearVelocity.x, -rb.linearVelocity.x);
                        }
                    }

                    // max speed limit
                    if (rb.linearVelocity.magnitude > maxSpeed)
                    {
                        rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
                    }
                }
                else
                {
                    // if in air, apply gravity
                    rb.gravityScale = gravityScale;
                }
                
                break;
            case TurnManager.TurnAction.ANGLE:
                if (startedLook) return;
                Quaternion look = new Quaternion(0,0,actionFloat,0);
                barrelPivot.StartRotationTowards(look);
                startedLook = true;
                rb.gravityScale = gravityScale;
                break;
        }
        
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0, colliderSize.y / 3, 0);
        
        HorizontalSlopeCheck(checkPos);
        VerticalSlopeCheck(checkPos);
    }

    private void HorizontalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D frontCheck = Physics2D.Raycast(checkPos, Vector2.right, slopeCheckDistance, groundLayer);
        RaycastHit2D backCheck= Physics2D.Raycast(checkPos, Vector2.left, slopeCheckDistance, groundLayer);

        if (frontCheck)
        {
            isOnSlope = true;
            rightSlope = true;
            slopeSideAngle = Vector2.Angle(frontCheck.normal, Vector2.up);
        }
        else if (backCheck)
        {
            isOnSlope = true;
            rightSlope = false;
            slopeSideAngle = Vector2.Angle(backCheck.normal, Vector2.up);
        }
        else
        {
            isOnSlope = false;
            slopeSideAngle = 0.0f;
        }
    }

    private void VerticalSlopeCheck(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        if (hit)
        {
            slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
            
            slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeDownAngle != slopeDownAngleOld)
            {
                isOnSlope = true;
            }

            slopeDownAngleOld = slopeDownAngle;
            
            Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            Debug.DrawRay(hit.point, hit.normal, Color.green);
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
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
        
        StartCoroutine("TurnTimer");
    }

    private IEnumerator TurnTimer()
    {
        yield return new WaitForSeconds(1);
        GameManager.Instance.turnManager.EnemyTurnTrigger();
    }

    private IEnumerator PlaceLandmine()
    {
        // place landmine at end of turn
        yield return new WaitForSeconds(1);
        Vector2 landminePosition = new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2);
        Instantiate(landminePrefab, landminePosition, Quaternion.identity);
        landminePrefab.GetComponent<Landmine>().timer = actionFloat;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
