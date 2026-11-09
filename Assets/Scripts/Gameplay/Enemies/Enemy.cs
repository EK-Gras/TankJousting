using System;
using System.Collections;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;


public class Enemy: MonoBehaviour
{
    [Header("References")]
    [SerializeField] Barrel barrel;
    [SerializeField] BarrelPivot pivot;
    [SerializeField] Transform groundCheck;
    [SerializeField] GameObject nuke;
    
    private EnemyHealth health;
    private Rigidbody2D rb;
    public bool takingTurn;

    [Header("Settings")]
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float movementDirection = -1f;
    [SerializeField] private float acceleration = 5f;
    [SerializeField] private float gravityScale = 1f;
    [SerializeField] private float groundCheckRadius = .01f;
    [SerializeField] private float slopeCheckDistance;
    
    [SerializeField] public EnemyType type;
    
    // Private variables
    private LayerMask groundLayer;
    
    private float actionFloat;
    private float slopeDownAngle;
    private float slopeDownAngleOld;

    private Vector2 slopeNormalPerp;
    
    private bool turnEnder = false;
    private bool startedLook = false;
    private bool shieldFront = true;
    
    public bool isGrounded;
    public bool isOnSlope;
    public bool rightSlope;
    public bool justHit;
    
    private Quaternion barrelStartRotation;
    
    
    public enum EnemyType
    {
        TURRET,
        BASHER,
        BIGBOY
    }
    
    public enum EnemyAction
    {
        IDLE,
        MOVE,
        CHARGE,
        ROTATE,
        SHOOT,
        ROCKET,
        EXPLOSION,
        LASER
    }

