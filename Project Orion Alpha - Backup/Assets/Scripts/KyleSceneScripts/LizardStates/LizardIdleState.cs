using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardIdleState : LizardIState
{
    float debugTimer;

    float randomStopTime;

    bool breathTimerS;
    float breathTimer;

    public void OnEnter(LizardStateController sc)
    {
        sc.agent.speed = 0f;
        sc.agent.acceleration = 0f;
        sc.agent.angularSpeed = 0f;
        sc.animator.SetBool("isIdle", true);
        sc.animator.SetBool("isWalking", false);
        randomStopTime = Random.Range(1f, 8f);
    }

    public void UpdateState(LizardStateController sc)
    {
        debugTimer += Time.deltaTime;
        sc.agent.ResetPath();

        sc.transform.position = sc.lockedPos;

        if(sc.longBreath)
        {
            breathTimerS = true;
            sc.animator.SetBool("LongBreath", true);
            sc.breathParticle.Play();
            sc.staminaCooldown = 10f;
        }

        if (breathTimerS)
        {
            breathTimer += Time.deltaTime;
            sc.longBreath = false;
        }

        if(breathTimer >= .3f)
        {
            sc.animator.SetBool("LongBreath", false);
            breathTimerS = false;
            breathTimer = 0;
        }

        if ((Mathf.Abs(sc.posX)) < 10f && (Mathf.Abs(sc.posZ)) < 10f)
        {
            if (sc.playerInv.currentHeldItem != null)
                if (sc.playerInv.currentHeldItem.name == "Wistiria")
                    sc.ChangeState(sc.confusedState);

            if ((Mathf.Abs(sc.posX)) < sc.playerDetectionDistance && (Mathf.Abs(sc.posZ)) < sc.playerDetectionDistance)
            {
                if (sc.willFleeFromPlayer)
                {
                    if ((Mathf.Abs(sc.posX)) < sc.playerDetectionDistance && (Mathf.Abs(sc.posZ)) < sc.playerDetectionDistance && !sc.player.GetComponent<PlayerMovement>().isSneaking)
                    {
                        sc.fleeFromPlayer = true;
                        sc.ChangeState(sc.scurryState);
                    }
                    if (sc.isLookingAtPlayer)
                    {
                        if (sc.isLookingAtPlayer)
                        {
                            if (sc.playerInv.currentHeldItem != null)
                                if (sc.playerInv.currentHeldItem.name == "Wistiria")
                                {
                                    sc.ChangeState(sc.confusedState);
                                }
                                else
                                {
                                    sc.fleeFromPlayer = true;
                                    sc.ChangeState(sc.scurryState);
                                }
                        }
                    }
                }
            }
        }

        if ((Mathf.Abs(sc.posXenemy)) < 6f && (Mathf.Abs(sc.posZenemy)) < 6f)
        {
            sc.fleeFromEnemy = true;
            sc.ChangeState(sc.scurryState);
        }

        for (int i = 0; i < sc.lizardCtrl.Count; i++)
        {
            if (sc.lizardCtrl[i] != null)
                if ((Mathf.Abs(sc.lizardDistances[i].x)) < 5f && (Mathf.Abs(sc.lizardDistances[i].z)) < 5f && sc.lizardCtrl[i].isScurrying)
                {
                    if (sc.lizardCtrl[i].fleeFromEnemy)
                    {
                        sc.fleeFromEnemy = true;
                        sc.ChangeState(sc.scurryState);
                    }
                    else
                    {
                        sc.fleeFromPlayer = true;
                        sc.ChangeState(sc.scurryState);
                    }
                }
        }

        if (debugTimer >= randomStopTime)
        {
            sc.ChangeState(sc.roamState);
        }
    }
    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.staminaCooldownStart = true;
        // "Must've been the wind"
        sc.animator.SetBool("isIdle", false);
        sc.animator.SetBool("LongBreath", false);
        sc.breathParticle.Stop();
    }
}
