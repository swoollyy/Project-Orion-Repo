using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrappedState : IState
{
    float trappedTimer;
    float trappedFloat;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetBool("IsTrapped", true);
        trappedFloat = Random.Range(8f, 12f);
        if(sc.agent.enabled)
        sc.agent.ResetPath();
    }

    public void UpdateState(StateController sc)
    {
        trappedTimer += Time.deltaTime;
        if (trappedTimer >= trappedFloat)
        {
            sc.ChangeState(sc.idleState);
        }
    }
    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsTrapped", false);
        trappedFloat = 0;
        trappedTimer = 0;
        sc.unhingeTrap = true;

    }
}
