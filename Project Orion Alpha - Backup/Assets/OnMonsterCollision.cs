using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnMonsterCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Enemy"))
            GetComponent<Rigidbody>().isKinematic = false;
    }
}
