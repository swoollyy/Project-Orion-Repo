using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    Vector3 distance;
    float random;
    float timer;
    bool timerStart;
    float patrolCount;
    public void OnEnter(StateController sc)
    {
        sc.agent.speed = 9f;
        sc.agent.acceleration = 19f;
        distance = sc.player.transform.position;
        random = Random.Range(0, 360f);
    }

    public void UpdateState(StateController sc)
    {
        sc.agent.SetDestination(distance);
        if(sc.agent.remainingDistance <= 1f)
        {
            timerStart = true;
            sc.transform.rotation = Quaternion.Slerp(sc.transform.rotation, Quaternion.Euler(0f, random, 0f), Time.deltaTime * 3f);
            if (timerStart)
            {
                timer += Time.deltaTime;
                timerStart = false;
            }

            //after this amount of time, turn a new direction
            if (timer > 1.5f)
            {
                patrolCount++;
                timer = 0;
                random = Random.Range(0f, 360f);
            }
        }
        if (patrolCount > 5)
            sc.ChangeState(sc.roamState);

        if ((sc.posX < 24 && (sc.posZ < 25) && sc.posX + sc.posZ < 32) || (sc.posZ < 24 && (sc.posX < 25) && sc.posZ + sc.posZ < 32))
        {
            sc.ChangeState(sc.chaseState);
        }
    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        // "Must've been the wind"
    }
}

