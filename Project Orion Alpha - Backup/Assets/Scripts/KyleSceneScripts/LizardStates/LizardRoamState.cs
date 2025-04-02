using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardRoamState : LizardIState
{
    float debugTimer;
    public void OnEnter(LizardStateController sc)
    {
        sc.agent.speed = 1.2f;
        sc.agent.acceleration = 13f;
        sc.agent.angularSpeed = 2700f;
        sc.animator.SetBool("isWalking", true);
    }

    public void UpdateState(LizardStateController sc)
    {
        sc.NextPoint();
        debugTimer += Time.deltaTime;

        if ((Mathf.Abs(sc.posX)) < 10f && (Mathf.Abs(sc.posZ)) < 10f)
        {
            if (sc.playerInv.currentHeldItem != null)
                if (sc.playerInv.currentHeldItem.name == "Wistiria")
                {
                        sc.ChangeState(sc.confusedState);
                }


            if(sc.willFleeFromPlayer && sc.playerInv.currentHeldItem == null)
            {
                if ((Mathf.Abs(sc.posX)) < sc.playerDetectionDistance && (Mathf.Abs(sc.posZ)) < sc.playerDetectionDistance && !sc.player.GetComponent<PlayerMovement>().isSneaking)
                {
                    sc.fleeFromPlayer = true;
                    sc.currentStatus = LizardStateController.StatusToPlayer.Scared;
                    sc.ChangeState(sc.scurryState);
                }
                if(sc.isLookingAtPlayer)
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

        if ((Mathf.Abs(sc.posXenemy)) < 6f && (Mathf.Abs(sc.posZenemy)) < 6f)
        {
            sc.fleeFromEnemy = true;
            sc.ChangeState(sc.scurryState);
        }

        for (int i = 0; i < sc.lizardCtrl.Count; i++)
        {
            if(sc.lizardCtrl[i] != null)
            if ((Mathf.Abs(sc.lizardDistances[i].x)) < 5f && (Mathf.Abs(sc.lizardDistances[i].z)) < 5f && sc.lizardCtrl[i].isScurrying)
            {
                    if(sc.lizardCtrl[i].fleeFromEnemy)
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




    }
    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        // "Must've been the wind"
        sc.animator.SetBool("isWalking", false);
    }
}
