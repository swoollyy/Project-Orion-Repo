using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftHitDetection : MonoBehaviour
{

    public Transform forklift;

    public ForkliftFunction forkLiftFunction;

    public Level2Text text;

    public GameObject enemy;

    public bool monsterPinned;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Trap" && enemy.GetComponent<MonsterNav>().lockToOilpool)
        {
            enemy.transform.parent = forklift;
            enemy.GetComponent<MonsterNav>().postBiteSFX.Play();
        }

        if (col.gameObject.name == "ShippingCrateForklift" && enemy.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger && text.textCounter == 15)
        {
            forkLiftFunction.moveForklift = false;
            monsterPinned = true;
            enemy.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = false;
        }
    }

}
