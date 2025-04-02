using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering;

public class oilWall : MonoBehaviour
{
    public float minSizeFloor;
    public float maxSizeFloor = 50f;



    public float rate = 1f;


    public GameObject waterObj;
    public GameObject bigOil;

    public Transform oilSpawner;

    public WaterRise water;

    public bool isEmpty = false;
    public bool isFull = false;

    public bool startToFall;
    public bool startToRise;

    public bool isAtFloor;
    public bool isAtMiddle;
    public bool isAtTop;

    bool oilDrops;

    int rng;


    float oilDropTimer;

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
            rng = Random.Range(12, 20);
            oilDrops = true;
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
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, waterObj.transform.localScale.y, 0f);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, waterObj.transform.localScale.y, 1f);


        do
        {
            transform.localScale = Vector3.Lerp(maxScale, minScale, timer / riseTime);
            timer += Time.deltaTime;

            if (oilDrops)
            {
                    oilDropTimer += Time.deltaTime;
                    if (oilDropTimer >= .1f)
                    {
                        GameObject oil = Instantiate(bigOil, oilSpawner.transform.position, Quaternion.Euler(0f, 0f, 0f));
                        oil.GetComponent<Rigidbody>().AddForce(oilSpawner.transform.up * Random.Range(3f, 13f) + oilSpawner.transform.right * Random.Range(-12f, 12f) + oilSpawner.transform.forward * Random.Range(-3f, 16f), ForceMode.Impulse);
                        oilDropTimer = 0f;
                    }
            }

            yield return null;
        }

        while (timer < riseTime);

        oilDrops = false;
        isEmpty = true;

        /*if (isMax)
        {
            timer = 0f;
            StartCoroutine(Fall());
        }*/
    }
    private IEnumerator OilRise()
    {
        Vector3 minScale = new Vector3(waterObj.transform.localScale.x, waterObj.transform.localScale.y, 1f);
        Vector3 maxScale = new Vector3(waterObj.transform.localScale.x, waterObj.transform.localScale.y, 0f);


        do
        {
            transform.localScale = Vector3.Lerp(maxScale, minScale, timer / riseTime);
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
