using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InOilZone : MonoBehaviour
{

    public Level2Text levelText;
    bool doOnce;
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
        if(col.gameObject.tag == "OilPool" && !doOnce && levelText.tutTextMaxIndex == 11)
        {
                levelText.oilReleased = true;
            doOnce = true;
        }
    }

}
