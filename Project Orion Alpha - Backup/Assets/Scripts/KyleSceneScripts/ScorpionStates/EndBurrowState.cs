using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndBurrowState : IState
{
    float burrowDelay;
    public void OnEnter(StateController sc)
    {
        sc.scorpAudio.PlayStartBurrowSound();

    }

    public void UpdateState(StateController sc)
    {
        burrowDelay += Time.deltaTime;
        sc.EndBurrow();
        if (burrowDelay > .6f)
        {
            sc.StartRise();
        }

    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        burrowDelay = 0f;

    }
}

