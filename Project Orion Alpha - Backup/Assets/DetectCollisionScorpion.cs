using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionScorpion : MonoBehaviour
{

    public Animator archway;
    bool disableDeathAnim;
    float disableDeath;

    GameObject scorp;

    // Start is called before the first frame update
    void Start()
    {
        disableDeathAnim = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(disableDeathAnim)
        {
            disableDeath += Time.deltaTime;
            print("doin it");

            if (disableDeath >= .5f)
            {
                scorp.transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", false);
                print("doin it hard");
                disableDeath = 0;
                disableDeathAnim = false;
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Enemy")
        {
            scorp = col.gameObject;
            scorp.transform.GetChild(0).GetComponent<Animator>().SetBool("IsEating", false);
            scorp.transform.GetChild(0).GetComponent<Animator>().SetBool("IsDead", true);
            archway.speed = 0f;
            scorp.GetComponent<BoxCollider>().isTrigger = false;
            scorp.GetComponent<StateController>().gc.deadScorpion = true;
            scorp.GetComponent<StateController>().gc.EndGlobalEvents();
            scorp.GetComponent<StateController>().enabled = false;
            disableDeathAnim = true;

        }
    }


}
