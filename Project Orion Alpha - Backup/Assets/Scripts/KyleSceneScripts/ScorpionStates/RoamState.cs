using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamState : IState
{
    int burrowChance;
    int minChance = 1;
    int maxChance = 4;
    float burrowTimer;
    bool burrowTimerStart;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetBool("IsWalking", true);
        sc.agent.speed = 4f;
        sc.agent.acceleration = 17f;
        sc.agent.angularSpeed = 2700f;
        burrowTimerStart = true;
    }

    public void UpdateState(StateController sc)
    {
        sc.NextPoint();
        if (burrowTimerStart)
            burrowTimer += Time.deltaTime;

        if(burrowTimer >= 3f)
        {
            burrowChance = Random.Range(minChance, maxChance);
            burrowTimer = 0;
        }
        if(burrowChance == 1 || sc.gc.wakeUpScorp)
        {
            sc.ChangeState(sc.burrowState);
            burrowChance = 0;
        }

        if ((Mathf.Abs(sc.posX)) < 25f && (Mathf.Abs(sc.posZ)) < 25f)
        {
            sc.animator.SetTrigger("IsDetecting");
            sc.ChangeState(sc.chaseState);
        }
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

    }
    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsWalking", false);
        // "Must've been the wind"
        burrowTimerStart = false;
    }
}
