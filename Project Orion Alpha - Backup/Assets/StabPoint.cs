using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class StabPoint : MonoBehaviour
{
    public GameObject stabpoint;
    public LayerMask enemyLayerMask;
    public LayerMask objectLayerMask;

    public GameObject bloodParticle;

    Camera cam;
    public float test;
    public float size;
    bool startDetection;
    bool isSlashing;

    public GameObject stabSpark;
    public GameObject slashSpark;

    public GameController gc;
    public GameControllerLevel2 gc2;

    public GameObject camHolder;

    public Animator knifeAnim;

    public AudioSource knifeSFX;
    public AudioClip slashAudio;
    public AudioClip stabAudio;

    Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (startDetection)
        {
            if (Physics.SphereCast(cam.transform.position, size, cam.transform.forward, out hit, test, enemyLayerMask))
            {
                pos = hit.point;
                if (hit.transform.gameObject.tag == "Lizard")
                {
                    GameObject hitLiz = hit.transform.gameObject;

                    GameObject bloodPart = Instantiate(bloodParticle, hit.point, Quaternion.Euler(-transform.forward));

                    Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.3f, .3f, .3f), duration: .2f, frequency: 20);

                    hitLiz.GetComponent<LizardHealth>().TakeDamage(50f);
                }
                else if (hit.transform.gameObject.tag == "Enemy")
                {
                    GameObject hitScorp = hit.transform.gameObject;
                    if (hit.transform.gameObject.name == "Brown Scorpion" && !isSlashing)
                    {
                        if(gc != null)
                        if (hitScorp.GetComponent<StateController>().gc.deadScorpion)
                        {

                            GameObject bloodPart = Instantiate(bloodParticle, hit.point, Quaternion.Euler(-transform.forward));
                            GetComponent<Animator>().speed = 0;
                            knifeAnim.speed = 0;

                            hitScorp.GetComponent<StateController>().gc.startSuccTimer = true;
                            Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: 3f, frequency: 10);

                        }
                    }
                    else
                    {
                        if(gc2 != null)
                        if (gc2.deadMonster && !isSlashing)
                        {

                            GameObject bloodPart = Instantiate(bloodParticle, hit.point, Quaternion.Euler(-transform.forward));
                            GetComponent<Animator>().speed = 0;
                            knifeAnim.speed = 0;

                            gc2.startSuccTimer = true;
                            Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: 3f, frequency: 10);

                        }
                    }
                }
                startDetection = false;
            }

            if(!isSlashing)
            if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1.5f, objectLayerMask))
            {
                if (hit.transform.gameObject.tag == "Room" || hit.transform.gameObject.tag == "Terrain")
                {
                    GameObject spark = Instantiate(stabSpark, hit.point, Quaternion.identity);

                        Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: .2f, frequency: 10);

                        Destroy(spark, 2f);
                }
                else if(hit.transform.gameObject.tag == "Barrel")
                    {
                        GameObject spark = Instantiate(stabSpark, hit.point, Quaternion.identity);

                        Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: .2f, frequency: 10);

                        Destroy(spark, 2f);
                    }
                startDetection = false;
            }

            if(isSlashing)
            {
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 1.5f, objectLayerMask))
                {
                    if (hit.transform.gameObject.tag == "Room" || hit.transform.gameObject.tag == "Terrain")
                    {
                        GameObject spark = Instantiate(slashSpark, hit.point, cam.transform.rotation);

                        Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: .2f, frequency: 10);

                        Destroy(spark, 2f);
                    }
                    else if (hit.transform.gameObject.tag == "Barrel")
                    {
                        GameObject spark = Instantiate(stabSpark, hit.point, Quaternion.identity);



                        if(gc2 != null)
                        {
                            gc2.barrel = hit.transform.gameObject;
                            gc2.hitBarrel = true;
                            gc2.barrelHitPoint = hit.point;
                        }


                        Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.1f, .1f, .1f), duration: .2f, frequency: 10);



                        Destroy(spark, 2f);
                    }
                    startDetection = false;
                }
            }


        }

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pos, 1f);
    }

    public void DetectStab()
    {
        startDetection = true;
        knifeSFX.volume = Random.Range(.58f, .65f);
        knifeSFX.pitch = Random.Range(.9f, 1.1f);
        knifeSFX.clip = stabAudio;
        if (!knifeSFX.isPlaying)
            knifeSFX.Play();
    }
    public void DetectSlash()
    {
        startDetection = true;
        isSlashing = true;
        knifeSFX.volume = Random.Range(.58f, .65f);
        knifeSFX.pitch = Random.Range(.9f, 1.1f);
        knifeSFX.clip = slashAudio;
        if (!knifeSFX.isPlaying)
            knifeSFX.Play();
    }


    public void StopDetection()
    {
        startDetection = false;
        isSlashing = false;
    }

}
