using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;


public class BurrowAttackLizardState : IState
{
    float debugTimer;
    float emergeTimer;
    float explodeTimer;
    bool emergeTimerStart;
    bool explodeTimerStart;
    bool isCharging;
    public bool climbUp;
    bool doOnce = false;
    bool doOnce2 = false;
    bool doOnce3 = false;

    public bool eatLizard;
    public bool getLizard;

    bool keepDmgAreaOn;

    RaycastHit hit;

    int pickPosition;

    float RNGRangeX;
    float RNGRangeZ;

    float posOrNeg;

    bool hasBurAtk;
    bool jump;
    Vector3 lookDirection;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetBool("IsBurrowAttacking", true);
        sc.scorpAudio.PlayDiggingSound();

        sc.agent.enabled = true;

        debugTimer = 0;
        explodeTimer = 0;
        explodeTimerStart = false;
        hasBurAtk = false;
        sc.agent.speed = 35f;
        sc.agent.acceleration = 75f;
        sc.agent.angularSpeed = 5000f;

    }

    public void UpdateState(StateController sc)
    {
        debugTimer += Time.deltaTime;
        if (sc.agent.enabled == true && sc.gc.trappedLizard != null && !sc.lizardInPosition)
            sc.agent.SetDestination(sc.gc.trappedLizard.transform.position);


        if(eatLizard && emergeTimerStart)
            emergeTimer += Time.deltaTime;

        if(emergeTimer > 2f)
        {
            sc.gc.trappedLizard = sc.trappedLizard;
            getLizard = true;
            sc.enabled = false;
            emergeTimerStart = false;
            emergeTimer = 0;
        }



        if (sc.agent.enabled == true)
        if(sc.agent.remainingDistance <= 2f && debugTimer > .5f && !hasBurAtk)
        {
            if(!isCharging)
                climbUp = true;

                Debug.Log("line 47");

            }
        if (climbUp)
        {
            if (sc.succ != null)
                if (sc.succ.beginSuckage)
                    sc.eruptNearPlayer = false;

            Debug.Log("line 52");
            sc.agent.enabled = false;
            sc.animator.SetBool("IsBurrowAttacking", false);
            if (sc.gc.lizardEruption || sc.isburrowAtkLizard)
            {
                if (sc.succ == null || !sc.succ.beginSuckage)
                {
                    if (sc.lizardInPosition)
                    {
                        if (!doOnce2)
                        {
                            pickPosition = Random.Range(1, 3);
                            doOnce2 = true;
                        }

                        if (pickPosition == 1)
                        {
                            if (sc.gc.trappedLizard != null)
                                sc.transform.position = new Vector3(sc.location1.transform.position.x, sc.transform.position.y + .5f, sc.location1.transform.position.z);
                        }
                        else if (pickPosition == 2)
                        {
                            if (sc.gc.trappedLizard != null)
                                sc.transform.position = new Vector3(sc.location2.transform.position.x, sc.transform.position.y + .5f, sc.location2.transform.position.z);
                        }

                        if (sc.gc.trappedLizard != null)
                            if (sc.transform.position.y >= sc.gc.trappedLizard.transform.position.y - 2f)
                                sc.animator.SetBool("IsUnburrowing", true);

                        RaycastHit hitter;
                        if (Physics.Raycast(sc.transform.position, -sc.upReference.transform.up, out hitter, Mathf.Infinity, sc.groundMask))
                        {
                            sc.agent.enabled = true;
                            sc.nms.enabled = true;
                            sc.scorpAudio.StopDiggingSound();
                            hasBurAtk = true;
                            if (!doOnce3)
                            {
                                sc.agent.ResetPath();
                                sc.agent.speed = 6f;
                                sc.agent.acceleration = 12f;
                                sc.agent.angularSpeed = 5000f;
                                emergeTimerStart = true;
                                eatLizard = true;
                                doOnce3 = true;
                            }
                            pickPosition = 0;
                            sc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                            sc.animator.SetBool("IsUnburrowing", false);

                        }
                    }
                    else
                    {
                        if(sc.succ != null)
                        {
                            sc.succ.timer = 0;
                            sc.succ = null;
                        }
                        if (sc.gc.trappedLizard != null)
                            sc.transform.position = new Vector3(sc.gc.trappedLizard.transform.position.x, sc.transform.position.y + .5f, sc.gc.trappedLizard.transform.position.z);
                        if (sc.gc.trappedLizard != null)
                            if (sc.transform.position.y >= sc.gc.trappedLizard.transform.position.y - 2f)
                                isCharging = true;
                    }

            }
                else if (sc.poisonedLizard && sc.gc.playerInv.isHoldingLizard)
                {
                    if (!doOnce2)
                    {
                        RNGRangeX = Random.Range(4f, 8f);
                        RNGRangeZ = Random.Range(4f, 8f);

                        posOrNeg = Random.Range(1, 3);

                        if (posOrNeg == 1)
                            return;
                        else if (posOrNeg == 2)
                        {
                            RNGRangeX *= -1;
                            RNGRangeZ *= -1;
                        }
                        doOnce2 = true;
                    }
                    if (sc.gc.trappedLizard != null)
                        sc.transform.position = new Vector3(sc.gc.trappedLizard.transform.position.x + RNGRangeX, sc.transform.position.y + .5f, sc.gc.trappedLizard.transform.position.z + RNGRangeZ);
                    if (sc.gc.trappedLizard != null)
                        if (sc.transform.position.y >= sc.gc.trappedLizard.transform.position.y - 2f)
                            sc.animator.SetBool("IsUnburrowing", true);

                    RaycastHit hitter;
                    if (Physics.Raycast(sc.transform.position, -sc.upReference.transform.up, out hitter, Mathf.Infinity, sc.groundMask))
                    {
                        sc.agent.enabled = true;
                        hasBurAtk = true;
                        sc.nms.enabled = true;
                        sc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
                        sc.animator.SetBool("IsUnburrowing", false);
                    }



                    Debug.Log("AGANATOR");

                }
            }


        }
        if (isCharging)
        {
            Debug.Log("line 64");
            climbUp = false;
            if (!sc.hasDone)
            {
                sc.lizardRumbleAudio.Play();
                if (sc.gc.trappedLizard != null)
                    sc.burrowLockPosition = sc.gc.trappedLizard.transform.position;
                Tween.ShakeLocalPosition(sc.camHolder.transform, strength: new Vector3(.4f, .4f, .4f), duration: .65f, frequency: 40);
                sc.scorpAudio.StopDiggingSound();
                Vector3 rayCast = new Vector3(sc.burrowLockPosition.x, sc.burrowLockPosition.y + 30, sc.burrowLockPosition.z);

                if (Physics.Raycast(rayCast, -sc.transform.up, out hit, Mathf.Infinity, sc.groundMask))
                {
                    Quaternion rotation = Quaternion.LookRotation(hit.normal);
                    Vector3 hitPoint = new Vector3(hit.point.x, hit.point.y + 2f, hit.point.z);
                    sc.SpawnGroundParticles(hitPoint, rotation);
                }
                sc.hasDone = true;
            }
            sc.transform.position = new Vector3(sc.burrowLockPosition.x, sc.burrowLockPosition.y - 3f, sc.burrowLockPosition.z);
            explodeTimerStart = true;
            jump = true;
            sc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;


        }
        if (explodeTimerStart)
            explodeTimer += Time.deltaTime;

        if(isCharging && explodeTimer < .55f)
        {
            if(!sc.lizardRumbleAudio.isPlaying)
            sc.lizardRumbleAudio.Play();

                sc.scorpAudio.StopBurrowSound();

            if (!sc.watchCases && sc.player.GetComponent<PlayerMovement>().hasEscaped)
            {
                Debug.Log("BURROW ATTACK");
                sc.eruptNearPlayer = true;
                sc.hasBurAtk = false;
                sc.ChangeState(sc.burrowAttackState);
            }
            if(!sc.watchCases && sc.lizardInPosition)
            {
                climbUp = true;
                Debug.Log("SWITCHING POSITIONS");
                isCharging = false;
            }
        }

        if (explodeTimer >= .55f)
        {
            Debug.Log("line 83");
            isCharging = false;
            sc.animator.SetBool("IsUnburrowing", false);
            sc.animator.SetBool("IsBurrowAttacking", false);
            sc.damageArea.transform.position = sc.transform.position;

            sc.lizardRumbleAudio.Stop();

            if (jump)
            {
                sc.damageArea.SetActive(true);
                Tween.ShakeLocalPosition(sc.camHolder.transform, strength: new Vector3(.6f, .6f, .6f), duration: .7f, frequency: 80);
                sc.scorpAudio.PlayStartBurrowSound();
                sc.GetComponent<Collider>().enabled = false;
                sc.GetComponent<Rigidbody>().AddForce(sc.upReference.transform.up * 21f, ForceMode.VelocityChange);
                jump = false;
            }

            if (!sc.agent.enabled)
            lookDirection = new Vector3(sc.GetComponent<Rigidbody>().velocity.y * -9f, 0f, 0f);
            hasBurAtk = true;
            sc.transform.rotation = Quaternion.Euler(lookDirection).normalized;
        }

        if(explodeTimer > .75f && explodeTimer < .85f)
        {
            sc.damageArea.SetActive(false);
        }

        if (Physics.Raycast(sc.transform.position, -sc.upReference.transform.up, out hit, 3f, sc.groundMask) && explodeTimer >= 2f)
        {
            sc.transform.eulerAngles = new Vector3(0f, 180f, 0f);
            sc.transform.GetChild(0).GetComponent<RotateToGround>().RotateNow();
            sc.agent.enabled = true;
            sc.nms.enabled = true;
            sc.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePosition;
            sc.animator.SetBool("IsBurrowAttacking", false);


            if (!sc.firstJump)
            {
                Debug.Log("first function");
                sc.animator.SetBool("isHissing", true);
                sc.isScaringPlayer = true;
                sc.firstJump = true;
            }
            else
                sc.animator.SetBool("isHissing", false);

            sc.hasDone = false;
            sc.inBurAtkRange = false;
            sc.damageArea.SetActive(false);
            sc.GetComponent<Collider>().enabled = true;
            if (sc.gc.doOnce2 && (Mathf.Abs(sc.posX) > 7f && Mathf.Abs(sc.posZ) > 7f))
            {
                sc.ChangeState(sc.burrowState);
            }
            else if(sc.gc.doOnce2)
            {
                sc.animator.SetTrigger("IsDetecting");
                Debug.Log("detecting?");
                sc.transform.LookAt(sc.player.transform);
                sc.ChangeState(sc.chaseState);
            }

        }



        if (Physics.Raycast(sc.transform.position, -sc.upReference.transform.up, out hit, 9f, sc.groundMask) && explodeTimer >= 1.5f)
        {
            if(!doOnce)
            {
                Debug.Log("detecting wrongness");
                sc.gc.zoomToFace = true;
                doOnce = true;
            }
        }




    }


    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        explodeTimerStart = false;
        doOnce2 = false;
        doOnce2 = false;
        hasBurAtk = false;
        debugTimer = 0;
        explodeTimer = 0;
        sc.player.GetComponent<PlayerMovement>().hasEscaped = false;
        sc.animator.SetBool("IsUnburrowing", false);
        sc.scorpAudio.StopDiggingSound();
        sc.scorpAudio.StopBurrowSound();

    }



}
