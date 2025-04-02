using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMovement : MonoBehaviour
{
    public Camera cam;
    public Camera revCam;
    public Transform target;
    GameObject player;
    Rigidbody rb;
    public Collider mainCol;
    public Collider crouchCol;
    bool forward = false;
    bool left = false;
    bool right = false;
    bool back = false;
    public bool sneak = false;
    public GameObject burrowTracker;
    float latSpeed;
    float longSpeed;
    float diagSpeed;
    float diagRef = 4f;
    public GroundCheck gCheck;
    Vector3 movementVelocity;
    Vector3 diagMovementVelocity;
    float gravity = 9.81f;
    float reference;
    public GameObject center;
    public GameObject crouch;
    Vector3 targetPosition;
    Vector3 vecReference;
    bool isDiagLeft;
    bool isDiagRight;
    bool isHoldingW;
    bool isHoldingA;
    bool isHoldingS;
    bool isHoldingD;
    bool isRunning;
    bool isJumping;
    bool isSneaking;
    public bool isCrouching;
    Vector3 smoothJump;
    Vector3 smoothFall;
    bool onlyOnce;
    int once = 0;
    float onceTimer;
    float currentSpeed;
    public enum MovementMode
    {
        Idle,
        Walk,
        WalkLeft,
        WalkRight,
        WalkBackward,
        WalkDiagLeft,
        WalkDiagRight,
        WalkDiagLeftBack,
        WalkDiagRightBack,
        Run,
        RunLeft,
        RunRight,
        RunBackward,
        RunDiagLeft,
        RunDiagRight,
        RunDiagLeftBack,
        RunDiagRightBack,
        SlowDown,
        SlowDownDiag
    }

    public MovementMode currentMovementMode = MovementMode.Idle;


    // Start is called before the first frame update
    void Start()
    {
        revCam.enabled = false;
        player = GameObject.FindWithTag("Player").GetComponent<GameObject>();
        rb = GameObject.FindWithTag("Player").GetComponent<Rigidbody>();
        latSpeed = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isCrouching && currentMovementMode == MovementMode.Idle)
        {
            targetPosition = crouch.transform.position;
            cam.transform.position = Vector3.SmoothDamp(cam.transform.position, targetPosition, ref vecReference, .3f);

        }
        else cam.transform.position = transform.position;
        print(currentMovementMode);
        transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        if (Input.GetKeyDown("w"))
        {
            isHoldingW = true;
        }
        if (Input.GetKeyDown("a"))
        {
            isHoldingA = true;
        }
        if (Input.GetKeyDown("s"))
        {
            isHoldingS = true;
        }
        if (Input.GetKeyDown("d"))
        {
            isHoldingD = true;
        }
        if (Input.GetKeyDown("c"))
        {
            isCrouching = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyDown(KeyCode.Space) && gCheck.onGround)
            isJumping = true;
        if (Input.GetKeyDown(KeyCode.LeftControl))
            isSneaking = true;
        if (Input.GetKeyUp("w"))
        {
            isHoldingW = false;
        }
        if (Input.GetKeyUp("a"))
        {
            isHoldingA = false;
        }
        if (Input.GetKeyUp("s"))
        {
            isHoldingS = false;
        }
        if (Input.GetKeyUp("d"))
        {
            isHoldingD = false;
        }
        if (Input.GetKeyUp("c"))
        {
            isCrouching = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
        if (Input.GetKeyUp(KeyCode.Space))
            isJumping = false;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            isSneaking = false;
        if (isJumping)
        {
            rb.AddForce(transform.up * 15f);
        }
        if (rb.velocity.y > 2.2f)
            isJumping = false;

        if (isCrouching)
        {
            mainCol.enabled = false;
            crouchCol.enabled = true;
        }
        if (!isCrouching)
        {
            mainCol.enabled = true;
            crouchCol.enabled = false;
        }

        switch (currentMovementMode)
        {
            case MovementMode.Idle:
                {
                    if (isHoldingW)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (isHoldingA)
                    {
                        currentMovementMode = MovementMode.WalkLeft;
                    }
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkBackward;
                    }
                    if (isHoldingD)
                    {
                        currentMovementMode = MovementMode.WalkRight;
                    }
                    break;
                }
            case MovementMode.Walk:
                {
                    currentSpeed = Vector3.Magnitude(rb.velocity);
                    latSpeed = 4f;
                    if(currentSpeed < 4f)
                    {
                        rb.velocity += transform.forward * currentSpeed;
                    }
                }
                break;
        }
    }
}

