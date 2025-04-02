using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float mouseSensitivity = 3; // 100 sufficient in video lesson
    private float xRotation = 0f;
    private float yRotation = 0f;
    public Camera cam;
    public Transform playerBody;
    Vector3 reference;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
        xRotation -= mouseY;
        yRotation += mouseX;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
