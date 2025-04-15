using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondRoomState : TutorialIState
{
    Ray ray;
    bool onlyOnce;
    RaycastHit hit;
    public void OnEnter(TutorialController sc)
    {
    }

    public void UpdateState(TutorialController sc)
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (sc.gc.currentHitObject != null)
        {
            if (sc.gc.currentHitObject.name == "Contract")
            {
                sc.gc.helperText.text = "Press 'E' to sign the Contract";
                sc.gc.helperText.enabled = true;
                if (Input.GetKeyDown("e") && sc.gc.currentHitObject.name == "Contract" && Physics.Raycast(ray, out hit, 5f, sc.gc.layMask))
                {
                    sc.gc.tutText.text = "";
                    sc.gc.currentCharacter = 0;
                    onlyOnce = true;
                }
            }
            else sc.gc.helperText.enabled = false;
        }
        if(onlyOnce)
        {
            sc.gc.tutText.text = "";
            sc.gc.currentCharacter = 0;
            sc.gc.delayTimer = 5f;
            sc.gc.tutTextMaxIndex = 3;
            sc.gc.textCounter = 3;
            sc.ChangeState(sc.thirdRoomState);
        }
    }

    public void OnHurt(TutorialController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(TutorialController sc)
    {

    }
}