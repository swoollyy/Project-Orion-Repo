using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiteCollision : MonoBehaviour
{
    public bool collided;
    public GameObject player;

    public float distanceToPlayer;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (collided)
            timer += Time.deltaTime;

        if(timer > 4f)
        {
            GetComponent<BoxCollider>().isTrigger = false;
            collided = false;
        }

        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            collided = true;
            timer = 0;
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }

}
