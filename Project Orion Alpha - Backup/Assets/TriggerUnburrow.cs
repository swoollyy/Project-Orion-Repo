using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerUnburrow : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            if(col.gameObject.GetComponent<StateController>().currentState ==
                col.gameObject.GetComponent<StateController>().endBurrowState)
            col.gameObject.GetComponent<StateController>().triggerUnburrow = true;
        }
    }


}
