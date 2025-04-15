using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterRise : MonoBehaviour
{

    public water waterScript;
    public oil oilScript;
    public oilWall oilWallScript;
    public Fire fireSpawner;
    public MonsterNav monsterNav;

    bool doOnce;

    public bool isOn = false;
    public bool isOff = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Flip()
    {
        if(!doOnce)
        {
            isOff = !isOff;
            isOn = !isOn;
            transform.parent.GetComponent<LeverCheck>().UpdateCount();
            fireSpawner.startFire = true;
            if (!monsterNav.isOnLand)
                monsterNav.ForceLandJump();

            if (!GetComponent<AudioSource>().isPlaying)
                GetComponent<AudioSource>().Play();




            if (isOn && !isOff)
            {
                oilScript.startToFall = true;
                oilWallScript.startToFall = true;
                /*if(waterScript.isAtFloor)
                {
                    waterScript.startFloorToTop = true;
                }
                if (waterScript.isAtMiddle)
                {
                    waterScript.startFloorToTop = true;
                }*/
            }
            doOnce = true;
        }


        if (!isOn && isOff)
        {
            //waterScript.startTopDescentToFloor = true;
        }

    }

}
