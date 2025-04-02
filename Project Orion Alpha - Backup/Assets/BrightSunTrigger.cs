using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightSunTrigger : MonoBehaviour
{


    public bool enableBrightSunAnimation;
    public GameController gc;

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
        print(col.gameObject.tag);
        if(col.gameObject.name == "PlayerObj" && gc.openedLocker && gc.playerInv.collectedGun && gc.playerInv.collectedKnife)
        {
            enableBrightSunAnimation = true;
        }
    }

}
