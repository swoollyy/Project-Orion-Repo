using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticles : MonoBehaviour
{

    float killTimer;

    public float maxKill;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        killTimer += Time.deltaTime;

        if (killTimer >= maxKill)
            Destroy(this.gameObject);

    }
}
