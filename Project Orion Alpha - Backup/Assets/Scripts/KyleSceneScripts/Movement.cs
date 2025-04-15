using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    public GameObject player;
    public Rigidbody rb;
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
    public Vector3 movementVelocity;
    Vector3 sideMovementVelocity;
    Vector3 diagMovementVelocity;
    float gravity = 9.81f;
    float reference;
    float otherreference;
    public GameObject center;
    public GameObject crouch;
    Vector3 targetPosition;
    Vector3 vecReference;
    bool isDiagLeft;
    bool isDiagRight;
    public bool isHoldingW;
    public bool isHoldingA;
    public bool isHoldingS;
    public bool isHoldingD;
    public bool isRunning;
    public bool isJumping;
    public bool isSneaking;
    public bool isCrouching;
    Vector3 smoothJump;
    Vector3 smoothFall;
    bool onlyOnce;
    int once = 0;
    float onceTimer;
    float camShakeTimer;
    bool increase;
    bool decrease;
    float yTransform;
    float bobbing;
    float currentSpeed;
    public bool hasJumped;
    public bool jumpdebugTimerStart;
    public float jumpDebugTimer;
    public Animator knifeAnim;
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
        Crouch,
        CrouchWalk,
        CrouchWalkLeft,
        CrouchWalkRight,
        CrouchWalkBack,
        CrouchWalkDiagLeft,
        CrouchWalkDiagRight,
        CrouchWalkDiagLeftBack,
        CrouchWalkDiagRightBack
    }

    public MovementMode currentMovementMode = MovementMode.Idle;


    // Start is called before the first frame update
    void Start()
    {
        latSpeed = 0f;
        yTransform = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        burrowTracker.transform.position = new Vector3(transform.position.x, transform.position.y - 5.8f, transform.position.z); ;
        transform.rotation = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f);
        if (Input.GetKeyDown("w"))
        {
            isHoldingW = true;
            knifeAnim.SetBool("IsWalking", true);
        }
        if (Input.GetKeyDown("a"))
        {
            isHoldingA = true;
            knifeAnim.SetBool("IsWalking", true);
        }
        if (Input.GetKeyDown("s"))
        {
            isHoldingS = true;
            knifeAnim.SetBool("IsWalking", true);
        }
        if (Input.GetKeyDown("d"))
        {
            isHoldingD = true;
            knifeAnim.SetBool("IsWalking", true);
        }
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isCrouching = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyDown(KeyCode.Space) && gCheck.onGround)
        {
            isJumping = true;
            jumpdebugTimerStart = true;
        }
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
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isCrouching = false;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
        if (Input.GetKeyUp(KeyCode.LeftControl))
            isSneaking = false;
        if (jumpdebugTimerStart)
            jumpDebugTimer += Time.deltaTime;
        if (isJumping)
        {
            rb.AddForce(transform.up * 50f);
        }
        if (jumpDebugTimer >= .2f)
            hasJumped = true;
        if (rb.velocity.y > 3.1f || Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
            jumpDebugTimer = 0;
        }

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
                    knifeAnim.SetBool("IsWalking", false);
                    bobbing = 0f;
                    camShakeTimer = 0f;
                    cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                    longSpeed = Mathf.SmoothDamp(longSpeed, 0f, ref reference, Time.deltaTime * 9f);
                    latSpeed = Mathf.SmoothDamp(latSpeed, 0f, ref reference, Time.deltaTime * 9f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime) * .5f);
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
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.Crouch:
                {
                    cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y - .4f, ref reference, 4f * Time.deltaTime), transform.position.z);

                    if (!isCrouching)
                        currentMovementMode = MovementMode.Idle;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.CrouchWalk;
                    if (isHoldingA)
                        currentMovementMode = MovementMode.CrouchWalkLeft;
                    if (isHoldingS)
                        currentMovementMode = MovementMode.CrouchWalkBack;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.CrouchWalkRight;
                    break;
                }
            case MovementMode.CrouchWalk:
                {
                    CameraBobCrouch();
                    longSpeed = 0;
                    latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = transform.forward * latSpeed;
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingW && !isCrouching) || !isHoldingW && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingW && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (isHoldingA)
                        currentMovementMode = MovementMode.CrouchWalkDiagLeft;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.CrouchWalkDiagRight;
                    break;
                }
            case MovementMode.CrouchWalkLeft:
                {
                    CameraBobCrouch();
                    latSpeed = 0;
                    longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingA && !isCrouching) || !isHoldingA && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingA && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (isHoldingD)
                        currentMovementMode = MovementMode.CrouchWalkRight;
                    if (isHoldingW)
                    {
                        currentMovementMode = MovementMode.CrouchWalkDiagLeft;
                    }
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.CrouchWalkDiagLeftBack;
                    }
                    break;
                }
            case MovementMode.CrouchWalkRight:
                {
                    CameraBobCrouch();
                    latSpeed = 0;
                    longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingD && !isCrouching) || !isHoldingD && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingD && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (isHoldingA)
                        currentMovementMode = MovementMode.CrouchWalkLeft;
                    if (isHoldingW)
                    {
                        currentMovementMode = MovementMode.CrouchWalkDiagRight;
                    }
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.CrouchWalkDiagRightBack;
                    }
                    break;
                }
            case MovementMode.WalkDiagLeft:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }

                    if (isSneaking)
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    }
                    else
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, 3f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, -3f, ref otherreference, Time.deltaTime * 10f);
                    }
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if (!isHoldingA && !isHoldingW)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (!isHoldingA && isHoldingW)
                        currentMovementMode = MovementMode.Walk;
                    if (isHoldingA && !isHoldingW)
                        currentMovementMode = MovementMode.WalkLeft;
                    if (isRunning)
                        currentMovementMode = MovementMode.RunDiagLeft;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;

                }
            case MovementMode.CrouchWalkDiagLeft:
                {
                    CameraBobCrouch();
                    latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingA && !isCrouching) || !isHoldingA && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingA && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (!isHoldingW)
                        currentMovementMode = MovementMode.CrouchWalkLeft;
                    break;
                }
            case MovementMode.CrouchWalkDiagRight:
                {
                    CameraBobCrouch();
                    latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingD && !isCrouching) || !isHoldingD && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingD && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (!isHoldingW)
                        currentMovementMode = MovementMode.CrouchWalkRight;
                    break;
                }
            case MovementMode.CrouchWalkDiagLeftBack:
                {
                    CameraBobCrouch();
                    latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingA && !isCrouching) || !isHoldingA && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingA && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (!isHoldingS)
                        currentMovementMode = MovementMode.CrouchWalkLeft;
                    break;
                }
            case MovementMode.CrouchWalkDiagRightBack:
                {
                    CameraBobCrouch();
                    latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingD && !isCrouching) || !isHoldingD && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingD && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (!isHoldingS)
                        currentMovementMode = MovementMode.CrouchWalkRight;
                    break;
                }
            case MovementMode.CrouchWalkBack:
                {
                    CameraBobCrouch();
                    longSpeed = 0;
                    latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = transform.forward * latSpeed;
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if ((!isHoldingS && !isCrouching) || !isHoldingS && isCrouching)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isHoldingS && !isCrouching)
                    {
                        currentMovementMode = MovementMode.Walk;
                    }
                    if (isHoldingA)
                        currentMovementMode = MovementMode.CrouchWalkDiagLeftBack;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.CrouchWalkDiagRightBack;
                    break;
                }
            case MovementMode.Walk:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0f;
                    }
                    longSpeed = 0;
                    if (isSneaking)
                        latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                    else latSpeed = Mathf.SmoothDamp(latSpeed, 3f, ref reference, Time.deltaTime * 10f);

                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = transform.forward * latSpeed;
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (isHoldingA)
                    {
                        currentMovementMode = MovementMode.WalkDiagLeft;
                    }
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkBackward;
                    }
                    if (isHoldingD)
                    {
                        currentMovementMode = MovementMode.WalkDiagRight;
                    }
                    if (isRunning)
                    {
                        currentMovementMode = MovementMode.Run;
                    }
                    if (!isHoldingW)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.WalkBackward:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    longSpeed = 0;
                    if (isSneaking)
                        latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                    else latSpeed = Mathf.SmoothDamp(latSpeed, -3f, ref reference, Time.deltaTime * 10f);
                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = (transform.forward * latSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (isRunning)
                        currentMovementMode = MovementMode.RunBackward;
                    if (isHoldingA)
                        currentMovementMode = MovementMode.WalkDiagLeftBack;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.WalkDiagRightBack;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.Walk;
                    if (!isHoldingS)
                        currentMovementMode = MovementMode.Idle;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.WalkLeft:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = 0;
                    if (isSneaking)
                        longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                    else longSpeed = Mathf.SmoothDamp(longSpeed, -3f, ref reference, Time.deltaTime * 10f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (isRunning)
                        currentMovementMode = MovementMode.RunLeft;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.WalkDiagLeft;
                    if (!isHoldingA)
                        currentMovementMode = MovementMode.Idle;
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkDiagLeftBack;
                    }
                    if (isHoldingD)
                        currentMovementMode = MovementMode.WalkRight;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.RunDiagLeft:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = Mathf.SmoothDamp(latSpeed, 6f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, -6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkDiagLeft;
                    if (!isHoldingA)
                        currentMovementMode = MovementMode.Run;
                    if (!isHoldingW)
                        currentMovementMode = MovementMode.RunLeft;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.RunDiagRight:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = Mathf.SmoothDamp(latSpeed, 6f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, 6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkDiagRight;
                    if (!isHoldingD)
                        currentMovementMode = MovementMode.Run;
                    if (!isHoldingW)
                        currentMovementMode = MovementMode.RunRight;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.WalkRight:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = 0;
                    if (isSneaking)
                        longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                    else longSpeed = Mathf.SmoothDamp(longSpeed, 3f, ref reference, Time.deltaTime * 10f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (isRunning)
                        currentMovementMode = MovementMode.RunRight;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.WalkDiagRight;
                    if (isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkDiagRightBack;
                    }
                    if (isHoldingA)
                        currentMovementMode = MovementMode.WalkLeft;
                    if (!isHoldingD)
                        currentMovementMode = MovementMode.Idle;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.WalkDiagRight:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    if (isSneaking)
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, 1.5f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    }
                    else
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, 3f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, 3f, ref otherreference, Time.deltaTime * 10f);
                    }
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));
                    if (!isHoldingD && !isHoldingW)
                    {
                        currentMovementMode = MovementMode.Idle;
                    }
                    if (!isHoldingD && isHoldingW)
                        currentMovementMode = MovementMode.Walk;
                    if (isHoldingD && !isHoldingW)
                        currentMovementMode = MovementMode.WalkRight;
                    if (isRunning)
                        currentMovementMode = MovementMode.RunDiagRight;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;

                }

            case MovementMode.WalkDiagLeftBack:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    if (isSneaking)
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, -1.5f, ref reference, Time.deltaTime * 10f);

                    }
                    else
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, -3f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, -3f, ref otherreference, Time.deltaTime * 10f);
                    }
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));

                    if (!isHoldingA && isHoldingS)
                        currentMovementMode = MovementMode.WalkBackward;
                    if (!isHoldingA && !isHoldingS)
                        currentMovementMode = MovementMode.Idle;
                    if (isHoldingA && !isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkLeft;

                    }
                    if (isRunning)
                        currentMovementMode = MovementMode.RunDiagLeftBack;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;

                }
            case MovementMode.WalkDiagRightBack:
                {
                    if (gCheck.onGround && !isSneaking)
                        CameraBob();
                    else if (gCheck.onGround && isSneaking)
                        CameraBobSneak();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }

                    if (isSneaking)
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, -1.5f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, 1.5f, ref reference, Time.deltaTime * 10f);

                    }
                    else
                    {
                        latSpeed = Mathf.SmoothDamp(latSpeed, -3f, ref reference, Time.deltaTime * 10f);
                        longSpeed = Mathf.SmoothDamp(longSpeed, 3f, ref otherreference, Time.deltaTime * 10f);
                    }
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));

                    if (!isHoldingD && isHoldingS)
                        currentMovementMode = MovementMode.WalkBackward;
                    if (isHoldingD && !isHoldingS)
                    {
                        currentMovementMode = MovementMode.WalkRight;
                    }
                    if (!isHoldingD && !isHoldingS)
                        currentMovementMode = MovementMode.Idle;
                    if (isRunning)
                        currentMovementMode = MovementMode.RunDiagRightBack;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;

                }

            case MovementMode.Run:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    longSpeed = 0;
                    latSpeed = Mathf.SmoothDamp(latSpeed, 6f, ref reference, Time.deltaTime * 10f);
                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = transform.forward * latSpeed;
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.Walk;
                    if (isHoldingA)
                        currentMovementMode = MovementMode.RunDiagLeft;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.RunDiagRight;
                    if (isHoldingA && !isHoldingW)
                        currentMovementMode = MovementMode.RunLeft;
                    if (isHoldingD && !isHoldingW)
                        currentMovementMode = MovementMode.RunRight;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }

            case MovementMode.RunLeft:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        camShakeTimer = 0;
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = 0;
                    longSpeed = Mathf.SmoothDamp(longSpeed, -6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkLeft;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.RunDiagLeft;
                    if (isHoldingS)
                        currentMovementMode = MovementMode.RunDiagLeftBack;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.RunRight;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.RunRight:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        bobbing = 0;
                    }
                    latSpeed = 0;
                    longSpeed = Mathf.SmoothDamp(longSpeed, 6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = Vector3.SmoothDamp(movementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkRight;
                    if (isHoldingW)
                        currentMovementMode = MovementMode.RunDiagRight;
                    if (isHoldingS)
                        currentMovementMode = MovementMode.RunDiagRightBack;
                    if (isHoldingA)
                        currentMovementMode = MovementMode.RunLeft;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.RunBackward:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        camShakeTimer = 0;
                        bobbing = 0;
                    }
                    longSpeed = 0;
                    latSpeed = Mathf.SmoothDamp(latSpeed, -6f, ref reference, Time.deltaTime * 10f);
                    //diagSpeed = Mathf.SmoothDamp(diagSpeed, 0f, ref diagRef, Time.deltaTime * 25f);
                    //diagMovementVelocity = Vector3.SmoothDamp(diagMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 25f);
                    sideMovementVelocity = Vector3.SmoothDamp(sideMovementVelocity, new Vector3(0f, 0f, 0f), ref vecReference, Time.deltaTime * 10f);
                    movementVelocity = transform.forward * latSpeed;
                    transform.position += (movementVelocity * Time.deltaTime) + ((sideMovementVelocity * Time.deltaTime));
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkBackward;
                    if (isHoldingA)
                        currentMovementMode = MovementMode.RunDiagLeftBack;
                    if (isHoldingD)
                        currentMovementMode = MovementMode.RunDiagRightBack;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;
                }
            case MovementMode.RunDiagLeftBack:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        camShakeTimer = 0;
                        bobbing = 0;
                    }
                    latSpeed = Mathf.SmoothDamp(latSpeed, -6f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, -6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));

                    if (!isHoldingA && isHoldingS)
                        currentMovementMode = MovementMode.WalkBackward;
                    if (isHoldingA && !isHoldingS)
                        currentMovementMode = MovementMode.WalkLeft;
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkDiagLeftBack;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;
                    break;

                }
            case MovementMode.RunDiagRightBack:
                {
                    if (gCheck.onGround)
                        CameraBobRun();
                    else
                    {
                        cam.transform.position = new Vector3(transform.position.x, Mathf.SmoothDamp(cam.transform.position.y, transform.position.y + .5f, ref yTransform, 4f * Time.deltaTime), transform.position.z);
                        camShakeTimer = 0;
                        bobbing = 0;
                    }
                    latSpeed = Mathf.SmoothDamp(latSpeed, -6f, ref reference, Time.deltaTime * 10f);
                    longSpeed = Mathf.SmoothDamp(longSpeed, 6f, ref otherreference, Time.deltaTime * 10f);
                    movementVelocity = (transform.forward * latSpeed);
                    sideMovementVelocity = (transform.right * longSpeed);
                    transform.position += (((movementVelocity * Time.deltaTime) * .5f) + (sideMovementVelocity * Time.deltaTime));

                    if (!isHoldingD && isHoldingS)
                        currentMovementMode = MovementMode.WalkBackward;
                    if (isHoldingD && !isHoldingS)
                        currentMovementMode = MovementMode.WalkRight;
                    if (!isRunning)
                        currentMovementMode = MovementMode.WalkDiagRightBack;
                    if (isCrouching)
                        currentMovementMode = MovementMode.Crouch;

                    break;

                }
        }
        currentSpeed = Vector3.Magnitude(rb.velocity);
        /*if (currentSpeed > 0.0001f && !isCrouching)
        {
            DefaultCam();
            print("DOING");
        }*/
    }
    void CameraBob()
    {
        if (camShakeTimer <= .25f && !decrease)
        {
            increase = true;
            decrease = false;
        }
        if (increase)
        {
            bobbing += .002f;
            camShakeTimer += Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer > .25f)
        {
            increase = false;
            decrease = true;
        }
        if (decrease)
        {
            bobbing -= .002f;
            camShakeTimer -= Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer <= 0f)
            decrease = false;
    }
    void DefaultCam()
    {
        cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f), transform.position.z);
    }
    void CameraBobSneak()
    {
        if (camShakeTimer <= .35f && !decrease)
        {
            increase = true;
            decrease = false;
        }
        if (increase)
        {
            bobbing += .002f;
            camShakeTimer += Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer > .35f)
        {
            increase = false;
            decrease = true;
        }
        if (decrease)
        {
            bobbing -= .002f;
            camShakeTimer -= Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer <= 0f)
            decrease = false;
    }
    void CameraBobCrouch()
    {
        if (camShakeTimer <= .35f && !decrease)
        {
            increase = true;
            decrease = false;
        }
        if (increase)
        {
            bobbing += .002f;
            camShakeTimer += Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y - .4f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer > .35f)
        {
            increase = false;
            decrease = true;
        }
        if (decrease)
        {
            bobbing -= .002f;
            camShakeTimer -= Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y - .4f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer <= 0f)
            decrease = false;
    }
    void CameraBobRun()
    {
        if (camShakeTimer <= .2f && !decrease)
        {
            increase = true;
            decrease = false;
        }
        if (increase)
        {
            bobbing += .008f;
            camShakeTimer += Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer > .2f)
        {
            increase = false;
            decrease = true;
        }
        if (decrease)
        {
            bobbing -= .008f;
            camShakeTimer -= Time.deltaTime;
            cam.transform.position = new Vector3(transform.position.x, (transform.position.y + .5f) + -bobbing, transform.position.z);
        }
        if (camShakeTimer <= 0f)
            decrease = false;
    }
}

