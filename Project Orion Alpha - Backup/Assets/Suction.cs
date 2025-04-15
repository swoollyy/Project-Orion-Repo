using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class Suction : MonoBehaviour
{

    public Transform gravityCenter;
    public Transform player;

    public PhysicMaterial slideyMat;

    public GameObject dirtShakeParticles;
    public GameObject camHolder;


    public StateController scorpController;
    public GameObject scorpion;

    GameObject spawnedPart;

    public Coroutine gravityCoroutine;

    public bool beginSuckage;

    public bool doOnce = false;

    public bool beginSuckageTimer;
    public float timer;

    float disableMovementTimer;

    // Start is called before the first frame update
    void Start()
    {
        dirtShakeParticles.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (beginSuckageTimer)
        {
            timer += Time.deltaTime;
            print("suckage timer " + timer);
        }
        else timer = 0;


        if(timer > 11f && player.GetComponent<Inventory>().isHoldingLizard)
        {
            print("suckage at max WAKE SCORPION");
            if (!scorpion.activeSelf)
                scorpion.SetActive(true);
            if(!doOnce)
            {
                scorpController.agent.enabled = false;
                doOnce = true;
            }
            scorpController.gc.deadLizard = true;
            scorpController.gc.lizardEruption = true;
            scorpController.eruptNearPlayer = false;
            scorpController.isScaringPlayer = false;
            scorpController.ForceHuntPlayer();
        }


    }

    public IEnumerator ApplyGravity()
    {
        beginSuckage = true;
        scorpController.succ = this;
        while (beginSuckage) // Infinite loop, runs continuously
        {
            Vector3 direction = (gravityCenter.position - player.position).normalized;
            float distance = Vector3.Distance(gravityCenter.position, player.position);
            float forceStrength = 21 / Mathf.Clamp(distance * .5f + 1, 1, 100);

            Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.12f, .12f, .12f), duration: .4f, frequency: 20);

            spawnedPart = Instantiate(dirtShakeParticles, dirtShakeParticles.transform.position, dirtShakeParticles.transform.rotation);
            spawnedPart.SetActive(true);

            transform.parent.GetComponent<AudioSource>().pitch = Random.Range(.9f, 1.1f);
            transform.parent.GetComponent<AudioSource>().Play();

            player.gameObject.GetComponent<Rigidbody>().AddForce(direction * forceStrength, ForceMode.Impulse);
            player.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * forceStrength, ForceMode.Impulse);

            yield return new WaitForSeconds(.7f);


        }
        gravityCoroutine = null;
    }



}
