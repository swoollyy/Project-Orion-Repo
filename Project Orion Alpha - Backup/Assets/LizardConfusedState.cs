using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardConfusedState : LizardIState
{

    float timer;
    float movementTimer;

    float maxTimer;
    float maxMovementTimer;
    int negOrPos;
    float horizIntensity;

    int doOnce;


    public float moveDuration;
    public float elapsedTime = 0f;

    Vector3 playerPos;

    bool decreaseVal;
    bool turnToPlayer;
    bool beginMovement;
    bool exitMovement;
    bool playConfusedParticle;
    bool hasDone;
    bool hasSetDest;

    public void OnEnter(LizardStateController sc)
    {
        moveDuration = 3f;
        sc.currentStatus = LizardStateController.StatusToPlayer.Cautious;
        sc.agent.speed = 2f;
        sc.agent.acceleration = 25f;
        sc.agent.stoppingDistance = 2f;
        sc.animator.SetBool("isIdle", true);
        sc.confusedParticle.Play();
        sc.agent.ResetPath();
        turnToPlayer = true;

    }

    public void UpdateState(LizardStateController sc)
    {
        NavMeshHit hit;

        if (sc.playerInv.currentHeldItem == null && !sc.isTamed)
            {
            sc.fleeFromPlayer = true;
            sc.ChangeState(sc.scurryState);
            }

        if (sc.isTamed)
        {
            sc.ChangeState(sc.chaseState);
        }


        if (turnToPlayer)
        {
            Debug.Log("Turningggg");
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            sc.transform.rotation = Quaternion.Slerp(sc.transform.rotation,
        Quaternion.LookRotation((sc.transform.position - sc.player.transform.position) * -1), t);
            if (t >= 1f)
            {
                if (!exitMovement)
                {
                    Debug.Log("Turned! Moving");
                    beginMovement = true;
                    t = 0;
                    elapsedTime = 0;
                }
                turnToPlayer = false;
            }
            else if(exitMovement)
            {
                t = 0;
                elapsedTime = 0;
            }
        }

        if(!exitMovement)
        {
            if (beginMovement)
            {
                Debug.Log("Turned!!!!!");


                if (doOnce < 0)
                    doOnce++;
                if (doOnce == 0)
                    RandomizeVariables();
                timer += Time.deltaTime;
                if (timer >= maxTimer)
                    if (!sc.agent.pathPending)
                        if (NavMesh.SamplePosition(sc.player.transform.position, out hit, 6.0f, NavMesh.AllAreas))
                        {
                            movementTimer += Time.deltaTime;
                            if (!hasSetDest)
                            {
                                sc.agent.SetDestination(hit.position + ((sc.transform.right * negOrPos) * horizIntensity));
                                hasSetDest = true;
                            }
                            sc.animator.SetBool("isWalking", true);
                            sc.animator.SetBool("isIdle", false);

                            if (movementTimer >= maxMovementTimer)
                            {
                                sc.agent.ResetPath();
                                beginMovement = false;
                            }

                            if (!sc.agent.pathPending)
                                if (sc.agent.remainingDistance <= sc.agent.stoppingDistance)
                                {
                                    sc.animator.SetBool("isIdle", true);
                                    sc.animator.SetBool("isWalking", false);
                                    sc.agent.ResetPath();
                                }
                        }
            }
            else if(!beginMovement && hasSetDest)
            {
                doOnce = -1;
                timer = 0;
                movementTimer = 0;
                beginMovement = true;
                hasSetDest = false;
            }
        }



        if ((Mathf.Abs(sc.posX)) < sc.agent.stoppingDistance && (Mathf.Abs(sc.posZ)) < sc.agent.stoppingDistance)
        {
            timer = 0;
            movementTimer = 0;
            sc.animator.SetBool("isWalking", false);
            sc.animator.SetBool("isIdle", true);
            sc.agent.ResetPath();
            exitMovement = true;
            if (!hasDone)
            {
                playConfusedParticle = true;
                hasDone = true;
            }
            turnToPlayer = true;
        }
        else
        {
            exitMovement = false;
            hasDone = false;
        }


        Debug.Log("begin movement" + beginMovement);



        if(playConfusedParticle)
        {
            sc.confusedParticle.Play();
            playConfusedParticle = false;
        }






    }


    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.animator.SetBool("isIdle", false);
        sc.animator.SetBool("isWalking", false);
        // "Must've been the wind"
        sc.lockPosition = true;
        elapsedTime = 0;
        timer = 0;
        movementTimer = 0;
        doOnce = -1;
        sc.confusedParticle.Stop();
    }

    void RandomizeVariables()
    {
        doOnce++;
        maxTimer = Random.Range(1, 2f);
        maxMovementTimer = Random.Range(1f, 3f);
        negOrPos = Random.Range(-1, 3);
        horizIntensity = Random.Range(-6f, 6f);

        if (negOrPos <= 0)
        {
            negOrPos = -1;
        }
        else negOrPos = 1;
    }

}

