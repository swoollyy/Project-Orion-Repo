using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftFunction : MonoBehaviour
{
    Rigidbody rb;

    public bool moveForklift;

    public GameControllerLevel2 gc;

    public GameObject enemy;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (moveForklift)
        {
            rb.AddForce(-transform.right * 20, ForceMode.Acceleration);
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    public void Vroom()
    {
        moveForklift = true;
        rb.isKinematic = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Enemy" && moveForklift)
        {
            gc.KillMonster();
            enemy.transform.GetChild(0).GetComponent<BoxCollider>().isTrigger = true;
        }
    }



}
