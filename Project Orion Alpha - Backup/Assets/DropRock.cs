using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropRock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAnimation()
    {
        GetComponent<Animator>().enabled = true;
        if(!GetComponent<AudioSource>().isPlaying)
        {
            GetComponent<AudioSource>().Play();
        }
    }

}
