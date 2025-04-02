using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInWater : MonoBehaviour
{

    public Transform topMap;
    public Transform player;

    public LayerMask waterMask;

    float timer;
    float drownTimer;

    // Start is called before the first frame update
    void Start()
    {
        drownTimer = 3f;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerInWater())
        {
            Physics.gravity = new Vector3(0f, -.79f, 0f);
            print("ooga");

            timer += Time.deltaTime;

            if(timer > 12f)
            {
                drownTimer += Time.deltaTime;

                if(drownTimer >= 1.5f)
                {
                    player.GetComponent<PlayerHealth>().TakeDamage(10f);
                    drownTimer = 0f;
                }
            }

        }
        else
        {
            Physics.gravity = new Vector3(0f, -9.81f, 0f);
            timer = 0f;
            print("booga");
        }
    }

    public bool IsPlayerInWater()
    {
        RaycastHit hit;
        if (Physics.Raycast(topMap.position, -transform.up, out hit, Mathf.Infinity, waterMask))
        {
            if (player.position.y < hit.point.y)
                return true;
        }
        return false;
    }

}
