using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PrimeTween;

public class BurrowState : IState
{
    float burrowDelay;

    bool doOnce;

    public void OnEnter(StateController sc)
    {
        sc.scorpAudio.PlayStartBurrowSound();
        sc.explodeTimer = 0;
    }

    public void UpdateState(StateController sc)
    {
        burrowDelay += Time.deltaTime;
        sc.animator.SetBool("IsBurrowing", true);
        sc.BeginBurrow();
        if (burrowDelay > .15f)
        {
            if(!doOnce)
            {
                Tween.ShakeLocalPosition(sc.camHolder.transform, strength: new Vector3(.5f, .5f, .5f), duration: 1.2f, frequency: 40);
                doOnce = true;
            }
            sc.StartDecline();
        }

    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsBurrowing", false);
        burrowDelay = 0f;
        doOnce = false;
    }
}

