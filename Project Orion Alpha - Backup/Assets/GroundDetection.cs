using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{

    public LayerMask groundMask;

    float debugTimer;
    float waitTimer;

    // Start is called before the first frame update
    void Start()
    {
        debugTimer = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        waitTimer += Time.deltaTime;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, -transform.up, out hit, .5f, groundMask))
        {
            if(waitTimer >= .8f)
            {
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
            }
        }
    }

    void OnTriggerStay(Collider col)
    {
        debugTimer += Time.deltaTime;

        if(col.gameObject.name == "PlayerObj")
        {
                if(debugTimer >= 2f)
                {
                    col.gameObject.GetComponent<PlayerHealth>().TakeDamage(10);
                col.gameObject.GetComponent<PlayerHealth>().isOnFire = true;
                col.gameObject.GetComponent<PlayerHealth>().wasOnFire = true;
                debugTimer = 0f;
                }
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.name == "PlayerObj")
        {
            debugTimer = 0f;
            col.gameObject.GetComponent<PlayerHealth>().isOnFire = false;
        }
    }

}
