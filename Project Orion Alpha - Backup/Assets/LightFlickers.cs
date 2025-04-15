using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlickers : MonoBehaviour
{

    float initialInt;

    bool increase;
    bool decrease;

    float timeElapsed;
    float timeElapsed2;
    float duration;


    // Start is called before the first frame update
    void Start()
    {
        initialInt = GetComponent<Light>().intensity;
        IncreaseIntensity();
        duration = 1.2f;
    }

    // Update is called once per frame
    void Update()
    {
        if(increase)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed / duration;

            GetComponent<Light>().intensity = Mathf.Lerp(initialInt, 450f, t);

            if(t >= 1f)
            {
                increase = false;
            }

        }
        if (!increase)
        {
            decrease = true;
            timeElapsed = 0;
        }



        if (decrease)
        {
            timeElapsed2 += Time.deltaTime;
            float t = timeElapsed2 / duration;

            GetComponent<Light>().intensity = Mathf.Lerp(450f, initialInt, t);

            if (t >= 1f)
            {
                decrease = false;
            }
        }
        if (!decrease)
        {
            increase = true;
            timeElapsed2 = 0;
        }

    }


    void IncreaseIntensity()
    {
        increase = true;
    }

    void DecreaseIntensity()
    {
        decrease = true;
    }

}
