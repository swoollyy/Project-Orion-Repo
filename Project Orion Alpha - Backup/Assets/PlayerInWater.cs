using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerInWater : MonoBehaviour
{

    public Transform topMap;
    public Transform player;

    public LayerMask waterMask;

    float timer;
    float drownTimer;

    public Volume volume;
    private ColorAdjustments colorAdjustments;

    public AudioMixerSnapshot underwaterSnapshot;
    public AudioMixerSnapshot levelSnapshot;

    // Start is called before the first frame update
    void Start()
    {
        drownTimer = 3f;

        volume.profile.TryGet(out colorAdjustments);

    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInWater())
        {
            Physics.gravity = new Vector3(0f, -.79f, 0f);
            print("ooga");

            timer += Time.deltaTime;

            if(timer > 12f)
            {
                drownTimer += Time.deltaTime;

                if(drownTimer >= 1f)
                {
                    player.GetComponent<PlayerHealth>().TakeDamage(7.5f);
                    drownTimer = 0f;
                }
            }

            colorAdjustments.colorFilter.overrideState = true;
            underwaterSnapshot.TransitionTo(.5f);

        }
        else
        {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
            timer = 0f;
            print("booga");
            colorAdjustments.colorFilter.overrideState = false;
            levelSnapshot.TransitionTo(.5f);
        }
    }

    public bool IsPlayerInWater()
    {
        RaycastHit hit;
        if (Physics.Raycast(topMap.position, -transform.up, out hit, Mathf.Infinity, waterMask))
        {
            if (player.position.y < hit.point.y)
                return true;
        }
        return false;
    }

}
