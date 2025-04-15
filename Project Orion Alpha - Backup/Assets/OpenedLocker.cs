using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenedLocker : MonoBehaviour
{
    public GameController gc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gc.openedLocker)
        {
            gameObject.GetComponent<MeshFilter>().mesh = gc.lockerOpen;
            gameObject.tag = "Room";
                }
    }
}
