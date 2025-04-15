using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurrowAttackState : IState
{
    public void OnEnter(StateController sc)
    {
        sc.scorpAudio.PlayDiggingSound();
    }

    public void UpdateState(StateController sc)
    {
        if(sc.agent.enabled == true)
            sc.agent.SetDestination(sc.burrowTracker.transform.position);
            sc.agent.speed = 35f;
            sc.agent.acceleration = 75f;
            sc.agent.angularSpeed = 5000f;
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.atkCD = .5f;
        sc.scorpAudio.StopDiggingSound();
    }
}

