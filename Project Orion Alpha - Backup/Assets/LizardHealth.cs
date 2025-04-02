using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LizardHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth = 100f;

    public GameController gc;

    bool doOnce = false;

    public GameObject bloodParticle;

    // Start is called before the first frame update
    void Start()
    {

        

    }

    // Update is called once per frame
    void Update()
    {
        if(currentHealth <= 0)
        {
            GetComponent<NavMeshAgent>().enabled = false;
            transform.GetChild(1).GetComponent<Animator>().enabled = false;
            transform.eulerAngles = new Vector3(0f, 0f, -180f);
            transform.GetChild(1).GetComponent<RotateToGroundLizard>().enabled = false;
            transform.GetChild(1).localPosition = Vector3.zero;
            GetComponent<LizardStateController>().enabled = false;
            GetComponent<BoxCollider>().material = null;
            if (!doOnce)
            {
                gc.deadLizard = true;
                GameObject bloodPart = Instantiate(bloodParticle, new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f), this.transform);
                gc.trappedLizard = this.gameObject;
                gameObject.AddComponent<Item>();
                doOnce = true;
            }

        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }
}
