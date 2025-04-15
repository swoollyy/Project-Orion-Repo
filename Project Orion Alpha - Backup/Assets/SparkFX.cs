using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SparkFX : MonoBehaviour
{

    public ParticleSystem stabSparks;
    public ParticleSystem slashSparks;

    ParticleSystem instParticle;

    public GameObject location;

    float timer;
    bool beginTimer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Sparks()
    {
        instParticle = Instantiate(stabSparks, location.transform.position, location.transform.rotation);
    }

    public void SlashSparks()
    {
        instParticle = Instantiate(slashSparks, new Vector3(location.transform.position.x, location.transform.position.y + .2f, location.transform.position.z), location.transform.rotation);
    }

    public void DestroySparks()
    {
        Destroy(instParticle.gameObject);
    }
}
