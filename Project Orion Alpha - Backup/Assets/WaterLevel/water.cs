using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class water : MonoBehaviour
{
    public float minSizeFloor = 1f;
    public float maxSizeFloor = 50f;
    public float minSizeMiddle = 50f;
    public float maxSizeMiddle = 80f;



    public float rate = 1f;


    public GameObject waterObj;

    public LeverCheck leverCheck;

    public bool isMax = false;

    public bool startFloorRise = false;
    public bool startMiddleRise = false;
    public bool startTopDescentToMid = false;
    public bool startMidDescentToFloor = false;
    public bool startTopDescentToFloor = false;
    public bool startFloorToTop = false;

    public bool isAtFloor;
    public bool isAtMiddle;
    public bool isAtTop;

    public bool isMoving;


    public float timer;
    public float randomMoveTimer;
    public float riseTime = 1f;

    public float randomMax;

    private void Start()
    {
        waterObj.transform.localScale = new Vector3(transform.localScale.x, minSizeFloor, transform.localScale.z);
        isAtFloor = true;
        randomMax = Random.Range(25f, 65f);
    }

    private void Update()
    {
        if (!isMoving)
        {
            if (randomMoveTimer <= 0)
            {
                if (isAtTop)
                    randomMax = Random.Range(15f, 30f);
                else
                    randomMax = Random.Range(25f, 65f);
            }
            randomMoveTimer += Time.deltaTime;

        }

        if (randomMoveTimer >= randomMax && !leverCheck.finalSegment && !isMoving)
        {
            int rng = Random.Range(1, 3);
            if(isAtFloor)
            {
                if (rng == 1)
                {
                    startFloorRise = true;
                }
                else startFloorToTop = true;
            }
            else if(isAtMiddle)
            {
                if (rng == 1)
                {
                    startMidDescentToFloor = true;
                }
                else startMiddleRise = true;
            }
            else if(isAtTop)
            {
                if (rng == 1)
                {
                    startTopDescentToMid = true;
                }
                else startTopDescentToFloor = true;
            }
            randomMoveTimer = -.5f;
        }


        if(startFloorRise)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(RiseToMiddle());
            startFloorRise = false;
        }
        if (startMiddleRise)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(RiseToTop());
            startMiddleRise = false;
        }
        if (startTopDescentToMid)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(FallToMid());
            startTopDescentToMid = false;
        }
        if (startMidDescentToFloor)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(FallToFloor());
            startMidDescentToFloor = false;
        }
        if (startFloorToTop)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(RiseFromFloorToTop());
            startFloorToTop = false;
        }
        if (startTopDescentToFloor)
        {
            timer = 0;
            randomMoveTimer = 0;
            StartCoroutine(FallToFloorFromTop());
            startTopDescentToFloor = false;
        }


    }

    private IEnumerator RiseToMiddle()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, minSizeFloor, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, maxSizeFloor, waterObj.transform.localScale.z);


        do
        {
            isMoving = true;
            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / riseTime);
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);

        isMoving = false;
        isAtMiddle = true;
        isAtFloor = false;
        isAtTop = false;


        //isMax = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }

    private IEnumerator RiseToTop()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, minSizeMiddle, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, maxSizeMiddle, waterObj.transform.localScale.z);

        do
        {
            isMoving = true;
            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / riseTime);
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);

        isMoving = false;
        isAtTop = true;
        isAtMiddle = false;
        isAtFloor = false;

        //isMax = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }

    private IEnumerator RiseFromFloorToTop()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, minSizeFloor, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, maxSizeMiddle, waterObj.transform.localScale.z);

        do
        {
            isMoving = true;

            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / (riseTime * 2));
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < (riseTime *2));

        isMoving = false;
        isAtTop = true;
        isAtMiddle = false;
        isAtFloor = false;


        //isMax = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }

    private IEnumerator FallToMid()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, maxSizeMiddle, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, minSizeMiddle, waterObj.transform.localScale.z);

        do
        {
            isMoving = true;

            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / riseTime);
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);

        isMoving = false;
        isAtMiddle = true;
        isAtTop = false;
        isAtFloor = false;

        //isMax = false;

        /*if (!isMax)
        {
            timer = 0f;
            StartCoroutine(Rise());
        }*/
    }
    private IEnumerator FallToFloor()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, maxSizeFloor, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, minSizeFloor, waterObj.transform.localScale.z);

        do
        {
            isMoving = true;

            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / riseTime);
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < riseTime);

        isMoving = false;
        isAtFloor = true;
        isAtMiddle = false;
        isAtTop = false;

        //isMax = false;

        /*if (!isMax)
        {
            timer = 0f;
            StartCoroutine(Rise());
        }*/
    }
    private IEnumerator FallToFloorFromTop()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, maxSizeMiddle, waterObj.transform.localScale.z);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, minSizeFloor, waterObj.transform.localScale.z);

        do
        {
            isMoving = true;

            transform.localScale = Vector3.Lerp(minScale, maxScale, timer / (riseTime * 2));
            timer += Time.deltaTime;
            yield return null;
        }

        while (timer < (riseTime * 2));

        isMoving = false;
        isAtTop = false;
        isAtMiddle = false;
        isAtFloor = true;


        //isMax = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }



}
