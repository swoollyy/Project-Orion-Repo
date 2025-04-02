using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    float pounceTimer;
    float animTimer;
    bool attack;
    public void OnEnter(StateController sc)
    {
        sc.animator.SetBool("IsAttacking", true);
        attack = true;
        pounceTimer = 0;
        sc.hasHitPlayer = false;
    }

    public void UpdateState(StateController sc)
    {
        animTimer += Time.deltaTime;
        if(sc.agent.enabled == true)
        if (sc.agent.remainingDistance <= 10f)
        {
            attack = true;
                sc.scorpAudio.PlayAttackSound();
                sc.agent.enabled = false;

        }
        if(attack && animTimer >= .25f)
        {
            if (sc.atkCD <= 0)
            {
                pounceTimer += Time.deltaTime;
                if (pounceTimer <= .7f || !sc.hasHitPlayer)
                {
                    sc.transform.position = Vector3.Lerp(sc.transform.position, sc.player.transform.position,  5 * Time.deltaTime);
                }
                if (pounceTimer > .7f || sc.hasHitPlayer)
                {
                    sc.agent.enabled = true;
                    sc.ChangeState(sc.chaseState);

                }
            }
        }
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        sc.animator.SetBool("IsAttacking", false);
        sc.atkCD = 4f;
        sc.hasHitPlayer = false;
        pounceTimer = 0;
        attack = false;
        animTimer = 0;
    }
}

