using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilDrops : MonoBehaviour
{

    float timer;

    public List<GameObject> oilLocations = new List<GameObject>();

    public GameObject oil;

    int rngToStart;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 3)
        {
            rngToStart = Random.Range(1, 3);
            if (rngToStart == 1 && transform.parent.GetComponent<MonsterNav>().isOnLand)
            {
                DropOil();
            }
            else
            {
                rngToStart = 0;
                timer = 0;
            }
        }
    }


    void DropOil()
    {
        timer = 0;
        int rng = Random.Range(1, 5);

        for(int i = 0; i < oilLocations.Count; i++)
        {
            if(rng == i)
            {

                int rng2 = Random.Range(1, 3);
                GameObject instOil = Instantiate(oil, oilLocations[i].transform.position, Quaternion.identity);
                if (rng2 == 1)
                {
                    instOil.GetComponent<Rigidbody>().AddForce(oilLocations[i].transform.forward * 3f + transform.up * 12f, ForceMode.Impulse);
                    rngToStart = 0;
                }
                else
                {
                    instOil.GetComponent<Rigidbody>().AddForce(-oilLocations[i].transform.forward * 3f + transform.up * 12f, ForceMode.Impulse);
                    rngToStart = 0;
                }
            }
        }
    }

}
