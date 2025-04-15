using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DamagePlayer : MonoBehaviour
{
    public StateController sc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player" && !sc.isCharging && sc.gc.playerInv.isHoldingLizard)
        {
                    SceneManager.LoadScene("Generation");
        }

        if (col.gameObject.tag == "Lizard")
        {
            sc.unhingeTrap = true;
            for (int i = 0; i < sc.currentLizards.Count; i++)
            {
                if (col.gameObject.name == sc.currentLizards[i].name)
                {
                    sc.lizardName = sc.currentLizards[i].name;
                    sc.removeFromLizardLists = true;
                    sc.isburrowAtkLizard = false;
                    sc.gc.wakeUpScorp = false;
                    sc.RemoveGameObject(i);
                    Destroy(col.gameObject);
                }
            }
            sc.hasHitLizard = true;
        }

        if(col.gameObject.tag == "RockStructure")
        {
            col.gameObject.GetComponent<RockStructureBehavior>().hasBeenHit = true;
            col.gameObject.GetComponent<RockStructureBehavior>().ShootUp();
        }



    }
    public void Reset()
    {
    }
}
