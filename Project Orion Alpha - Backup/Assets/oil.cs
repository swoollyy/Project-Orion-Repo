using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class oil : MonoBehaviour
{
    public float minSizeFloor;
    public float maxSizeFloor = 50f;

    public GameControllerLevel2 gc;

    public float rate = 1f;


    public GameObject waterObj;

    public WaterRise water;

    public bool isEmpty = false;
    public bool isFull = false;

    public bool startToFall;
    public bool startToRise;

    public bool isAtFloor;
    public bool isAtMiddle;
    public bool isAtTop;


    public float timer;
    public float emptyTimer;
    public float riseTime = 3f;

    private void Start()
    {
        waterObj.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        isAtFloor = true;
    }

    private void Update()
    {
        if (startToFall)
        {
            isFull = false;
            timer = 0;
            StartCoroutine(OilFall());
            startToFall = false;
        }
        if (startToRise)
        {
            isEmpty = false;
            timer = 0;
            StartCoroutine(OilRise());
            startToRise = false;
        }

    }

    private IEnumerator OilFall()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, 0f, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, 0.2553418f, waterObj.transform.localScale.z);
        float minZ = 0f;
        float maxZ = 0.318f;


        do
        {
            transform.localScale = Vector3.Lerp(maxScale, minScale, timer / riseTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(maxZ, minZ, timer / riseTime));
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);


        isEmpty = true;
        gc.UpdateOilTankCount();

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }
    private IEnumerator OilRise()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, 0.2553418f, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, 0f, waterObj.transform.localScale.z);
        float minZ = 0.318f;
        float maxZ = 0;

        do
        {
            transform.localScale = Vector3.Lerp(maxScale, minScale, timer / riseTime);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(maxZ, minZ, timer / riseTime));
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);


        isFull = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }
}
