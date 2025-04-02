using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToGroundLizard : MonoBehaviour
{

    public LayerMask groundMask;

    public GameObject centerRef;

    float timer; 
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(centerRef.transform.position, -transform.up, out rayHit, Mathf.Infinity, groundMask))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation,
        Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation, 2
        * Time.deltaTime);
            transform.position = (transform.parent.transform.position * .75f) + (rayHit.point * .25f);
        }

        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            Quaternion currentRotation = transform.localRotation;

            Vector3 currentEulerAngles = currentRotation.eulerAngles;
            transform.localPosition = Vector3.zero;
            Quaternion newRotation = Quaternion.Euler(currentEulerAngles.x, 90f, currentEulerAngles.z);

            transform.localRotation = newRotation;
            timer = 0;
        }

    }
}
