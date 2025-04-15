using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    MonsterNav navScript;

    bool detecting;

    // Start is called before the first frame update
    void Start()
    {
        navScript = transform.parent.GetComponent<MonsterNav>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(detecting)
            navScript.isInWater = true;
        else
            navScript.isInWater = false;
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "RigDetection")
        {
            if(navScript.isOnWater || navScript.isInWater)
            navScript.isAbleToJump = true;
        }


        if (col.gameObject.tag == "Water")
        {
            detecting = true;
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "RigDetection")
        {
            if (navScript.isOnWater)
                navScript.isAbleToJump = false;
        }



        if (col.gameObject.tag == "Water")
        {
            detecting = false;
        }
    }

}
