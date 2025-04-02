using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionLizard : MonoBehaviour
{

    public StateController scorpController;

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
        if(col.gameObject.tag == "Lizard")
        {
            if(col.gameObject.GetComponent<LizardHealth>().currentHealth <= 0)
            {
                scorpController.trappedLizard = col.gameObject;
                scorpController.lizardInPosition = true;
            }
        }
    }

}
