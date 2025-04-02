using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardChaseState : LizardIState
{

    float timer;

    public void OnEnter(LizardStateController sc)
    {
        sc.agent.speed = 2f;
        sc.agent.acceleration = 19f;
        sc.agent.stoppingDistance = 2f;
        sc.animator.SetBool("isWalking", true);
        sc.agent.SetDestination(sc.player.transform.position);
    }

    public void UpdateState(LizardStateController sc)
    {

        if (sc.isTamed)
            sc.currentStatus = LizardStateController.StatusToPlayer.Tamed;

        NavMeshHit hit;

        if (!sc.agent.pathPending)
        {
            if (Vector3.Distance(sc.transform.position, sc.player.transform.position) < sc.agent.stoppingDistance + .7f)
            {
                sc.animator.SetBool("isIdle", true);
                sc.animator.SetBool("isWalking", false);
                sc.agent.ResetPath();
                Debug.Log("path RESET");
            }
            else if (Vector3.Distance(sc.transform.position, sc.player.transform.position) > sc.agent.stoppingDistance)
            {
                Debug.Log("path distance " + Vector3.Distance(sc.transform.position, sc.player.transform.position));
                if (NavMesh.SamplePosition(sc.player.transform.position, out hit, 2.0f, NavMesh.AllAreas))
                {
                    Debug.Log("path FOUND");
                    sc.animator.SetBool("isWalking", true);
                    sc.animator.SetBool("isIdle", false);
                    sc.agent.SetDestination(hit.position);
                }
            }
        }
        else Debug.Log("path is pending!!!");






    }


    public void OnHurt(LizardStateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(LizardStateController sc)
    {
        sc.animator.SetBool("isIdle", false);
        sc.animator.SetBool("isWalking", false);
        // "Must've been the wind"
        sc.lockPosition = true;
    }
}

