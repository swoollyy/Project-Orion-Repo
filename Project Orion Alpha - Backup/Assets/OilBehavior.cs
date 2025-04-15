using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBehavior : MonoBehaviour
{

    public float barrelOilTimer;

    public GameObject oilPool;

    public GameControllerLevel2 gc;

    int layerMask = 7;

    public bool startTimer;
    int oilCount;
    int rng;

    public bool canBePushed;

    // Start is called before the first frame update
    void Start()
    {
        rng = Random.Range(12, 41);
        canBePushed = true;
    }

    // Update is called once per frame
    void Update()
    {

        if(startTimer)
        {
            barrelOilTimer += Time.deltaTime;
        }
        if (barrelOilTimer >= .3f && oilCount < rng)
        {
            GameObject instOil = Instantiate(oilPool, gc.barrelHitPoint, Camera.main.transform.rotation);
            instOil.tag = "OilPool";
            instOil.layer = layerMask;
            instOil.GetComponent<Rigidbody>().AddForce(transform.forward * Random.Range(-.1f, 1.5f) + transform.up * Random.Range(-.25f, 1.5f) + transform.right * Random.Range(-.5f, 2f), ForceMode.Impulse);
            oilCount++;
            barrelOilTimer = 0f;
        }

        if (oilCount == rng)
        {
            ChangeProperties();
        }

    }
    public void SpawnOil()
    {
        startTimer = true;
    }

    void ChangeProperties()
    {
        GetComponent<Rigidbody>().mass = .5f;
        canBePushed = false;
    }



}
