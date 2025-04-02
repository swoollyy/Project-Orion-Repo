using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableWeatherState : IState
{
    float hissTimer;
    float timer;

    int rngChance;

    float ZValue;
    float XValue;

    bool doOnce;
    bool doOnce2;
    bool poisonAttacking;
    bool stopHissAndAttack;

    public void OnEnter(StateController sc)
    {
        sc.eruptNearPlayer = false;
        sc.animator.SetBool("IsHissing", true);
        sc.explodeTimer = 0;
    }

    public void UpdateState(StateController sc)
    {
        sc.transform.LookAt(sc.player.transform.position);
        sc.agent.ResetPath();

        Debug.Log("poison attacking bool " + poisonAttacking);

        if(!poisonAttacking)
        hissTimer += Time.deltaTime;

        if(hissTimer >= 1.5f)
        {   
            if(!doOnce)
            {
                Debug.Log("global events");
                sc.gc.GlobalEvents();
            doOnce = true;
            }
        }

        if(hissTimer < 0)
            rngChance = -1;

        if (hissTimer >= 2f)
        {
            rngChance = Random.Range(1, 3);

            Debug.Log("RNG CHANGE + " + rngChance);

            if(rngChance != 1)
            {
                hissTimer = 0f;
            }
            RaycastHit hit;

            if (rngChance == 1)
            {
                while (!Physics.Raycast(new Vector3(sc.transform.position.x + XValue, sc.transform.position.y + 30f, sc.transform.position.z + ZValue), -sc.upReference.transform.up, out hit, Mathf.Infinity, sc.groundMask))
                {
                    XValue = Random.Range(4f, 10f);
                    ZValue = Random.Range(4f, 10f);

                    int negOrPos = Random.Range(1, 3);

                    if (negOrPos == 1)
                        return;
                    else
                    {
                        XValue = -XValue;
                        ZValue = -XValue;
                    }
                }


                poisonAttacking = true;

                if (Physics.Raycast(new Vector3(sc.transform.position.x + XValue, sc.transform.position.y + 30f, sc.transform.position.z + ZValue), -sc.upReference.transform.up, out hit, Mathf.Infinity, sc.groundMask))
                {
                    sc.transform.position = hit.point;
                    sc.animator.SetBool("IsDetecting", true);
                    stopHissAndAttack = true;

                    Debug.Log("hissTimer found location");


                    hissTimer = -1f;
                    rngChance = -1;
                }
            }
        }




        if(stopHissAndAttack)
        {
            timer += Time.deltaTime;
            if(timer >= .1f && timer < 1f)
            {
                Debug.Log("hissTimer animation detect false");
                sc.animator.SetBool("IsDetecting", false);
            }
            if(timer >= 1f && timer < 1.1f)
            {
                Debug.Log("hissTimer animation tail true");
                sc.animator.SetBool("IsTailAttacking", true);
            }
            if(timer >= 1.2f)
            {
                Debug.Log("hissTimer animation tail false");
                sc.animator.SetBool("IsTailAttacking", false);
            }
            if(timer >= 2.1f)
            {
                if(!doOnce2)
                {
                    sc.doOnce = false;
                    sc.SpawnPoisonBall();
                    doOnce2 = true;
                }
            }
            if(timer >= 2.8f)
            {
                sc.isScaringPlayer = true;
                sc.ChangeState(sc.burrowState);
            }


        }



    }

    public void OnHurt(StateController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(StateController sc)
    {
        poisonAttacking = false;
        timer = 0f;
        hissTimer = 0f;
        doOnce = false;
        doOnce2 = false;
    }
}

