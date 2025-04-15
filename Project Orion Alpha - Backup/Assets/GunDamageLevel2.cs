using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.VFX;
using PrimeTween;

public class GunDamageLevel2 : MonoBehaviour
{
    public float damage;
    public float range;

    public string enemyTag = "Enemy";
    private Transform mainCamera;


    public Item item;
    public VFXFire steamFX;

    public bool isLeftClicking;
    public bool isRightClicking;
    public UnityEvent onGunShoot;
    public bool automatic = false;

    public Animator animator;
    public Animator camAnim;

    public AudioSource gunFire;
    public ParticleSystem lightningBall;
    public ParticleSystem staticSpark;
    public ParticleSystem rechargeParticles;
    public ParticleSystem gunFireSteam;
    public ParticleSystem loadUpPart;
    public GameObject fireNode;
    public GameObject realNode;

    public PlayerInWater playerInWater;


    public GameObject player;
    public GameObject camHolder;

    bool shootRay;
    bool lowerDrag;

    public LayerMask layerMask;

    public AudioSource rechargeSFX;
    public AudioSource overheatSFX;

    public GameObject impactSFXInstantiate;
    public GameObject impactPartInstantiate;

    public GameControllerLevel2 gc2;
    public GameControllerShip gcShip;

    float resetDragTimer;

    bool overheatTimerBool;
    float overheatTimer;

    bool rechargePart = true;
    bool hasShot;
    bool resetAnim;
    float resetAnimTimer;

    Vector3 hitPoint;
    GameObject hitObj;
    bool sphereCastBubble;

    // Start is called before the first frame update
    void Start()
    {
        mainCamera = Camera.main.transform;
        if(gc2 != null)
        gc2.gunCurrentCooldown = 0;
        else if(gcShip != null)
            gcShip.gunCurrentCooldown = 0;
    }


    private void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * 10;
        Debug.DrawRay(transform.position, -forward, Color.yellow);

        if (automatic)
        {
            if (Input.GetMouseButton(0))
            {
                isLeftClicking = true;
                if(gc2 != null)
                if (gc2.gunCurrentCooldown <= 0f)
                {
                    animator.SetTrigger("Fire");
                    onGunShoot?.Invoke();
                    gc2.gunCurrentCooldown = gc2.gunCooldown;
                }
                if (gcShip != null)
                    if (gcShip.gunCurrentCooldown <= 0f)
                    {
                        animator.SetTrigger("Fire");
                        onGunShoot?.Invoke();
                        gcShip.gunCurrentCooldown = gcShip.gunCooldown;
                    }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                isLeftClicking = true;
                if(gc2 != null)
                if (gc2.gunCurrentCooldown <= 0f)
                {
                    animator.SetTrigger("Fire");
                    onGunShoot?.Invoke();
                    gc2.gunCurrentCooldown = gc2.gunCooldown;
                    steamFX.FireMeow();
                }
                if (gcShip != null)
                    if (gcShip.gunCurrentCooldown <= 0f)
                    {
                        animator.SetTrigger("Fire");
                        onGunShoot?.Invoke();
                        gcShip.gunCurrentCooldown = gcShip.gunCooldown;
                    }
            }
        }
        if (Input.GetMouseButtonUp(0))
            isLeftClicking = false;

        if (Input.GetMouseButtonDown(1))
        {
            isRightClicking = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            isRightClicking = false;
        }

        if (isLeftClicking && isRightClicking)
        {
            animator.SetBool("hasADSFired", true);
            camAnim.SetBool("IsADShooting", true);
            resetAnim = true;
        }
        if (!isLeftClicking && !isRightClicking)
            animator.SetBool("hasADSFired", false);

        if (resetAnim)
            resetAnimTimer += Time.deltaTime;

        if (resetAnimTimer >= .3f)
        {
            camAnim.SetBool("IsADShooting", false);
            resetAnim = true;
            resetAnimTimer = 0;
        }

