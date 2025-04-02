using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WhatAmILookingAt2 : MonoBehaviour
{
    private int range = 2;
    public TMP_Text text;

    public LayerMask layerMask;

    public GameObject currentObject;

    public GameControllerLevel2 gc2;
    public GameControllerShip gcShip;

    public GameObject sphere;


    // Start is called before the first frame update
    void Start()
    {
        sphere.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, range, layerMask))
        {
            currentObject = hit.transform.gameObject;
            if(gc2 != null)
            {
                if (currentObject.tag != "Barrel")
                {
                    gc2.currentHitObject = currentObject;
                    text.text = hit.transform.gameObject.name;
                }
                if (Physics.Raycast(ray, out hit, .8f, layerMask))
                {
                    currentObject = hit.transform.gameObject;
                    gc2.currentHitObject = currentObject;
                    text.text = hit.transform.gameObject.name;
                }
            }
            if (gcShip != null)
            {
                    currentObject = hit.transform.gameObject;
                gcShip.currentHitObject = currentObject;
                    text.text = hit.transform.gameObject.name;

                if(sphere != null)
                {
                    if (gcShip.panel1Hovered)
                    {
                        sphere.SetActive(true);
                        sphere.transform.position = hit.point;
                    }
                    else if (gcShip.panel2Hovered)
                    {
                        sphere.SetActive(true);
                        sphere.transform.position = hit.point;
                    }
                    else sphere.SetActive(false);
                }    

            }



        }
        else
        {
            currentObject = null;
            if (gc2 != null)
                gc2.currentHitObject = null;
            else if (gcShip != null)
                gcShip.currentHitObject = null;
            text.text = ""; 
        }

    }
}
