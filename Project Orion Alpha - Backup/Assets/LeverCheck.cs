using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverCheck : MonoBehaviour
{

    public List<WaterRise> levers = new List<WaterRise>();

    public water waterScript;

    public int onCount;

    public bool finalSegment;


    // Start is called before the first frame update
    void Start()
    {
        levers[0] = transform.GetChild(0).GetComponent<WaterRise>();
        levers[1] = transform.GetChild(1).GetComponent<WaterRise>();
        levers[2] = transform.GetChild(2).GetComponent<WaterRise>();
        levers[3] = transform.GetChild(3).GetComponent<WaterRise>();


    }

    // Update is called once per frame
    void Update()
    {



        if(onCount == 4)
        {
            finalSegment = true;
            if (waterScript.isAtTop)
            {
                waterScript.startTopDescentToFloor = true;
            }
            else if (waterScript.isAtMiddle)
            {
                waterScript.startMidDescentToFloor = true;
            }
            else
            {
                onCount = 0;
                return;
            }
        }

    }

    public void UpdateCount()
    {
        onCount = 0;
        for(int i = 0; i < levers.Count; i++)
        {
            if(levers[i].isOn)
            {
                onCount++;
            }
        }
    }

}
