using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BrightSunInteraction : MonoBehaviour
{

    public BrightSunTrigger brightSunTrigger;

    public Animator playerAnim;

    public VolumeProfile volumeProfile;
    Bloom bloom;

    public float bloomIntensity;
    public float defBloomInt = 1.13f;

    public GameObject sunBlindArm;

    public Inventory playerInv;
    public CollisionForces colForces;

    bool hasFinished;

    // Start is called before the first frame update
    void Start()
    {
        defBloomInt = 1.13f;

        if (!volumeProfile.TryGet(out bloom)) throw new System.NullReferenceException(nameof(bloom));
        bloomIntensity = defBloomInt;
        bloom.intensity.value = defBloomInt;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(brightSunTrigger.enableBrightSunAnimation && !hasFinished)
        {
            BeginBrightSun();
        }
        bloom.intensity.value = bloomIntensity;

    }


    public void BeginBrightSun()
    {
        hasFinished = true;
        sunBlindArm.SetActive(true);
        playerAnim.enabled = true;
        playerAnim.SetBool("SunGlare", true);
        colForces.DisableMovement();


        if (playerInv.isHoldingKnife)
            playerInv.PutAwayKnife();
        else if (playerInv.isHoldingGun)
            playerInv.PutAwayGun();
        else playerInv.PutAwayItem(0);

    }

    public void DisableBool()
    {
        playerAnim.SetBool("SunGlare", false);
    }

    public void StopBrightSun()
    {
        sunBlindArm.SetActive(false);
        brightSunTrigger.enableBrightSunAnimation = false;

    }

}
