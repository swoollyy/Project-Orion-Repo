using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    private float xRotation = 0f;
    private float yRotation = 0f;
    float floatref = 0f;
    Vector3 reference;


    public KnifeAnimControl2 KAC2;
    public KnifeAnimControl KAC;
    public PlayerMovementLevel2 mMent;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * sensY;
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);


        if (mMent != null)
        {
            if(!mMent.enabled)
                sensX = 30f;
            else sensX = 250f;
        }


    }
}