        if(gc2 != null)
        {
            if (gc2.gunCurrentCooldown <= 4f && hasShot)
            {

                if (!rechargeSFX.isPlaying)
                {
                    rechargeSFX.Play();
                    loadUpPart.Play();
                    hasShot = false;
                }


            }

            if (gc2.gunCurrentCooldown <= .5f && !rechargePart)
            {
                rechargeParticles.Play();
                rechargePart = true;
            }
        }

        if (gcShip != null)
        {
            if (gcShip.gunCurrentCooldown <= 4f && hasShot)
            {

                if (!rechargeSFX.isPlaying)
                {
                    rechargeSFX.Play();
                    loadUpPart.Play();
                    hasShot = false;
                }


            }

            if (gcShip.gunCurrentCooldown <= .5f && !rechargePart)
            {
                rechargeParticles.Play();
                rechargePart = true;
            }
        }





        if (overheatTimerBool)
            overheatTimer += Time.deltaTime;

        if (overheatTimer >= .5f)
            if (!overheatSFX.isPlaying)
            {
                overheatSFX.Play();
                overheatTimer = 0f;
                overheatTimerBool = false;
            }




        if (shootRay)
        {
            RaycastHit hit;
            Ray ray;


            if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, Mathf.Infinity, layerMask))
            {
                GameObject instImpactSFX = Instantiate(impactSFXInstantiate, hit.point, Quaternion.identity);
                GameObject instImpactPart = Instantiate(impactPartInstantiate, hit.point, Quaternion.identity);

                hitPoint = hit.point;
                hitObj = hit.transform.gameObject;

                Destroy(instImpactSFX, 5f);
                Destroy(instImpactPart, 2.5f);

                sphereCastBubble = true;

            }


            shootRay = false;
        }





        if (sphereCastBubble)
        {
            RaycastHit[] allHit = Physics.SphereCastAll(hitPoint, 2.5f, -camHolder.transform.up, Mathf.Infinity, layerMask);

            for (int i = 0; i < allHit.Length; i++)
            {

                if (allHit[i].transform.gameObject.tag == "Lizard")
                {
                    allHit[i].transform.gameObject.GetComponent<LizardStateController>().hasBeenShot = true;
                    allHit[i].transform.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    allHit[i].transform.gameObject.GetComponent<Rigidbody>().AddForce(hitPoint * .1f, ForceMode.Impulse);
                }
                else if (allHit[i].transform.gameObject.tag == "RockStructure")
                {
                    allHit[i].transform.GetComponent<RockStructureBehavior>().hasBeenHit = true;
                    allHit[i].transform.gameObject.GetComponent<Rigidbody>().AddForce(hitPoint * .4f, ForceMode.Impulse);
                }
                else if (allHit[i].transform.gameObject.tag == "RockArchway")
                {
                    allHit[i].transform.GetComponent<DropRock>().PlayAnimation();
                }




            }


            sphereCastBubble = false;
        }



    }

    public void Shoot()
    {

        Instantiate(lightningBall, realNode.transform.position, mainCamera.transform.rotation);
        gunFire.Play();
        staticSpark.Play();
        gunFireSteam.Play();

        Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.3f, .3f, .3f), duration: .3f, frequency: 40);

        overheatTimerBool = true;
        rechargePart = false;
        hasShot = true;

        if(gc2 != null)
        gc2.gunCurrentCooldown = gc2.gunCooldown;
        else if (gcShip != null)
            gcShip.gunCurrentCooldown = gcShip.gunCooldown;

        shootRay = true;

        Vector3 forceDir = mainCamera.transform.forward.normalized * -1f;

        if(playerInWater != null)
        {
            if (playerInWater.IsPlayerInWater())
            {
                player.GetComponent<CollisionForces2>().GunHitback(forceDir, 28f);
            }
            else
            player.GetComponent<CollisionForces2>().GunHitback(forceDir, 8f);
        }
        else
            player.GetComponent<CollisionForces2>().GunHitback(forceDir, 8f);



    }
}
