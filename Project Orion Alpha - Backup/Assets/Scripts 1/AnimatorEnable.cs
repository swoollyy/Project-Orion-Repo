using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorEnable : MonoBehaviour
{

    public Animator itemHandsAnim;
    Animator camAnim;

    public MouseLook mLook;
    public GameController gc;
    public MoveCamera camMovement;
    public Inventory playerInv;

    public GameObject blackBarBottom;
    public GameObject blackBarTop;


    public bool enableBlackbars;
    public bool disableBlackbars;

    public bool BBatMax;


    float elapsedTime;
    float elapsedTime2;
    float duration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        camAnim = GetComponent<Animator>();

        blackBarBottom.transform.localPosition = new Vector3(0f, -607f, 0f);
        blackBarTop.transform.localPosition = new Vector3(0f, 607f, 0f);
        blackBarBottom.SetActive(false);
        blackBarTop.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        print("enable? " + BBatMax);
        print("disable? " + disableBlackbars);


        if (enableBlackbars && !disableBlackbars)
        {
            disableBlackbars = false;
            blackBarBottom.SetActive(true);
            blackBarTop.SetActive(true);
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            blackBarTop.transform.localPosition = Vector3.Lerp(blackBarTop.transform.localPosition, new Vector3(0f, 511f, 0f), t * .2f);
            blackBarBottom.transform.localPosition = Vector3.Lerp(blackBarBottom.transform.localPosition, new Vector3(0f, -511f, 0f), t * .2f);

            if (t >= 1f)
            {
                enableBlackbars = false;
                BBatMax = true;
            }
        }

        if(disableBlackbars)
        {
            enableBlackbars = false;
            BBatMax = false;
            elapsedTime2 += Time.deltaTime;
            float t = elapsedTime2 / duration;
            blackBarTop.transform.localPosition = Vector3.Lerp(blackBarTop.transform.localPosition, new Vector3(0f, 610f, 0f), t * .2f);
            blackBarBottom.transform.localPosition = Vector3.Lerp(blackBarBottom.transform.localPosition, new Vector3(0f, -610f, 0f), t * .2f);
            if(t >= .6f)
            {
                blackBarBottom.SetActive(false);
                blackBarTop.SetActive(false);
                disableBlackbars = false;
            }
        }

        if(!enableBlackbars && !disableBlackbars)
        {
            elapsedTime = 0;
            elapsedTime2 = 0;

        }

    }

    void DisableAnim()
    {
        mLook.enabled = true;
    }


    void FeedLizard()
    {
        mLook.enabled = false;
        camMovement.enabled = false;
        GetComponent<PlayerMovement>().enabled = false;
        playerInv.isHoldingItem = false;
        playerInv.isHoldingFruit = false;
        playerInv.enabled = false;
        camAnim.SetBool("FeedLizard", false);
        itemHandsAnim.SetBool("FeedLizard", false);
        EnableBlackbars();
    }

    void StopLizardFeed()
    {
        print("wrongness");
        GetComponent<PlayerMovement>().enabled = true;
        mLook.enabled = true;
        camMovement.enabled = true;
        playerInv.enabled = true;
    }

    void TriggerLizardFeed()
    {
        gc.tamedLizardAnim.SetBool("EatFood", true);
    }

    void ResetLizardFeed()
    {
        gc.tamedLizardAnim.SetBool("EatFood", false);
    }

    public void EnableBlackbars()
    {
        elapsedTime = 0;
        enableBlackbars = true;
    }

    public void DisableBlackbars()
    {
        elapsedTime2 = 0;
        disableBlackbars = true;
    }


}
