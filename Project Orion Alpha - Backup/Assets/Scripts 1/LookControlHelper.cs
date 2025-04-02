using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LookControlHelper : MonoBehaviour
{
    public WhatAmILookingAt waila;
    public GameController gc;
    public Inventory inv;

    public TMP_Text text;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(waila.currentObject != null)
        {
            if (waila.currentObject.GetComponent<Item>() != null)
                if (waila.currentObject.GetComponent<Item>().canBeCollected)
                PickupText();
        }
        else text.text = "";

        if (inv.isHoldingGun)
            DisplayGunControlsText();
        if (inv.isHoldingKnife)
            DisplayKnifeControlsText();
    }
    private void PickupText()
    {
        if(gc.pickupKey == KeyCode.Mouse0)
        text.text = "Left Click: Pick Up";
        else
        text.text = gc.pickupKey.ToString() + ": Pick up";
    }

    public void DisplayGunControlsText()
    {
        text.text = "F: Throw <br>Left Click: Shoot <br>Right Click: Aim";
    }

    public void DisplayKnifeControlsText()
    {
        text.text = "F: Throw";
    }
}
