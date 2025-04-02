using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;



public class PlayerMovementLevel2 : MonoBehaviour
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
    public bool isOily;
    public bool isOnFire;

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

    public Animator gunAnim;
    public Animator knifeAnim;
    public Animator camAnim;

    public Inventory2 inv;
    public GameControllerLevel2 gc2;
    public PlayerHealth playerHealth;


    Vector3 moveDirection;

    public VolumeProfile volumeProfile;
    DepthOfField dof;
    float focalLengthValue;


    Rigidbody rb;

    Camera cam;
    Quaternion quat;
    Vector3 vect;

    public LayerMask groundMask;

    public Vector3 slipDirection;
    public Vector3 storedInitVel;
    bool doOnce;
    bool doOnce2;

    bool startOilTimer = false;
    float oilTimer;
    float debugTimer;




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

        if (isOily)
        {
            if (!doOnce)
            {
                startOilTimer = true;
                doOnce = true;
            }
            groundDrag = .5f;


            RaycastHit capHit;
            Vector3 p1 = transform.position + Vector3.up * -GetComponent<CapsuleCollider>().height * 0.5f;
            Vector3 p2 = p1 + Vector3.up * GetComponent<CapsuleCollider>().height;

            if (Physics.CapsuleCast(p1, p2, .5f, transform.forward, out capHit, 1f))
                if (capHit.transform.tag == "Barrier" && rb.velocity.magnitude > 10f && !doOnce2)
                {
                    rb.AddForce(transform.up * 10f, ForceMode.VelocityChange);
                    doOnce2 = true;
                }


        }
        else groundDrag = 3.49f;

        if (doOnce2)
        {
            debugTimer += Time.deltaTime;

            if (debugTimer >= 1f)
            {
                isOily = false;
                playerHealth.wasOnFire = false;
                doOnce2 = false;
            }
        }
        else
            debugTimer = 0f;

        if (startOilTimer)
        {
            oilTimer += Time.deltaTime;

            if (oilTimer >= 9f)
            {
                isOily = false;
                doOnce = false;
            }
        }
        if (!isOily)
        {
            startOilTimer = false;
            oilTimer = 0;
        }

        print("velocity - " + rb.velocity);

        RaycastHit hit;
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

        if(gc2 != null)
        {
            if (Input.GetKeyDown(gc2.aimKey) && inv.isHoldingGun)
            {
                gunAnim.SetBool("IsAiming", true);

                camAnim.SetBool("StoppedAiming", false);
                camAnim.SetBool("IsAiming", true);


                decreaseDoF = false;
                increaseDoF = true;

            }
            if (Input.GetKeyUp(gc2.aimKey) && inv.isHoldingGun)
            {
                gunAnim.SetBool("IsAiming", false);
                camAnim.SetBool("IsAiming", false);
                camAnim.SetBool("StoppedAiming", true);

                increaseDoF = false;
                decreaseDoF = true;
            }
        }


        if (increaseDoF)
            IncreaseDoF();

        if (decreaseDoF)
            DecreaseDoF();

        if(gc2 != null)
        {
            if (!isJumping && !isCrouching)
            {
                gc2.playerCanPush = true;
            }
            else gc2.playerCanPush = false;
        }


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
        if(Input.GetKeyDown(crouchKey))
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
        if (Input.GetKey(crouchKey))
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

        if(isOily)
        {
                Quaternion quat = Quaternion.Euler(0, horizontalInput, 0);
                moveDirection = transform.forward * verticalInput + quat * moveDirection;
                if (moveDirection.x <= -90f)
                    moveDirection.x = -90f;
                else if (moveDirection.x >= 90f)
                    moveDirection.x = 90f;

                if (moveDirection.z<= -90f)
                    moveDirection.z = -90f;
                else if (moveDirection.z >= 90f)
                    moveDirection.z = 90f;

                print("move dir vel " + moveDirection);


            //grounded
            if (grounded)
            {
                    rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * moveSpeed * 5.5f, Time.deltaTime);
            }
        }
        else
        {
            moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;


            //grounded
            if (grounded)
            {
                    rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * moveSpeed * 15f, .5f * Time.deltaTime);
            }
            //in air
            else if (!grounded)
            {
                    rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * airMultiplier * moveSpeed, .7f * Time.deltaTime);

            }
        }

        //on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, GetSlopeMoveDirection() * moveSpeed, Time.deltaTime);
        }

    }

    private void SpeedControl()
    {




    }

    private void Jump()
    {
        exitingSlope = true;
        hasJumped = false;

        //reset y velocity
        if (isIdle)
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
