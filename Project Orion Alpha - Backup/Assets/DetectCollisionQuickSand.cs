using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionQuickSand : MonoBehaviour
{

    public Suction succ;

    public GameObject dustParticle1;

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


        if (col.gameObject.tag == "Player" && succ.gravityCoroutine == null && col.gameObject.GetComponent<Inventory>().isHoldingLizard && dustParticle1.activeSelf)
        {
            succ.timer = 0;
            succ.beginSuckageTimer = true;
            succ.player.gameObject.GetComponent<Collider>().material = succ.slideyMat;
            succ.gravityCoroutine = StartCoroutine(succ.ApplyGravity());
            succ.player.gameObject.GetComponent<PlayerMovement>().isBeingSucked = true;
        }
    }
}
