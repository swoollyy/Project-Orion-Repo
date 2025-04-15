using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldState : TutorialIState
{
    public void OnEnter(TutorialController sc)
    {
        sc.gc.helmetUI.SetActive(true);
        sc.gc.invBar.SetActive(true);
        sc.gc.weaponBar.SetActive(true);
        sc.gc.isEnableHelmet = true;
        sc.gc.PowerUpHelm();
    }

    public void UpdateState(TutorialController sc)
    {

    }

    public void OnHurt(TutorialController sc)
    {
        // Transition to Hurt State
    }
    public void OnExit(TutorialController sc)
    {

    }
}
