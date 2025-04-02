using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowIdleState : IState
{
    int unburrowChance;
    int minChance = 1;
    int maxChance = 4;
    float unburrowTimer;
    bool unburrowTimerStart;
    bool hasRolled;
    float attackOrUnburrowRNG;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetBool("IsBurrowWalking", true);
        sc.agent.speed = 8f;
        sc.agent.acceleration = 21f;
        sc.agent.angularSpeed = 5400f;
        sc.patrolPosition++;
        unburrowTimerStart = true;
        sc.scorpAudio.PlayDiggingSound();

    }

    public void UpdateState(StateController sc)
    {
        sc.NextPoint();
        if (unburrowTimerStart)
            unburrowTimer += Time.deltaTime;

        if (unburrowTimer >= 5f)
        {
            unburrowChance = Random.Range(minChance, maxChance);
            unburrowTimer = 0;
        }
        if (unburrowChance == 1)
        {
            sc.ChangeState(sc.endBurrowState);
            unburrowChance = 0;
        }

        if ((sc.posX < 24 && (sc.posZ < 25) && sc.posX + sc.posZ < 32) || (sc.posZ < 24 && (sc.posX < 25) && sc.posZ + sc.posZ < 32))
        {
            if(hasRolled == false)
            {
                attackOrUnburrowRNG = Random.Range(1, 5);
                hasRolled = true;
            }
        }
        if (attackOrUnburrowRNG == 1 || attackOrUnburrowRNG == 2)
        {
            sc.ChangeState(sc.burrowAttackState);
            hasRolled = false;
        }
        if(attackOrUnburrowRNG > 2)
        {
            sc.ChangeState(sc.endBurrowState);
            hasRolled = false;
        }

    }
    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsBurrowWalking", false);
        // "Must've been the wind"
        unburrowTimerStart = false;
        sc.scorpAudio.StopDiggingSound();
    }
}
