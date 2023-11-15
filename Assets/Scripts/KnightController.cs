using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class KnightController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static KnightController localPlayer;
    public PhotonView view;
    public Animator animator;
    RaycastHit2D[] PlayerHit;
    Vector2 HitRayPositionCenter;
    //public Collider2D collider;
    private bool CanHit;
    // Jumping Variables using Raycast to detect ground.
    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float rayLength;

    [SerializeField]
    private float rayPositionOffset;
    float direction = 1;
    Vector3 RayPositionCenter;
    Vector3 RayPositionLeft;
    Vector3 RayPositionRight;

    RaycastHit2D[] GroundHitsCenter;
    RaycastHit2D[] GroundHitsLeft;
    RaycastHit2D[] GroundHitsRight;

    RaycastHit2D[][] AllRaycastHits = new RaycastHit2D[3][];
    public float extraGravity;
    bool CanJump = true; // can jump if on ground 
    // ***********************************************************
    public Rigidbody2D player_rb;
    public Transform myAvatar;

    public float movementSpeed;
    public Transform groundCheck;
    private bool isTouchingGround;
    private float horizontal;
    private float vertical;
    public float groundCheckRadius;
    public LayerMask groundLayer;

    [SerializeField]
    public Transform attackPoint;
    public bool doubleJump;
    public float attackRange = 2f;
    public LayerMask enemyLayers;

    // Start is called before the first frame update
    public float attack_delay = 5f;
    private bool attackBlocked;
    public Vector3 respawnPoint;
    Vector2 movementInput;
    
    public bool isFacingLeft;
    private bool FaceLeft;
    //private Vector2 facingLeft;

    public AttackState CurrentAttackState { get; private set; }

    void Start()
    {
        view = GetComponent<PhotonView>();

        if(view.IsMine)
        {
            localPlayer = this;
        }
         
        player_rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 

        player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);

        if(view.IsMine)
        {
            respawnPoint = transform.position;
        }
    
        
    }
    
    void Update() 
    {
        if(view.IsMine)
        {
            isTouchingGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            float speedcheck = 0;
            if(horizontal < 0)
            {
                speedcheck = horizontal * -1;
                direction = horizontal;
            }
            else if(horizontal > 0)
            {
                speedcheck = horizontal;
                direction = horizontal;
            }
            // updates ray cast every frame to the position of the character
            CanJumpRayCast();

            if(isTouchingGround && !Input.GetKey(KeyCode.Space))
            {
                doubleJump = false;
            }

            // moves character left and right if the A or D key is pressed and rotates the animation accordingly
            if (Input.GetKeyDown(KeyCode.D) && isFacingLeft || direction > 0 && isFacingLeft) 
            {   
                Flip();
                animator.SetFloat("Speed", speedcheck);
            }
            else if(Input.GetKeyDown(KeyCode.A) && !isFacingLeft || direction < 0 && !isFacingLeft)
            {
                Flip();
                animator.SetFloat("Speed", speedcheck);
                
            }
            else if (!Input.GetKeyDown(KeyCode.D)||!Input.GetKeyDown(KeyCode.A) || horizontal == 0) // && !Input.GetKey(KeyCode.Space))
            {
                animator.SetFloat("Speed", speedcheck);  
            }

            if(Input.GetKeyDown(KeyCode.Space))
            {
                if(isTouchingGround || !doubleJump)
                {
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpSpeed);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    doubleJump = true;
                    StartCoroutine(DelayAttack());
                }
                else if(doubleJump)
                {
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpSpeed);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    doubleJump = false;
                    
                }
                
            }

            if(Input.GetKeyDown(KeyCode.Space) && player_rb.velocity.y > 0 && doubleJump)
            {
               player_rb.velocity = new Vector2(player_rb.velocity.x, player_rb.velocity.y * 0.5f);
            }

            if(Input.GetKeyUp(KeyCode.J))
            {
                view.RPC("RPC_SwordAttack", RpcTarget.All);
                //animator.SetTrigger("SwordAttack");
                //animator.SetBool("Default", false);
                //CurrentAttackState = AttackState.SwordAttack;

            }
            else if(Input.GetKeyUp(KeyCode.I))
            {
                view.RPC("RPC_ShieldAttack", RpcTarget.All);
                //animator.SetTrigger("ShieldAttack");
                //animator.SetBool("Default", false);
                //CurrentAttackState = AttackState.ShieldAttack;
            }
            
        

        }
        

    }
    
    private void FixedUpdate()
    {
            player_rb.velocity = new Vector2(horizontal * movementSpeed, player_rb.velocity.y);

            if(player_rb.velocity.y < 0)
            {
                extraGravity = -Physics2D.gravity.y * Time.deltaTime * 600;
                player_rb.AddForce(Vector3.down * extraGravity);
                //player_rb.velocity = new Vector2(horizontal * movementSpeed, Physics2D.gravity.x*Time.deltaTime);
            }
            else if(player_rb.velocity.y > 0)
            {
                extraGravity = -Physics2D.gravity.y * Time.deltaTime * 1000;
                player_rb.AddForce(Vector3.down * extraGravity);
            }
        
    }

    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;
        isFacingLeft = !isFacingLeft;
    }
    //raycast method check below character to see if on the ground, will need to add on either side of character to detect when hitting another character
    private void CanJumpRayCast()
    {
        RayPositionCenter = transform.position + new Vector3(0, -0f, 0);
        RayPositionLeft = transform.position + new Vector3(-rayPositionOffset, -0f, 0);
        RayPositionRight = transform.position + new Vector3(rayPositionOffset, -0f, 0);

        GroundHitsCenter = Physics2D.RaycastAll(RayPositionCenter, Vector2.down, rayLength);
        GroundHitsLeft = Physics2D.RaycastAll(RayPositionLeft, Vector2.down, rayLength);
        GroundHitsRight = Physics2D.RaycastAll(RayPositionRight, Vector2.down, rayLength);

        // remember this for the array cause this could fix the character selection page.
        AllRaycastHits[0] = GroundHitsCenter;
        AllRaycastHits[1] = GroundHitsLeft;
        AllRaycastHits[2] = GroundHitsRight;

        CanJump = GroundCheck(AllRaycastHits);

        ///Debug.DrawRay(RayPositionCenter, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(RayPositionLeft, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(RayPositionRight, Vector2.down * rayLength, Color.red);


    }

    // checks if the player is on the ground
    private bool GroundCheck(RaycastHit2D[][] GroundHits)
    {
        foreach(RaycastHit2D[] HitList in GroundHits)
        {
            foreach(RaycastHit2D hit in HitList)
            {
                if(hit.collider != null)
                {
                    if(hit.collider.tag != "PlayerCollider")
                    {
                        return true; //if true then we can jump
                    }
                }
            }



        }
        return false;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(CurrentAttackState);
        }
        else
        {
            direction = (float)stream.ReceiveNext();
            CurrentAttackState = (AttackState)stream.ReceiveNext();
        }
    }

    public void death()
    {
        Debug.Log("Player Is Dead");
        view.RPC("RPC_Death", RpcTarget.All);
        
    }

    [PunRPC]
    void RPC_Death()
    {
        animator.Play("Dead");
        animator.enabled = false;
        if(view.IsMine)
        {
            this.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }

    
    
    [PunRPC]
    void RPC_SwordAttack()
    {
        if(attackBlocked)
        {
            return;
        }
        animator.SetTrigger("SwordAttack");
        attackBlocked = true;
        StartCoroutine(DelayAttack());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit a Enemy Player");
            enemy.GetComponent<PlayerHealth>().GetHit(20, transform.parent.gameObject);
        }
    }

    [PunRPC]
    void RPC_ShieldAttack()
    {
        if(attackBlocked)
        {
            return;
        }
        animator.SetTrigger("ShieldAttack");
        attackBlocked = true;
        StartCoroutine(DelayAttack());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit a Enemy Player");
            enemy.GetComponent<PlayerHealth>().GetHit(20, transform.parent.gameObject);
        }
    }

    [PunRPC]
    void RPC_Jump()
    {
        animator.SetTrigger("Jumping");
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attack_delay);
        attackBlocked = false;
    }
    // don't need this for now. buit this checks the state of the character
    public enum AttackState
    {
    Idle,    
    SwordAttack,
    ShieldAttack,
    Jumping,
    Dead,
    }
    
    void OnDrawGizmosSelected()
    {
        if(attackPoint == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    public void OnTriggerEnter2D(Collider2D collision) //puts player back to where they started when they fall off
    {
        //Debug.Log("Collison Detected");
        if(collision.tag == "FallDetector")
        {
            if(view.IsMine)
            {
                this.enabled = false; 
                PhotonNetwork.Destroy(gameObject);
            }
            //transform.position = respawnPoint; // use this to respawn to a location
        }
    }
}
