using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class CrusaderController : MonoBehaviourPunCallbacks, IPunObservable, ICharacterController
{
    private int playerId;
    public UIManager uiManager;
    public static CrusaderController crusaderPlayer;
    public PhotonView view;
    [SerializeField]
    private Animator animator;
    RaycastHit2D[] PlayerHit;
    Vector2 HitRayPositionCenter;
    //public Collider2D collider;
    private bool CanHit;
    // Jumping Variables using Raycast to detect ground.
    [SerializeField]
    private float jumpSpeed;
    private bool canMove;

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
    public float attack_delay = 1f;
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
        canMove = true;
        if(view.IsMine)
        {
            crusaderPlayer = this;
        }
         
        player_rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>(); 

        player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        playerId = view.Owner.ActorNumber; // Get the ActorNumber/PlayerID
        Debug.Log("PlayerID: " + playerId);
        //facingLeft = new Vector2(-transform.localScale.x, transform.localScale.y);

        if(view.IsMine)
        {
            respawnPoint = transform.position;
        }
    
        
    }
    
    void Update() 
    {
        //Debug.Log(direction);
        //Debug.Log(isFacingLeft);
        if(!view.IsMine)
        {
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
            if (direction > 0 && !isFacingLeft) 
            {   
                Flip();
                animator.SetFloat("Speed", speedcheck);
            }
            else if(direction < 0 && isFacingLeft)
            {
                Flip();
                animator.SetFloat("Speed", speedcheck);
            }
        }

        if(view.IsMine)
        {
            if(!canMove)
            {
                return;
            }
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
            /*
            if(isTouchingGround && !Input.GetKey(KeyCode.Space))
            {
                doubleJump = false;
            }
            */
            // moves character left and right if the A or D key is pressed and rotates the animation accordingly
            if (Input.GetKeyDown(KeyCode.D) && isFacingLeft || direction > 0 && !isFacingLeft) 
            {   
                Flip();
                animator.SetFloat("Speed", speedcheck);
            }
            else if(Input.GetKeyDown(KeyCode.A) && !isFacingLeft || direction < 0 && isFacingLeft)
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
                
                if(isTouchingGround)
                {
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpSpeed);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    //doubleJump = true;
                    StartCoroutine(DelayAttack());
                }
                else 
                {
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpSpeed);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    //doubleJump = false;
                    
                }
                
            }

            if(Input.GetKeyDown(KeyCode.Space) && player_rb.velocity.y > 0)
            {
               player_rb.velocity = new Vector2(player_rb.velocity.x, player_rb.velocity.y * 0.5f);
            }

            if(Input.GetKeyUp(KeyCode.J))
            {
                view.RPC("RPC_ThrustAttack", RpcTarget.All);
                

            }
            else if(Input.GetKeyUp(KeyCode.I))
            {
                view.RPC("RPC_SlashAttack", RpcTarget.All);
                
            }
            

        }

        
        

    }
    
    private void FixedUpdate()
    { 
        if(!canMove)
        {
            return;
        }
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

    private void Flip() //can we RPC flip?
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

        //Debug.DrawRay(RayPositionCenter, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(RayPositionLeft, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(RayPositionRight, Vector2.down * rayLength, Color.red);


    }

    //checks if the player is on the ground
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
            stream.SendNext(horizontal); // Add this line
        }
        else
        {
            direction = (float)stream.ReceiveNext();
            //Debug.Log("Direction: " + direction);
            CurrentAttackState = (AttackState)stream.ReceiveNext();
            horizontal = (float)stream.ReceiveNext(); // And this line
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
        //animator.Play("Dead");
        //animator.enabled = false;
        if(view.IsMine)
        {
            this.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }

    
    
    [PunRPC]
    void RPC_ThrustAttack()
    {
        
        animator.SetTrigger("Stab");
        StartCoroutine(DelayAttack());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit a Enemy Player");
            enemy.GetComponent<PlayerHealth>().GetHit(20, transform.gameObject);

            // Get the KnockBack component on the attacked character
            KnockBack knockBack = enemy.GetComponent<KnockBack>();

            // Apply the knockback effect
            knockBack.PlayFeedback(gameObject);
        }
    }

    [PunRPC]
    void RPC_SlashAttack()
    {
        animator.SetTrigger("Slash");
        StartCoroutine(DelayAttack());

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            Debug.Log("Hit a Enemy Player");
            Debug.Log(enemy.name);
            enemy.GetComponent<PlayerHealth>().GetHit(10, gameObject);

            // Get the KnockBack component on the attacked character
            KnockBack knockBack = enemy.GetComponent<KnockBack>();

            // Apply the knockback effect
            knockBack.PlayFeedback(gameObject);
        }
    }

    [PunRPC]
    void RPC_Jump()
    {
        animator.SetTrigger("Jump");
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
    ThrustAttack,
    SlashAttack,
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

                // Get a reference to the UIManager object
                //UIManager uiManager = FindObjectOfType<UIManager>();

                if (UIManager.Instance == null)
                {
                    Debug.LogError("UIManager.Instance is null");
                    return;
                }

                PhotonView photonView = UIManager.Instance.GetComponent<PhotonView>();

                if (photonView == null)
                {
                    Debug.LogError("PhotonView is null on UIManager");
                    return;
                }
                // Call the DecreaseLives method on all players' screens using an RPC
                photonView.RPC("DecreaseLives", RpcTarget.All, playerId);
                Debug.Log("Called Decrease Lives");

                PhotonNetwork.Destroy(gameObject);

                // Find the PlayerSpawner object
                GameObject playerSpawnerObject = GameObject.Find("PlayerSpawner");

                // Get the PlayerSpawnerScript component
                PlayerSpawner playerSpawner = playerSpawnerObject.GetComponent<PlayerSpawner>();

                // Call the SpawnPlayer() function on the PlayerSpawnerScript component
                playerSpawner.Respawn_Command(view.Owner.ActorNumber);

            }
            //transform.position = respawnPoint; // use this to respawn to a location
        }
    }

    public void OnKnockbackStart()
    {
        canMove = false;
    }

    public void OnKnockbackEnd()
    {
        canMove = true;
    }
}
