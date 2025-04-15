using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour
{
    public AudioSource aSource;
    public AudioClip quiet;
    public AudioClip loud;
    float[] spectrum = new float[256];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("q"))
        {
            aSource.clip = quiet;
            aSource.Play();
            aSource.volume = .3f;
        }
        if(Input.GetKeyDown("l"))
        {
            aSource.clip = loud;
            aSource.Play();
            aSource.volume = 1f;
        }

    }
}
