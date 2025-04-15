using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardTrappedState : LizardIState
{
    float trappedTimer;
    float trappedFloat;
    public void OnEnter(LizardStateController sc)
    {
        sc.isTrapped = true;
        trappedFloat = Random.Range(8f, 12f);
        sc.agent.ResetPath();
    }

    public void UpdateState(LizardStateController sc)
    {
        trappedTimer += Time.deltaTime;
        if (trappedTimer >= trappedFloat)
        {
            sc.ChangeState(sc.idleState);
        }
    }
    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.isTrapped = false;
        trappedFloat = 0;
        trappedTimer = 0;
    }
}
