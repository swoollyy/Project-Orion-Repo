using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackLizardState : IState
{
    float pounceTimer;
    public void OnEnter(StateController sc)
    {
        pounceTimer = 0;
        sc.hasHitPlayer = false;
        sc.agent.speed = 9f;
        sc.agent.acceleration = 19f;
    }

    public void UpdateState(StateController sc)
    {
        if (sc.lockedOnObject != null)
        {
            sc.transform.position = Vector3.Lerp(sc.transform.position, sc.lockedOnObject.transform.position, 10 * Time.deltaTime);
            sc.transform.forward = Vector3.Lerp(sc.transform.forward, sc.lockedOnObject.transform.forward, 5 * Time.deltaTime);
        }
        if (sc.hasHitLizard)
            sc.ChangeState(sc.roamState);
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
    }
}

