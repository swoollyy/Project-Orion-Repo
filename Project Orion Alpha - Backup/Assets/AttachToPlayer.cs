using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToPlayer : MonoBehaviour
{

    public Transform reference;

    float smoothTime = 3f;
    float timeElapsed = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position == reference.position)
        {
            timeElapsed = 0;
        }
        else
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / smoothTime;
            transform.position = Vector3.Lerp(transform.position, reference.position, t);
        }

    }
}
