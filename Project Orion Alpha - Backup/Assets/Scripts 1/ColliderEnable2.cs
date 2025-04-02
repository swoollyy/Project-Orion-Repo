using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnable2 : MonoBehaviour
{
    public Inventory2 inv;
    bool stillColliding;

    public bool disableCollider = false;

    float debugTimer;

    public LayerMask excludeAllLayers;
    public LayerMask excludeNoLayers;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(disableCollider)
        {
            GetComponent<Collider>().excludeLayers = excludeAllLayers;
        }
        else if(!disableCollider && inv.isThrowingWeapon)
        {
            GetComponent<Collider>().excludeLayers = excludeNoLayers;
        }


    }


}