    private EnemyAction _enemyAction;
    
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<EnemyHealth>();
        groundLayer = LayerMask.GetMask("Ground");
        barrelStartRotation = new Quaternion(0,0,-barrel.sprayEndPoint.z,0);
    }

    private void FixedUpdate()
    {
        // stability check
        transform.rotation = new Quaternion(0,0,0,0);
        
        GroundCheck();
        
        // switch for action
        switch (_enemyAction)
        {
            case EnemyAction.MOVE:
                
                // check for terrain slopes
                SlopeCheck();

                // delete gravity while not airborne
                rb.gravityScale = 0.1f;

                if (isGrounded && !isOnSlope)
                {
                    // not max speed
                    if (rb.linearVelocity.magnitude <= maxSpeed)
                    {
                        // move left and right, no change to y
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movementDirection * Time.deltaTime * acceleration, rb.linearVelocity.y);
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
                        rb.linearVelocity = new Vector2(rb.linearVelocity.x + movementDirection * Time.deltaTime * acceleration * -slopeNormalPerp.x,
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
            case EnemyAction.ROTATE:
                if (startedLook) return;
                print("Enemy is rotating their barrel");
                pivot.StartRotationTowards(barrelStartRotation);
                startedLook = true;
                rb.gravityScale = gravityScale;
                break;
            case EnemyAction.SHOOT:
                if (!turnEnder)
                {
                    print("Enemy is shooting once");
                    // one second delay
                    barrel.ReadyFire(EnemyAction.SHOOT,1f);
                    turnEnder = true;
                }
                rb.gravityScale = gravityScale;
                break;
            case EnemyAction.ROCKET:
                if (!turnEnder)
                {
                    print("Enemy is shooting a spray of rockets");
                    // spread of 5 rockets, at .2 second intervals
                    barrel.ReadyFire(EnemyAction.ROCKET, .2f);
                    turnEnder = true;
                }
                rb.gravityScale = gravityScale;
                break;
            case EnemyAction.LASER:
                if (!turnEnder)
                {
                    barrel.ReadyFire(EnemyAction.LASER, 0f);
                    turnEnder = true;
                }
                rb.linearVelocity = Vector2.zero;
                rb.gravityScale = gravityScale;
                break;
            case EnemyAction.CHARGE:
                rb.gravityScale = gravityScale;
                
                break;
            case EnemyAction.EXPLOSION:
                if (!turnEnder)
                {
                    DropNuke();
                    turnEnder = true;
                }
                rb.gravityScale = gravityScale;
                break;
        }
        
    }

    private void SlopeCheck()
    {
        Vector2 checkPos = new Vector2(groundCheck.position.x, groundCheck.position.y + 1);
        
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
        }
        else if (backCheck)
        {
            isOnSlope = true;
            rightSlope = false;
        }
        else
        {
            isOnSlope = false;
        }
        Debug.DrawRay(frontCheck.point, frontCheck.normal, Color.blue);
        Debug.DrawRay(backCheck.point, backCheck.normal, Color.yellow);
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
    
    public void TakeTurn(int turnNumber)
    {
        takingTurn = true;

        switch (type)
        {
            case EnemyType.TURRET:
                TurretTurn(turnNumber);
                break;
            case EnemyType.BASHER:
                BasherTurn(turnNumber);
                break;
            case EnemyType.BIGBOY:
                BigBoyTurn(turnNumber);
                break;
        }
        
    }

    private void TurretTurn(int turnNumber)
    {
        EnemyAction action = EnemyAction.IDLE;
        
        // turret decision tree, add specific trees for specific enemies
        // explanation: enemies take even numbered turns, so use %(even #) to determine loop
        // turn order: Shoot forward -> missile spread -> rotate back to forward -> idle -> repeat
        switch (turnNumber%8)
        {
            case 0:
                action = EnemyAction.IDLE;
                print("Enemy is taking a break");
                break;
            case 2:
                action = EnemyAction.SHOOT;
                break;
            case 4:
                action = EnemyAction.ROCKET;
                break;
            case 6:
                action = EnemyAction.ROTATE;
                break;
        }
        
        _enemyAction = action;
    }

    private void BasherTurn(int turnNumber)
    {
        EnemyAction action = EnemyAction.IDLE;

        if (justHit)
        {
            justHit = false;
            BasherShieldFlip();
            return;
        }
        
        // Basher decision tree
        // explanation: enemies take even numbered turns, so use %(even #) to determine loop
        // turn order: Move forward -> Keep going forward -> Move backwards -> Keep going backwards -> idle -> repeat
        switch (turnNumber%10)
        {
            case 0:
                action = EnemyAction.IDLE;
                print("Enemy is taking a break");
                break;
            case 2:
                action = EnemyAction.MOVE;
                movementDirection = -1f;
                print("Enemy is moving forwards");
                break;
            case 4:
                action = EnemyAction.MOVE;
                movementDirection = -1f;
                print("Enemy is moving forwards");
                break;
            case 6:
                action = EnemyAction.MOVE;
                movementDirection = 1f;
                print("Enemy is moving backwards");
                break;
            case 8:
                action = EnemyAction.MOVE;
                movementDirection = 1f;
                print("Enemy is moving backwards");
                break;
            
        }
        
        _enemyAction = action;
    }
    
    public void BasherShieldFlip()
    {
        print("Basher hit, flipping shield");
        // rotate shield to opposite side
        if (shieldFront)
        {
            Quaternion temp = new Quaternion(0,0,barrel.sprayEndPoint.z, 0);
            pivot.StartRotationTowards(temp);
            shieldFront = false;
        }
        else
        {
            pivot.StartRotationTowards(barrelStartRotation);
            shieldFront = true;
        }
    }

    private void BigBoyTurn(int turnNumber)
    {
        EnemyAction action = EnemyAction.IDLE;

        if (justHit)
        {
            justHit = false;
            BasherShieldFlip();
            return;
        }
        
        // Basher decision tree
        // explanation: enemies take even numbered turns, so use %(even #) to determine loop
        // turn order: Move forward -> Charge explosion -> Explosion -> Move backwards
        // -> angle barrel left or right (depending on player position) -> Charge laser -> fire laser -> Idle
        // -> repeat
        switch (turnNumber%16)
        {
            case 0:
                action = EnemyAction.IDLE;
                print("Enemy is taking a break");
                break;
            case 2:
                action = EnemyAction.MOVE;
                movementDirection = transform.position.x > 0 ? -1: 1; 
                print("Enemy is moving");
                break;
            case 4:
                action = EnemyAction.CHARGE;
                print("Enemy is charging AOE attack");
                break;
            case 6:
                action = EnemyAction.EXPLOSION;
                print("Enemy is unleashing a big explosion");
                break;
            case 8:
                action = EnemyAction.MOVE;
                movementDirection = transform.position.x > 0 ? -1: 1; 
                print("Enemy is moving");
                break;
            case 10:
                // player position
                Player playerRef = GameManager.Instance.turnManager.playerRef;
                // if player is to the right of Big Boy, search for correct rotation
                if (playerRef.transform.position.x > transform.position.x)
                    action = pivot.transform.rotation.z >= 0 ? EnemyAction.ROTATE : EnemyAction.IDLE;
                // player is to the left
                else
                    action = pivot.transform.rotation.z <= 0 ? EnemyAction.ROTATE : EnemyAction.IDLE;
                print("Enemy is angling barrel towards player vicinity");
                break;
            case 12:
                action = EnemyAction.CHARGE;
                print("Enemy is charging AOE laser");
                break;
            case 14:
                action = EnemyAction.LASER;
                print("Enemy is firing its laser");
                break;
        }
        
        _enemyAction = action;
        
        // TODO change text of the enemy turn panel to describe the move it's taking
    }
    
    private void DropNuke()
    {
        Instantiate(nuke, new Vector2(transform.position.x, transform.position.y + 10), Quaternion.identity);
    }
    
    public void TurnOver()
    {
        takingTurn = true;
        startedLook = false;
        turnEnder = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}