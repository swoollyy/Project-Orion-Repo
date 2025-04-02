using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardScurryState : LizardIState
{

    float playerDistance;

    float navTimer;

    public void OnEnter(LizardStateController sc)
    {
        sc.animator.SetTrigger("isFleeing");
        sc.fleeParticle.Play();
        sc.agent.speed = sc.scurrySpeed;
        sc.agent.acceleration = sc.scurryAccel;
        sc.staminaCooldownStart = false;


        if(sc.tiredScurry)
        {
            sc.hardBreathParticle.Play();
            sc.tiredScurry = false;
        }

        sc.animator.SetBool("isWalking", true);
    }

    public void UpdateState(LizardStateController sc)
    {

        navTimer += Time.deltaTime;

        Vector3 dist = sc.transform.position + (sc.transform.position - sc.player.transform.position);
        Vector3 dist2 = new Vector3(dist.x, Mathf.Abs(dist.y) + 20f, dist.z);

        RaycastHit hit;


        if (sc.fleeFromEnemy)
        {
            sc.agent.SetDestination(sc.transform.position + (sc.transform.position - sc.enemy.transform.position));
        }
        else
        {
            sc.agent.SetDestination(sc.transform.position + (sc.transform.position - sc.player.transform.position) * .1f);
            //sc.agent.SetDestination(sc.gc.lizardSpawner.transform.position);
        }

        sc.animator.ResetTrigger("isFleeing");

        //SNEAKING && SOUND DETECTION -- ALICE
        /*if (mMent.sneak)
            if ((posX < 10 && (posZ < 10) && posX + posZ < 15) || (posZ < 10 && (posX < 10) && posX + posZ < 15))
            {
                if (!burrow.isBurrowed)
                {
                    agent.speed = 9f;
                    agent.acceleration = 19f;
                    lurk = true;
                    agent.isStopped = true;
                    hasChased = true;
                    inLurkState = true;
                }

            }
        
                     //sound detection; minimum values
            if ((posX < 35 || posZ < 35) && posX + posZ < 30 && soundSource.isPlaying)
            {
                if (!mMent.sneak)
                {
                    if (!burrow.isBurrowed)
                    {
                        lurk = false;
                        agent.isStopped = false;
                        agent.speed = 9f;
                        agent.acceleration = 19f;
                        agent.SetDestination(player.transform.position - transform.position * .06f);
                        hasChased = true;
                        print("solo");
                        inLurkState = false;
                    }
                }
            }
            //sound detection; maximum values
            if (posX + posZ < 50 && posX + posZ > 30 && (posX < 35 && posZ < 35) && soundSource.isPlaying)
            {
                if (!mMent.sneak)
                {
                    if (!burrow.isBurrowed)
                    {
                        lurk = false;
                        agent.isStopped = false;
                        agent.speed = 9f;
                        agent.acceleration = 19f;
                        agent.SetDestination(player.transform.position - transform.position * .06f);
                        hasChased = true;
                        print("cumbo");
                        inLurkState = false;
                    }
                }
            }*/

        if (sc.currentStatus == LizardStateController.StatusToPlayer.Scared)
        {
            playerDistance = 12f;
        }

        if (sc.currentStatus == LizardStateController.StatusToPlayer.Cautious)
        {
            playerDistance = 8f;
        }



        if (sc.fleeFromEnemy)
        if ((Mathf.Abs(sc.posXenemy)) > 20f || (Mathf.Abs(sc.posZenemy)) > 20f)
        {
                sc.ChangeState(sc.idleState);
            }

        if (sc.fleeFromPlayer)
        {
            if ((Mathf.Abs(sc.posX)) > playerDistance || (Mathf.Abs(sc.posZ)) > playerDistance)
            {

                sc.ChangeState(sc.idleState);
            }
        }


    }
    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.isScurrying = false;
        sc.longBreath = true;
        sc.lockPosition = true;
        sc.fleeFromPlayer = false;
        sc.fleeFromEnemy = false;
        sc.hardBreathParticle.Stop();
        // "Must've been the wind"
    }
}
