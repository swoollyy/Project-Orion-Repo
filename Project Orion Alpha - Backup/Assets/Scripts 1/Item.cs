using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public bool   isCollected;
    public bool hasCollectedOnce;
    public bool canBeCollected;





    // Start is called before the first frame update
    void Start()
    {
        canBeCollected = true;
    }

    // Update is called once per frame
    void Update()
    {


    }



}
