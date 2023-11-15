using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;
using System;
public class SamuraiController : MonoBehaviour, IPunObservable, ICharacterController
{
    public static SamuraiController samuraiPlayer;
    private int playerId;
    [SerializeField]
    private PhotonView view;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Rigidbody2D player_rb;

    [SerializeField]
    private Transform attackPoint;

    [SerializeField]
    private LayerMask enemyLayers;
    private bool canMove;
    float direction = 1;
    private Vector3 respawnPoint;
    private float movementSpeed = 30;
    private float extraGravity = -1;
    private float horizontal;
    private float vertical;
    private bool isFacingLeft;
    private bool FaceLeft;
    private float attack_delay = 1f;
    private bool attackBlocked;
    private float attackRange = 2f;
    public bool IsBlocking;

    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    private float groundCheckRadius = 0.2f;
    [SerializeField]
    private LayerMask groundLayers;

    private bool isGrounded;
    private float jumpForce = 150f;
    public AttackStateSam CurrentAttackStateSam { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        canMove = true;
        if(view.IsMine)
        {
            samuraiPlayer = this;
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

    // Update is called once per frame
    void Update()
    {
        
      
        if(view.IsMine)
        {         
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayers);      
            if(!canMove)
            {
                return;
            }
            float speedcheck = 0;
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
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
            //Debug.Log("Horizontal: " + horizontal); // Add this line to print out the value of the horizontal input
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
                //Debug.Log("Inside Jump If Statement");
                if(isGrounded)
                {
                    //Debug.Log("Jumping isGrounded");
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpForce);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    StartCoroutine(DelayAttack());
                }
                else
                {
                    //Debug.Log("Jumping Not isGrounded");
                    player_rb.velocity = new Vector2(player_rb.velocity.x, jumpForce);
                    view.RPC("RPC_Jump", RpcTarget.All);
                    
                }
                
            }

            if (Input.GetKeyDown(KeyCode.H))
            {
                view.RPC("RPC_StartBlock", RpcTarget.All);
            }

            if (Input.GetKeyUp(KeyCode.H))
            {
                view.RPC("RPC_StopBlock", RpcTarget.All);
            }

            if(Input.GetKeyDown(KeyCode.Space) && player_rb.velocity.y > 0)
            {
               player_rb.velocity = new Vector2(player_rb.velocity.x, player_rb.velocity.y * 0.5f);
            }

            if(Input.GetKeyUp(KeyCode.J))
            {
                view.RPC("RPC_Attack1", RpcTarget.All);
                
            }
            if(Input.GetKeyUp(KeyCode.I))
            {
                view.RPC("RPC_Attack2", RpcTarget.All);
                
            }
        }

        if(!view.IsMine)
        {
            // For remote players, horizontal is now synced. Use it to update animations:
            float speedcheck = Mathf.Abs(horizontal); // Using the absolute value of horizontal for speedcheck

            //Debug.Log("Horizontal: " + horizontal); // Add this line to print out the value of the horizontal input
            if (direction > 0 && !isFacingLeft) 
            {   
                Flip();
                animator.SetFloat("Speed", speedcheck);
                //Debug.Log("Speed: " + speedcheck); // Add this line to print out the value of the Speed parameter
            }
            else if(direction < 0 && isFacingLeft)
            {
                Flip();
                animator.SetFloat("Speed", speedcheck);
                //Debug.Log("Speed: " + speedcheck); // Add this line to print out the value of the Speed parameter
            }
            else
            {
                animator.SetFloat("Speed", speedcheck);
                //Debug.Log("Speed: " + speedcheck); // Add this line to print out the value of the Speed parameter
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

    [PunRPC]
    void RPC_Attack1()
    {
        
        animator.SetTrigger("Slash");
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
    void RPC_Attack2()
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
    //streams direction and attack state to other players

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(direction);
            stream.SendNext(CurrentAttackStateSam);
            stream.SendNext(horizontal); // Add this line
        }
        else
        {
            direction = (float)stream.ReceiveNext();
            CurrentAttackStateSam = (AttackStateSam)stream.ReceiveNext();
            horizontal = (float)stream.ReceiveNext(); // And this line
        }
    }

    public void OnTriggerEnter2D(Collider2D collision) //puts player back to where they started when they fall off
    {
        Debug.Log("Collison Detected");
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

    [PunRPC]
    void RPC_Jump()
    {
        animator.SetTrigger("Jumping");
    }
    
    public enum AttackStateSam
    {
    Idle,    
    Attack1,
    Jumping,
    Dead,
    Blocking,
    }

    private IEnumerator DelayAttack()
    {
        yield return new WaitForSeconds(attack_delay);
        attackBlocked = false;
    }

    public void OnKnockbackStart()
    {
        canMove = false;
    }

    public void OnKnockbackEnd()
    {
        canMove = true;
    }
    /*
    [PunRPC]
    void RPC_StartBlock()
    {
        // Set the current state to Blocking
        CurrentAttackStateSam = AttackStateSam.Blocking;
        Debug.Log("RPC_StartBlock called");
        animator.SetBool("IsBlocking", true);
        IsBlocking = true;
        // Wait for the block animation to reach the end
        StartCoroutine(WaitForBlockingAnimationEnd());
    }

    IEnumerator WaitForBlockingAnimationEnd()
    {
        Debug.Log("WaitForBlockingAnimationEnd started");

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
        {
            Debug.Log("Animation Normalized Time: " + animator.GetCurrentAnimatorStateInfo(0).normalizedTime);
            yield return null; // Wait for the next frame
        }

        Debug.Log("Animation should now be paused");
        animator.speed = 0; // Pause the animation
    }

    [PunRPC]
    void RPC_StopBlock()
    {
        
        Debug.Log("RPC_StopBlock called");
        animator.speed = 1; // Resume the animation
        animator.SetBool("IsBlocking", false);
        IsBlocking = false;
        // Reset the current state to Idle or whatever is appropriate
        CurrentAttackStateSam = AttackStateSam.Idle;
    }

    */


}
