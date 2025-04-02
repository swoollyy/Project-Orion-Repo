using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour
{
    bool isColliding;

    Vector3 contactPoint;

    public GameObject player;

    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        Ray ray;
        RaycastHit hit;

        if (Physics.Raycast(player.transform.position, Camera.main.transform.forward, out hit, 1.15f, layerMask))
        {
            transform.position = new Vector3(hit.point.x, hit.point.y + .3f, hit.point.z);
            print("hit" + hit.transform.gameObject);
        }
        else transform.localPosition = new Vector3(-.11f, -.25f, 1.15f);
    }
}
