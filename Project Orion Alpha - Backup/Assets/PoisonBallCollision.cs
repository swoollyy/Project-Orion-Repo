using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBallCollision : MonoBehaviour
{

    GameObject player;

    float timer;

    bool doOnce = false;
    bool inRange;
    Vector3 savedDirection;
    Vector3 lastPosition;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if(timer >= 15f)
        {
            Destroy(this.gameObject);
        }

                if (Vector3.Distance(transform.position, player.transform.position) > 7f && !inRange)
                {
            doOnce = false;
                    transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 4f);
                            savedDirection = (transform.position - lastPosition).normalized;
                lastPosition = transform.position;
                }
                else
                {
                    inRange = true;
            transform.position += savedDirection * 4f * Time.deltaTime;
                }
        print("hissTimer BALL");

    }


    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "Player")
        {
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.GetComponent<CollisionForces>().NormalAttackHitback(savedDirection, 5f, transform.up, 1.2f);
            col.gameObject.GetComponent<PlayerHealthLevel1>().PoisonDamage();
            Destroy(this.gameObject);
        }

        if (col.gameObject.tag == "Lizard")
        {
            col.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            col.gameObject.GetComponent<Rigidbody>().AddForce(savedDirection * 3f, ForceMode.Impulse);
            Destroy(this.gameObject);
        }


    }

}
