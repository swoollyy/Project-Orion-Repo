using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableLineRenderer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TrailRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void EnableRenderer()
    {
        GetComponent<TrailRenderer>().enabled = true;
    }

    void DisableRenderer()
    {
        GetComponent<TrailRenderer>().enabled = false;
    }


}
