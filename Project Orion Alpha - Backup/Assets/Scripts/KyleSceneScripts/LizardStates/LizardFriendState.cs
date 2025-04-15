using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardFriendState : LizardIState
{
    public void OnEnter(LizardStateController sc)
    {
        sc.agent.speed = 5.5f;
        sc.agent.acceleration = 19f;
        sc.rend.material = sc.friendMat;
    }

    public void UpdateState(LizardStateController sc)
    {

        if(!sc.agent.pathPending)
            if(sc.agent.remainingDistance <= sc.agent.stoppingDistance)
            {
                sc.animator.SetBool("isWalking", false);
                sc.animator.SetBool("isIdle", true);
            }
        else
            {
                sc.animator.SetBool("isWalking", true);
                sc.animator.SetBool("isIdle", false);
            }

        sc.agent.SetDestination(sc.player.transform.position);

    }

    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        // "Must've been the wind"
    }
}

