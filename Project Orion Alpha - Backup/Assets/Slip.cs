using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slip : MonoBehaviour
{

    public LayerMask allButPlayerLayer;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {

        if(col.gameObject.tag == "Terrain" || col.gameObject.tag == "Floor")
        {
            GetComponent<BoxCollider>().isTrigger = false;
            GetComponent<BoxCollider>().excludeLayers = allButPlayerLayer;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        }


    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<PlayerMovementLevel2>().isOily = true;
            Destroy(this.gameObject);
        }
    }

}
