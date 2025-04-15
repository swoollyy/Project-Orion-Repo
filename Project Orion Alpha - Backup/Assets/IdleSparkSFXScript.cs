using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleSparkSFXScript : MonoBehaviour
{

    public List<AudioClip> sparkClips = new List<AudioClip>();

    float randomSparkTriggerMin = .08f;
    float randomSparkTriggerMax  = 1.2f;

    float sparkTriggerMax;

    float timer;

    public GameObject gun;

    int index;
    AudioClip chosenClip;

    // Start is called before the first frame update
    void Start()
    {
        sparkTriggerMax = Random.Range(randomSparkTriggerMin, randomSparkTriggerMax);
    }

    // Update is called once per frame
    void Update()
    {
        if(gun.activeSelf)
        timer += Time.deltaTime;

        if(timer >= sparkTriggerMax)
        {
            index = Random.Range(0, sparkClips.Count);
            chosenClip = sparkClips[index];
            GetComponent<AudioSource>().clip = chosenClip;
            GetComponent<AudioSource>().Play();
            sparkTriggerMax = Random.Range(randomSparkTriggerMin, randomSparkTriggerMax);
            sparkTriggerMax = Random.Range(randomSparkTriggerMin, randomSparkTriggerMax);
            sparkTriggerMax = Random.Range(randomSparkTriggerMin, randomSparkTriggerMax);
            timer = 0;
        }


    }
}
