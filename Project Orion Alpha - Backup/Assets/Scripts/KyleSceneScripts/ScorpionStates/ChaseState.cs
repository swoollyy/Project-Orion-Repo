using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : IState
{
    float burrowAtkChanceTimer;
    float resetAnimTimer;
    bool burrowAtkChanceTimerStart;
    int burrowAtkRNG;
    float atkChanceTimer;
    bool atkChanceTimerStart;
    int normalAtkRNG;
    public void OnEnter(StateController sc)
    {
        burrowAtkChanceTimerStart = true;
        burrowAtkChanceTimer = 0;
        resetAnimTimer = 0;
        sc.agent.speed = 3.5f;
        sc.agent.acceleration = 19f;
    }

    public void UpdateState(StateController sc)
    {
        sc.agent.speed = 3.5f;
        sc.agent.acceleration = 19f;
        if (sc.agent != null)
        if (burrowAtkChanceTimerStart)
            burrowAtkChanceTimer += Time.deltaTime;

        if(burrowAtkChanceTimer >= .1f)
        {
            sc.animator.ResetTrigger("IsDetecting");

        }

        if(burrowAtkChanceTimer >= .5f)
        {
            sc.agent.SetDestination(sc.player.transform.position);
            sc.animator.SetBool("IsWalking", true);
        }

        if (burrowAtkChanceTimer >= 1.5f)
        {
            burrowAtkRNG = Random.Range(1, 4);
            burrowAtkChanceTimer = 0;
        }
        if(burrowAtkRNG == 1)
        {
            burrowAtkChanceTimerStart = false;
            sc.animator.SetBool("IsHissing", true);
            sc.animator.SetBool("IsWalking", false);

            sc.agent.ResetPath();

            resetAnimTimer += Time.deltaTime;

            if(resetAnimTimer >= .1f)
            {
                sc.animator.SetBool("IsHissing", false);
            }
            if(resetAnimTimer >= 2.1f)
            {
                sc.isScaringPlayer = true;
                sc.ChangeState(sc.burrowState);
            }

        }

        if (sc.agent.enabled)
            if (sc.posX <= 3f  && sc.posZ <= 3f && sc.atkCD <= 0)
            {
                sc.ChangeState(sc.attackState);
            }
        //IF DAMAGED -- ENABLE WEATHERSTATE
          //sc.ChangeState(sc.enableWeatherState);

        //if the player is too far, begin to patrol/lurk
        if (sc.player.transform.position.x - sc.transform.position.x > sc.maxChaseDistance)
        {
            sc.ChangeState(sc.patrolState);
        }
        if (sc.player.transform.position.x - sc.transform.position.x < -sc.maxChaseDistance)
        {
            sc.ChangeState(sc.patrolState);
        }
        if (sc.player.transform.position.z - sc.transform.position.z > sc.maxChaseDistance)
        {
            sc.ChangeState(sc.patrolState);
        }
        if (sc.player.transform.position.z - sc.transform.position.z < -sc.maxChaseDistance)
        {
            sc.ChangeState(sc.patrolState);
        }
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsWalking", false);
        burrowAtkRNG = 0;

    }
}

