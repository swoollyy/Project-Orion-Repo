using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopRockFallBehavior : MonoBehaviour
{

    bool doOnce = false;
    public RockStructureBehavior rsb;

    bool canStillHit = true;

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
        if (col.gameObject.tag == "Player" && !rsb.hasBeenHit)
        {
            if (!doOnce && canStillHit)
            {
                Vector3 forceDir = Camera.main.transform.forward.normalized * -1f;
                col.gameObject.GetComponent<CollisionForces>().GunHitback(forceDir, 9f);
                col.gameObject.GetComponent<PlayerHealthLevel1>().TakeDamage(95f);
                doOnce = true;
            }

        }

        if(col.gameObject.tag == "Terrain")
        {
            canStillHit = false;
        }

    }
}
