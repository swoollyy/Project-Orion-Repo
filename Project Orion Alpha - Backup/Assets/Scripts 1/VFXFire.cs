using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXFire : MonoBehaviour
{
    public ParticleSystem VFXprefab;
    public GameObject fireNode;
    public float lifetime = 5f;

    //Call this function
    public void FireMeow()
    {
        PlayParticle();
    }


    //This plays the particle (or should)
    private void PlayParticle()
    {
        ParticleSystem newBurst = Instantiate(VFXprefab, fireNode.transform.position, Camera.main.transform.rotation);
        newBurst.Play();
        Destroy(newBurst.gameObject, lifetime);
    }
}
