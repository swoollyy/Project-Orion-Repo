using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseLizardState : IState
{
    float atkChanceTimer;
    bool atkChanceTimerStart;
    float debugTimer;
    public void OnEnter(StateController sc)
    {
        sc.agent.speed = 8f;
        sc.agent.acceleration = 19f;
    }

    public void UpdateState(StateController sc)
    {
            if (sc.lockedOnObject != null)
            {
            Debug.Log("DUDEIMRUNNING");
            sc.agent.SetDestination(sc.lockedOnObject.transform.position);
            debugTimer += Time.deltaTime;

        }

        if (sc.agent.remainingDistance < 4f && debugTimer >= 1f)
        {
            Debug.Log("DUDE");
            sc.ChangeState(sc.attackLizardState);
        }
            if(sc.agent.remainingDistance > 25f)
            sc.ChangeState(sc.roamState);


    }
    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        debugTimer = 0f;
    }
}

