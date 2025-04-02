using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollisionRock : MonoBehaviour
{

    public Animator animator;

    public GameObject topRock;

    public AudioSource rockAudio;

    bool doOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        animator.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player" && col.gameObject.GetComponent<Inventory>().isHoldingLizard)
        {
            if(!doOnce)
            {
                if (!rockAudio.isPlaying)
                    rockAudio.Play();
                topRock.GetComponent<MeshCollider>().isTrigger = true;
                BeginAnimation();
                doOnce = true;
            }

        }
    }


    void BeginAnimation()
    {
        animator.enabled = true;
    }

}
