using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RockStructureBehavior : MonoBehaviour
{

    public bool hasBeenHit;

    bool doOnce = false;
    bool doOnce2 = false;

    public RockStructureBehavior otherRock1;
    public RockStructureBehavior otherRock2;
    public RockStructureBehavior otherRock3;


    public AudioSource shotSource;
    public AudioSource audioSource;
    public AudioClip floorHit;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }

    // Update is called once per frame
    void Update()
    {
        

        if(hasBeenHit)
        {
            otherRock1.hasBeenHit = true;
            otherRock2.hasBeenHit = true;
            otherRock3.hasBeenHit = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            if(!doOnce2)
            {
                if(shotSource != null)
                shotSource.Play();
                doOnce2 = true;
            }
        }

    }



    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Terrain" && hasBeenHit)
        {
            audioSource.clip = floorHit;
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
    }



}
