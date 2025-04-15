using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DupPlayer : MonoBehaviour
{

    Quaternion quat;
    Vector3 vect;

    public GameObject player;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position;
        transform.rotation = cam.transform.rotation;
    }
}
