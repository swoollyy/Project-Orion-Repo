using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeCollider : MonoBehaviour
{

    public GameObject knifeHands;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Camera.main.transform.rotation;
        if(knifeHands.activeSelf)
        {
            GetComponent<SphereCollider>().enabled = true;
        }
        else
            GetComponent<SphereCollider>().enabled = false;
    }
}
