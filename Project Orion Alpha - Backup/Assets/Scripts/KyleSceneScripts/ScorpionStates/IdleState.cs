using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    float idleTimer;
    float stopIdleFloat;
    public void OnEnter(StateController sc)
    {
        stopIdleFloat = Random.Range(5f, 8f);
    }

    public void UpdateState(StateController sc)
    {
        idleTimer += Time.deltaTime;
        if(idleTimer >= stopIdleFloat)
        {
            sc.ChangeState(sc.roamState);
            idleTimer = 0;
        }

        if ((Mathf.Abs(sc.posX)) < 40f && (Mathf.Abs(sc.posZ)) < 40f)
        {
            sc.ChangeState(sc.chaseState);
        }


        /*if (Mathf.Abs(sc.closestDistance.x) < 15f && Mathf.Abs(sc.closestDistance.z) < 15f && sc.debugTimer > 2f)
        {
            sc.lockedOnObject = sc.closestDistanceLizard;
            sc.ChangeState(sc.chaseLizardState);
        }*/
    }
    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        stopIdleFloat = 0;
        idleTimer = 0;
    }
}
