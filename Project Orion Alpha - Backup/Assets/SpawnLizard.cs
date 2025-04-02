using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnLizard : MonoBehaviour
{

    public GameObject lizard;
    public GameObject spawner;
    public GameObject player;

    GameObject newLizard;

    bool doOnce = false;

    public GameController gc;

    public bool isDead;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(player.transform.position, transform.position) < 10f)
        {
            if(player.GetComponent<Inventory>().isHoldingFruit)
            {
                if(!doOnce)
                {
                    gc.lizardNestDialogue = true;
                    print("DOONCE!!!!");
                    newLizard = Instantiate(lizard, spawner.transform.position, spawner.transform.rotation);
                    newLizard.name = "Brown Lizard";
                    isDead = false;
                    doOnce = true;
                }
            }
        }

        if (isDead)
        {
            print("RESET DOONCE");
            doOnce = false;
        }
        print("DOONCE " + doOnce);


    }
}
