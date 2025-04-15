using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionQuickSandExit : MonoBehaviour
{

    public Suction succ;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerExit(Collider col)
    {

        if (col.gameObject.tag == "Player")
        {
            succ.beginSuckageTimer = false;
            succ.beginSuckage = false;
            succ.scorpController.succ = null;
            succ.player.gameObject.GetComponent<Collider>().material = null;
            if(col.gameObject.GetComponent<Inventory>().isHoldingLizard)
            succ.player.gameObject.GetComponent<PlayerMovement>().hasEscaped = true;
            succ.player.gameObject.GetComponent<PlayerMovement>().isBeingSucked = false;
            succ.scorpController.watchCases = false;
            succ.doOnce = false;
            succ.timer = 0;
        }
    }

    void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player" && succ.gravityCoroutine == null && col.gameObject.GetComponent<Inventory>().isHoldingLizard)
        {
            if(succ.beginSuckage)
            succ.beginSuckageTimer = true;
        }
    }

}
