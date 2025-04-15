using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardStunnedState : LizardIState
{
    float stunnedTimer;
    float stunnedFloat;

    bool isIndeedAirbourne;
    float timer;
    public void OnEnter(LizardStateController sc)
    {
        sc.transform.gameObject.name += " (Dead)";
        sc.agent.enabled = false;

        stunnedFloat = Random.Range(8f, 12f);
        sc.animator.SetBool("hasBeenShocked", true);
    }

    public void UpdateState(LizardStateController sc)
    {
        timer += Time.deltaTime;

        if(timer >= .9f)
            sc.animator.SetBool("hasBeenShocked", false);
        sc.agent.enabled = false;
    }
    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.hasBeenShot = false;
        stunnedTimer = 0;
        stunnedFloat = 0;
    }
}
