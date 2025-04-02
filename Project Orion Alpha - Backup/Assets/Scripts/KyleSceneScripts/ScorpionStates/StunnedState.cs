using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : IState
{
    bool unstunTimerStart;
    float unstunTimer;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetTrigger("IsStunned");
        sc.agent.ResetPath();
        sc.agent.speed = 0f;
        sc.agent.acceleration = 0f;
        unstunTimerStart = true;

    }

    public void UpdateState(StateController sc)
    {
        sc.agent.SetDestination(sc.transform.position);
        if (unstunTimerStart)
            unstunTimer += Time.deltaTime;

        if (unstunTimer > 8f)
            sc.ChangeState(sc.chaseState);
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        unstunTimerStart = false;
        unstunTimer = 0f;
    }
}

