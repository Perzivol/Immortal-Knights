using UnityEngine;
using System.Collections;
using Spine;
using Spine.Unity;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.InputSystem;

public class AnimCtrlPhotonAnimator : MonoBehaviourPunCallbacks, IPunObservable
{
    public static AnimCtrlPhotonAnimator localPlayer;
    public PhotonView view;
    public float attackRange = 5f;
    public Animator animator;
    RaycastHit2D[] PlayerHit;
    Vector2 HitRayPositionCenter;
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

    bool CanJump = true; // can jump if on ground 
    // ***********************************************************
    public SkeletonMecanim skeletonMecanim;
    public Rigidbody2D player_rb;
    Transform myAvatar;
    //private int playerNumber;

    public float movementSpeed;
    public Transform groundCheck;

    private float horizontal;
    private float vertical;
    // Start is called before the first frame update

    [SerializeField]
    InputAction WASD;

    Vector2 movementInput;
    /*
    private void OnEnable()
    {
        WASD.Enable();
    }

    private  void OnDisable()
    {
        WASD.Disable();
    }
    */
    void Start()
    {
        view = GetComponent<PhotonView>();

        if(view.IsMine)
        {
            localPlayer = this;
        }
        
        //skeletonMe = GetComponent<SkeletonAnimation>();    
        player_rb = GetComponent<Rigidbody2D>();
        //myAvatar = transform.GetChild(0); // how to flip a skeletonanimation body
        animator = skeletonMecanim.Translator.Animator;
            
        //player_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        
        //photonPlayer = GetComponent<PhotonPlayer>();
        //Debug.Log("PlayerNumber: " + playerNumberLocal);
        
        //skeletonAnimation.AnimationName = "idle";
       

        
    }
    
    void Update() 
    {
        if(view.IsMine)
        {
            // the problem is in the animation of the skeletonmecainm
            //movementInput = WASD.ReadValue<Vector2>();
            horizontal = Input.GetAxis("Horizontal");
            vertical = Input.GetAxis("Vertical");
            //if(horizontal !=0)
            //{
            //    myAvatar.localScale = new Vector2(Mathf.Sign(horizontal), 1);
            //}   
            //player.constraints = RigidbodyConstraints2D.FreezeRotation;
            // updates ray cast every frame to the position of the character
            CanJumpRayCast();
            //Debug.Log(CanJump);
            // gets the values that come from the character moving back and forth
            

            // moves character left and right if the A or D key is pressed and rotates the animation accordingly
            if (Input.GetKeyDown(KeyCode.D) || horizontal == 1) 
            {   
                //running_2(); 
                Debug.Log("Player Is Moving Right");
                animator.SetBool("IsMoving", true);
                //skeletonAnimation.state.AnimationName = "Run"; 
                //player_rb.velocity = new Vector2(horizontal * movementSpeed, player_rb.velocity.y);
                //transform.position += transform.right * movementSpeed * Time.deltaTime;
            
            }
            else if(Input.GetKeyDown(KeyCode.A) || horizontal == -1)
            {
                Debug.Log("Player Is Moving Left");
                animator.SetBool("IsMoving", true);
                //running_2();   
                //animator.SetBool("IsMoving", true);
                //player_rb.velocity = new Vector2(horizontal * movementSpeed, player_rb.velocity.y);
                //transform.position += -transform.right * movementSpeed * Time.deltaTime;
                
            }
            else if (!Input.GetKeyDown(KeyCode.D)||!Input.GetKeyDown(KeyCode.A) || horizontal == 0) // && !Input.GetKey(KeyCode.Space))
            {
                Debug.Log("Player Is Not Moving");
                //skeletonAnimation.AnimationName = "idle";
                animator.SetBool("IsMoving", false);
                //player_rb.velocity = new Vector2(0, player_rb.velocity.y);      
            }

            if(Input.GetKey(KeyCode.Space) && CanJump|| vertical == 1 && CanJump)
            {
                //jumpAnim();
                //player.velocity = new Vector2(player.velocity.x, jumpSpeed);
                
            }
            else if (!Input.GetKey(KeyCode.Space))
            {
                //animator.SetBool("IsMoving", false);
                //player.velocity = new Vector2(player.velocity.x, jumpSpeed);
            }
            
            if(Input.GetKey(KeyCode.J) && !CanJump || Input.GetKey(KeyCode.I) && !CanJump)
            {
                if(Input.GetKey(KeyCode.J))
                {
                    //attack_1Anim();
                }
                else if(Input.GetKey(KeyCode.I))
                {
                    //attack_2Anim();
                }
                
            }

        }
        

    }

    private void FixedUpdate()
    {
        player_rb.velocity = new Vector2(horizontal * movementSpeed, player_rb.velocity.y);
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
        }
        else
        {
            direction = (float)stream.ReceiveNext();
        }
    }
/*
    private void idleAnim()
    {
        animator.SetBool("IsMoving", false);
    }
   
    private void running_2()
    {
        animator.SetBool("IsMoving", true);
    }
    
     private void jumpAnim()
    {
        // for later rigidbody.AddForce(new Vector2(0, jumpSpeed), ForceMode2d.Impulse);
        if(Input.GetKeyDown(KeyCode.Space))
        {
            spineAnimationState.SetAnimation(0, jump, false);
        }
        
    }

    private void attack_1Anim()
    {
        spineAnimationState.SetAnimation(0, attack_1, false);
    }
    
    private void attack_2Anim()
    {
        spineAnimationState.SetAnimation(0, attack_2, false);  
    }
    */
    
}
