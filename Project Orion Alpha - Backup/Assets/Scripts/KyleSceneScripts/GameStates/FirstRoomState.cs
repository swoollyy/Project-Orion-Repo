using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstRoomState : TutorialIState
{
    bool wentForward;
    bool wentBackward;
    bool wentLeft;
    bool wentRight;
    bool jumped;
    bool sprinted;
    bool sneaked;
    bool crouched;
    bool onlyOnce;
    public void OnEnter(TutorialController sc)
    {
        onlyOnce = false;
        sc.gc.tutTextMaxIndex = 0;
    }

    public void UpdateState(TutorialController sc)
    {


        if (Input.GetKeyDown("w") && sc.gc.ctrlTextHolder.activeSelf)
        {
            wentForward = true;
        }
        if (Input.GetKeyDown("a") && sc.gc.ctrlTextHolder.activeSelf)
        {
            wentBackward = true;
        }
        if (Input.GetKeyDown("s") && sc.gc.ctrlTextHolder.activeSelf)
        {
            wentLeft = true;
        }
        if (Input.GetKeyDown("d") && sc.gc.ctrlTextHolder.activeSelf)
        {
            wentRight = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) && sc.gc.ctrlTextHolder.activeSelf)
        {
            jumped = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && sc.gc.ctrlTextHolder.activeSelf)
        {
            sprinted = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && sc.gc.ctrlTextHolder.activeSelf)
        {
            crouched = true;
        }
        if (Input.GetKeyDown("c") && sc.gc.ctrlTextHolder.activeSelf)
        {
            sneaked = true;
        }
        if(wentForward && wentBackward && wentLeft && wentRight && sc.gc.ctrlTextHolder.activeSelf)
        {
            sc.gc.wasdText.color = Color.green;
        }
        if(jumped && sc.gc.ctrlTextHolder.activeSelf)
            sc.gc.spaceText.color = Color.green;
        if (sprinted && sc.gc.ctrlTextHolder.activeSelf)
            sc.gc.shiftText.color = Color.green;
        if (crouched && sc.gc.ctrlTextHolder.activeSelf)
            sc.gc.ctrlText.color = Color.green;
        if (sneaked && sc.gc.ctrlTextHolder.activeSelf)
            sc.gc.crouchText.color = Color.green;

        if (wentForward && wentBackward && wentLeft && wentRight && jumped && sprinted && sneaked && crouched && !onlyOnce && sc.gc.ctrlTextHolder.activeSelf)
        {
            sc.gc.tutText.text = "";
            sc.gc.currentCharacter = 0;
            onlyOnce = true;
        }
        if(onlyOnce)
        {
            sc.gc.tutText.text = "";
            sc.gc.ctrlTextHolder.SetActive(false);
            sc.gc.currentCharacter = 0;
            sc.gc.delayTimer = 5f;
            sc.gc.tutTextMaxIndex = 1;
            sc.gc.textCounter = 2;
            sc.ChangeState(sc.secondRoomState);
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