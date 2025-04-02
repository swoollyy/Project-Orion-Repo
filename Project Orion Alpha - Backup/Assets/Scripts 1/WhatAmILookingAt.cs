using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WhatAmILookingAt : MonoBehaviour
{
    private int range = 2;
    private TMP_Text text;

    public LayerMask layerMask;

    public GameObject currentObject;
    public GameController gc;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            currentObject = hit.transform.gameObject;
            text.text = hit.transform.gameObject.name;
            if(gc.openedLocker)
            {
                gc.PowerUpCrosshair();
                gc.PowerUpIndicator();
            }
        }
        else
        {
            currentObject = null;
            if(!gc.playerInv.isHoldingGun)
            gc.PowerDownCrosshair();
            text.text = "";
            gc.crosshairUITimer = 3;
        }
    }
}
