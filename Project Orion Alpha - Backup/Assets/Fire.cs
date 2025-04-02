using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{

    public List<GameObject> fire = new List<GameObject>();

    public GameObject cube;

    public LayerMask layerMask;

    public bool startFire = false;
    public bool rapidFire = false;

    public GameControllerLevel2 gc;

    float rapidFireTimer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (startFire)
        {
            if (fire.Count <= 15)
            {
                GameObject newFire = Instantiate(cube, transform.position, Quaternion.identity);
                fire.Add(newFire);
                newFire.GetComponent<Rigidbody>().AddForce(newFire.transform.forward * Random.Range(-15f, 15f) + newFire.transform.up * Random.Range(5f, 20f) + newFire.transform.right * Random.Range(-15f, 15f), ForceMode.Impulse);
            }
            else
            {
                fire.Clear();
                startFire = false;
            }
        }

        if(rapidFire)
        {
            rapidFireTimer += Time.deltaTime;
            if(rapidFireTimer >= 2.5f)
            {
                int rng = Random.Range(2, 10);
                for(int i = 0; i < rng; i++)
                {
                    GameObject newFire = Instantiate(cube, transform.position, Quaternion.identity);
                    newFire.GetComponent<Rigidbody>().AddForce(newFire.transform.forward * Random.Range(-15f, 15f) + newFire.transform.up * Random.Range(5f, 20f) + newFire.transform.right * Random.Range(-15f, 15f), ForceMode.Impulse);
                }
                rapidFireTimer = 0f;
            }

        }


    }
}
