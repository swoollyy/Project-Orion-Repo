using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;


    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYscale;
    private float startYScale;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode sneakKey = KeyCode.C;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("State Checks")]
    public bool isSneaking;
    public bool isSprinting;
    public bool isCrouching;
    public bool isWalking;
    public bool isJumping;
    public bool isIdle;

    ClampedFloatParameter focalLengthdefValue = new ClampedFloatParameter(90f, 90f, 90f);


    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    public bool crouchBob;
    public bool walkBob;
    public bool sprintBob;

    bool stopMovement;

    bool increaseDoF;
    bool decreaseDoF;

    public bool hasJumped;
    public bool hasCrouched;
    public bool hasEscaped;

    public bool isCrippled;
    public bool isBeingSucked;

    public Animator gunAnim;
    public Animator knifeAnim;
    public Animator camAnim;

    public Inventory inv;
    public GameController gc;
    public TutorialController tutController;

    Vector3 moveDirection;

    public VolumeProfile volumeProfile;
    DepthOfField dof;
    float focalLengthValue;


    Rigidbody rb;

    Camera cam;
    Quaternion quat;
    Vector3 vect;

    public GameObject playertracker;
    public LayerMask groundMask;

    public MovementState currentState;
    public enum MovementState
    {
        idle,
        walking,
        sprinting,
        crouching,
        sneaking,
        air
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;

        cam = Camera.main;

        if (!volumeProfile.TryGet(out dof)) throw new System.NullReferenceException(nameof(dof));
        focalLengthValue = 1f;
        dof.focalLength.Override(focalLengthValue);

    }

    // Update is called once per frame
    void Update()
    {
        MyInput();
        SpeedControl();
        StateHandler();
        MovePlayer();


        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, Mathf.Infinity, groundMask))
        {
            playertracker.transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
        }

        if(tutController.currentState == tutController.fieldState)
        {
            if(isCrippled)
            {
                walkSpeed = 1.1f;
                sprintSpeed = 2f;
                jumpForce = 0f;
            }
            else if(isBeingSucked)
            {
                walkSpeed = 2.5f;
                sprintSpeed = 3.3f;
                jumpForce = 6f;
            }
            else
            {
                walkSpeed = 3.3f;
                sprintSpeed = 5f;
                jumpForce = 9f;
            }
        }


        grounded = Physics.SphereCast(transform.position, 0.45f, Vector3.down, out hit, playerHeight * .5f + .12f, whatIsGround);


        if (grounded || OnSlope())
            rb.drag = groundDrag;
        else rb.drag = 0;

        vect = new Vector3(0f, cam.transform.eulerAngles.y, 0f);
        quat.eulerAngles = vect;
        transform.rotation = quat;


        if (currentState == MovementState.idle && inv.isHoldingGun)
        {
            gunAnim.SetBool("IsIdle", true);
        }
        else gunAnim.SetBool("IsIdle", false);

        if (Input.GetKeyDown(gc.aimKey) && inv.isHoldingGun)
        {
            gunAnim.SetBool("IsAiming", true);
            
            camAnim.SetBool("StoppedAiming", false);
            camAnim.SetBool("IsAiming", true);


            decreaseDoF = false;
            increaseDoF = true;

        }
        if (Input.GetKeyUp(gc.aimKey) && inv.isHoldingGun)
        {
            gunAnim.SetBool("IsAiming", false);
            camAnim.SetBool("IsAiming", false);
            camAnim.SetBool("StoppedAiming", true);

            increaseDoF = false;
            decreaseDoF = true;
        }

        if (increaseDoF)
        IncreaseDoF();

        if (decreaseDoF)
            DecreaseDoF();



        if ((currentState == MovementState.walking || currentState == MovementState.sprinting))
        {
            if(gunAnim.enabled)
            gunAnim.SetBool("IsWalking", true);
            if(knifeAnim.enabled)
            {
                knifeAnim.SetBool("IsWalking", true);
                knifeAnim.SetBool("IsIdle", false);
            }

        }
        else
        {
            if (gunAnim.enabled)
                gunAnim.SetBool("IsWalking", false);
            if (knifeAnim.enabled)
            {
                knifeAnim.SetBool("IsWalking", false);
                knifeAnim.SetBool("IsIdle", true);
            }
        }
        if (currentState == MovementState.sprinting && gunAnim.enabled)
        {
            gunAnim.SetBool("IsSprinting", true);
        }
        else gunAnim.SetBool("IsSprinting", false);

        if(isIdle)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, new Vector3(.3f, rb.velocity.y, .3f), Time.deltaTime);
            if (rb.velocity.magnitude <= .31f)
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }

    } 
    private void FixedUpdate()
    {
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        if(Input.GetKey(jumpKey) && readyToJump && grounded && currentState != MovementState.crouching)
        {
            readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);

        }
        if(Input.GetKeyDown(crouchKey) && grounded)
        {
            hasCrouched = true;
            GetComponent<CapsuleCollider>().height = 1.3f;
            GetComponent<CapsuleCollider>().center = new Vector3(0f, -.35f, 0f);
        }
        if (Input.GetKeyUp(crouchKey))
        {
            hasCrouched = false;
            GetComponent<CapsuleCollider>().height = 2f;
            GetComponent<CapsuleCollider>().center = new Vector3(0f, 0f, 0f);
        }
    }

    private void StateHandler()
    {

        //crouching
        if (Input.GetKey(crouchKey) && grounded)
        {
            currentState = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }
        //sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            currentState = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        //sprinting
        else if (grounded && Input.GetKey(sneakKey))
        {
            currentState = MovementState.sneaking;
            moveSpeed = crouchSpeed;
        }

        //walking
        else if (grounded && (horizontalInput != 0 || verticalInput != 0))
        {
            currentState = MovementState.walking;
            moveSpeed = walkSpeed;
        }
        else if(grounded && (horizontalInput == 0 && verticalInput == 0))
        {
            currentState = MovementState.idle;
        }

        //air
        else if (!grounded)
        {
            currentState = MovementState.air;
        }

        //conditionals
        if (currentState == MovementState.crouching)
        {
            isCrouching = true;
            crouchBob = true;
        }
        else
        {
            isCrouching = false;
            crouchBob = false;
        }
        if (currentState == MovementState.sneaking)
        {
            isSneaking = true;
            crouchBob = true;
        }
        else
        {
            isSneaking = false;
            crouchBob = false;
        }
        if (currentState == MovementState.walking)
        {
            isWalking = true;
            walkBob = true;
        }
        else
        {
            isWalking = false;
            walkBob = false;
        }
        if (currentState == MovementState.sprinting)
        {
            isSprinting = true;
            sprintBob = true;
        }
        else
        {
            isSprinting = false;
            sprintBob = false;
        }
        if (currentState == MovementState.air)
        {
            isJumping = true;
            hasJumped = true;
        }
        else
        {
            isJumping = false;
        }

        if (currentState == MovementState.idle)
        {
            isIdle = true;
        }
        else
            isIdle = false;

    }
    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        //on slope
        if(OnSlope() && !exitingSlope)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, GetSlopeMoveDirection() * moveSpeed, Time.deltaTime);
        }

        //grounded
        if (grounded)
        rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * moveSpeed * 15f, .5f * Time.deltaTime);
        //in air
        else if(!grounded)
            rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * airMultiplier * moveSpeed, .7f * Time.deltaTime);


    }

    private void SpeedControl()
    {




    }

    private void Jump()
    {
        exitingSlope = true;
        hasJumped = false;

        //reset y velocity
        if(isIdle)
        rb.velocity = Vector3.zero;

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        hasJumped = true;
        readyToJump = true;
        exitingSlope = false;
    }
    private bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * .5f + .2f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void IncreaseDoF()
    {
        bool increase = true;

        if(increase)
        {
            focalLengthValue = Mathf.Lerp(focalLengthValue, 40f, Time.deltaTime);
            dof.focalLength.Override(focalLengthValue);
        }

        print("increaseeee");

        if (focalLengthValue >= 35f)
        {
            increase = false;
            focalLengthValue = 35f;
            increaseDoF = false;
        }
    }

    void DecreaseDoF()
    {
        bool decrease = true;

        if(decrease)
        {
            focalLengthValue = Mathf.Lerp(focalLengthValue, -2f, 2 * Time.deltaTime);
            dof.focalLength.Override(focalLengthValue);
        }

        print("decreaseeee");


        if (focalLengthValue <= 1f)
        {
            decrease = false;
            focalLengthValue = 1;
            decreaseDoF = false;
        }
    }

}
