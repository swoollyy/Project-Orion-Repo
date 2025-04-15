using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderEnable : MonoBehaviour
{
    public Inventory inv;
    bool stillColliding;

    public bool disableCollider = false;

    float debugTimer;

    public LayerMask excludeAllLayers;
    public LayerMask excludeNoLayers;
    public LayerMask excludeLayersAllButPlayer;

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
