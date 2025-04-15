using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPos;
    public Transform cameraCrouchPos;
    public Transform cameraPosRef;
    public Transform cameraPosCrouchRef;

    [Range(.001f, 1f)]
    public float amount;
    [Range(1f, 30f)]
    public float frequency;
    [Range(0, 100f)]
    public float smooth;

    //sprint
    [Range(.001f, 1f)]
    public float sprintamount;
    [Range(1f, 30f)]
    public float sprintfrequency;
    [Range(0, 100f)]
    public float sprintsmooth;

    float sinAmountY;

    public PlayerMovement playerMovement;

    float crouchTime = 1.5f;
    float timeElapsed;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 inputVector = new Vector3(Input.GetAxisRaw("Vertical"), 0f, Input.GetAxisRaw("Horizontal"));
        Vector3 pos = Vector3.zero;
            if(playerMovement.walkBob)
            {
                amount = .81f;
                frequency = 10f;
                smooth = .23f;
            }
            else if (playerMovement.crouchBob)
            {
                amount = 1f;
                frequency = 13f;
                smooth = .6f;
            }
            else if (playerMovement.sprintBob)
            {
                amount = sprintamount;
                frequency = sprintfrequency;
                smooth = sprintsmooth;
            }
            pos.y += Mathf.Lerp(pos.y, Mathf.Sin(Time.time * frequency) * amount * 1.4f, smooth * Time.deltaTime);
            pos.x += Mathf.Lerp(pos.x, Mathf.Cos(Time.time * frequency / 2f) * amount * 1.6f, smooth * Time.deltaTime);
        if (inputVector.magnitude > 0 && !playerMovement.hasCrouched)
        {
            if (Vector3.Distance(cameraPos.position + pos, transform.position) <= 1.2f)
            {
                timeElapsed = 0;
                transform.position = Vector3.Lerp(transform.position, cameraPos.position + pos, 2f * Time.deltaTime);
            }
            else
            {
                transform.position = transform.position;
            }

        }
        else if (playerMovement.hasJumped)
        {
            float transY = transform.position.y;
            transY = Mathf.Lerp(transY, cameraPos.position.y, Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transY, transform.position.z);

        }
        
        if (playerMovement.hasCrouched)
        {
            timeElapsed += Time.deltaTime;
            float t = crouchTime / timeElapsed;
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, cameraCrouchPos.position.y, transform.position.z) + pos, 2.5f * Time.deltaTime);
        }

        if (playerMovement.grounded && inputVector.magnitude <= 0.3 && !playerMovement.hasCrouched)
        {
            print("doing");
            StopHeadbob();

        }

    }
    private void StopHeadbob()
    {
        timeElapsed = 0;
        transform.position = Vector3.Lerp(transform.position, cameraPosRef.position, 5 * Time.deltaTime);
    }
}
