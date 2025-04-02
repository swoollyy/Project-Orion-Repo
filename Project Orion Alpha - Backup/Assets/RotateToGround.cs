using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGround : MonoBehaviour
{

    public LayerMask groundMask;
    float timer;

    public GameObject ameture;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit rayHit;
            if (Physics.Raycast(ameture.transform.position, -transform.up, out rayHit, Mathf.Infinity, groundMask))
            {
                transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation, 2
            * Time.deltaTime);
            }

        timer += Time.deltaTime;
            if(timer >= 3f)
        {
            Quaternion currentRotation = transform.localRotation;

            Vector3 currentEulerAngles = currentRotation.eulerAngles;

            Quaternion newRotation = Quaternion.Euler(currentEulerAngles.x, 180f, currentEulerAngles.z);

            transform.localRotation = newRotation;
            timer = 0;
        }

    }

    public void RotateNow()
    {

        transform.eulerAngles = new Vector3(0f, 0f, 0f);
    }
}
