using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObjectMovement : MonoBehaviour
{

    public GameControllerLevel2 gc;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    public Transform camHolder;

    public Camera cam;

    [Header("Movement")]
    public float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode sprintKey = KeyCode.LeftShift;

    [Header("State Checks")]
    public bool isSprinting;
    public bool isWalking;
    public bool isIdle;

    public bool walkBob;
    public bool sprintBob;

    Quaternion quat;
    Vector3 vect;

    Rigidbody rb;

    public MovementState currentState;
    public enum MovementState
    {
        idle,
        walking,
        sprinting
    }


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<BoxCollider>().size = new Vector3(1.15f, 1.97f, .97f);
        GetComponent<BoxCollider>().center = new Vector3(0f, 0f, 1.01f);
        rb = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        MovePlayer();
        StateHandler();


        vect = new Vector3(0f, cam.transform.eulerAngles.y, 0f);
        quat.eulerAngles = vect;
        transform.rotation = quat;

        gc.lockedBarrel.transform.parent = this.transform;


    }

    void PlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
        rb.velocity = Vector3.Lerp(rb.velocity, moveDirection.normalized * moveSpeed * 8f, .5f * Time.deltaTime);
    }

    void StateHandler()
    {
            if ((horizontalInput != 0 || verticalInput != 0))
            {
                currentState = MovementState.walking;
                moveSpeed = walkSpeed;
            }
            else if ((horizontalInput == 0 && verticalInput == 0))
            {
                currentState = MovementState.idle;
            }

            if (currentState == MovementState.walking)
            {
                isWalking = true;
                sprintBob = true;
            }
            else
            {
                isWalking = false;
    sprintBob = false;
            }
            if (currentState == MovementState.idle)
            {
                isIdle = true;
            }
            else
                isIdle = false;
        }

}
