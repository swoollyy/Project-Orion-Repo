using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debug : MonoBehaviour
{
    public GameObject currentHitObject;
    public GameObject lockedOnObject;

    public float sphereRadius;
    public float maxDistance;
    public LayerMask layerMask;
    bool beginChase = false;
    public bool chase = false;
    public bool preyChase = false;

    private float currentHitDistance;
    private Vector3 origin;
    private Vector3 direction;

    float nullTimer;
    bool nullTimerStart = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //player gameObject detection
        origin = transform.position;
        direction = transform.position;
        RaycastHit hit;
        //enables creature chase if enemy sees player

    }

    //debug drawings
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, maxDistance);



        Debug.DrawLine(this.transform.position, this.transform.position + transform.forward * 15, Color.blue, .5f);
    }
}
