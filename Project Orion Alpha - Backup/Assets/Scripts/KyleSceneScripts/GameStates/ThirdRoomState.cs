using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdRoomState : TutorialIState
{
    bool onlyOnce;
    int collectCount;
    public void OnEnter(TutorialController sc)
    {

    }

    public void UpdateState(TutorialController sc)
    {
        if(sc.gc.currentHitObject != null)
        {

            if (sc.gc.currentHitObject.name == "LockerClosed" && !sc.gc.openedLocker)
            {
                sc.gc.helperText.text = "Press 'E' to grab your tactical helmet from the locker";
                sc.gc.helperText.enabled = true;
                if (Input.GetKey(KeyCode.E))
                {
                    sc.gc.openedLocker = true;
                    //sc.gc.playerInv.AddItem(sc.gc.trapObj);
                    collectCount++;
                    sc.gc.itemEquip.Play();
                    sc.gc.PowerUpHelm();
                }

            }

            else if (sc.gc.currentHitObject.tag == "Gun")
            {
                sc.gc.helperText.text = "Press 'E' to grab your plasma rifle";
                sc.gc.helperText.enabled = true;
                if (Input.GetKey(KeyCode.E))
                {
                    collectCount++;
                    sc.gc.currentHitObject = null;
                    sc.gc.helperText.enabled = false;
                    sc.gc.weaponEquip.Play();
                }
            }

            else if (sc.gc.currentHitObject.tag == "Knife")
            {
                sc.gc.helperText.text = "Press 'E' to grab your dagger";
                sc.gc.helperText.enabled = true;
                if (Input.GetKey(KeyCode.E))
                {
                    collectCount++;
                    sc.gc.currentHitObject = null;
                    sc.gc.helperText.enabled = false;
                    sc.gc.weaponEquip.Play();
                }
            }
            else sc.gc.helperText.enabled = false;

        }
        if(sc.gc.openedLocker && sc.gc.playerInv.collectedGun && sc.gc.playerInv.collectedKnife)
        {
            sc.gc.tutText.text = "";
            sc.gc.currentCharacter = 0;
            sc.gc.delayTimer = 5f;
            sc.gc.tutTextMaxIndex = 5;
            sc.gc.textCounter = 4;
            sc.ChangeState(sc.fieldState);
        }
    }

    public void OnHurt(TutorialController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(TutorialController sc)
    {
        sc.gc.helperText.enabled = false;
    }
}