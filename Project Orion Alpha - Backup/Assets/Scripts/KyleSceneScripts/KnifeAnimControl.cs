using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAnimControl : MonoBehaviour
{

    public GameObject knifeItem;

    public GameController gc;
    public Inventory inv;
    public MouseLook mLook;

    Animator knifeAnim;
    public Animator camAnim;
    public Animator sparksAnim;
    public Animator lineRendAnim;


    public int clickerCount;

    public bool isStabbing;
    public bool isSlashing;

    bool startResetTimer;
    float resetTimer;
    float slashTimer;

    public bool knifeAnimating;

    void Start()
    {
        knifeAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (knifeItem.GetComponent<Item>().isCollected)
        {
            knifeAnim.enabled = true;
        }
        else knifeAnim.enabled = false;

        if (Input.GetMouseButtonDown(0) && inv.isHoldingKnife)
        {
            clickerCount += 1;
            knifeAnimating = true;
        }
        if (Input.GetMouseButtonUp(0) && inv.isHoldingKnife && clickerCount == 1)
        {
            clickerCount += 1;
        }
        if (clickerCount == 1)
        {
            slashTimer += Time.deltaTime;
        }
        if (clickerCount == 1 && slashTimer >= .2f)
        {
            isSlashing = true;
            knifeAnim.SetBool("IsSlashing", true);
            camAnim.enabled = true;
            camAnim.SetBool("IsSlashing", true);
            sparksAnim.SetBool("IsSlashing", true);
            lineRendAnim.SetBool("IsSlashing", true);
            clickerCount++;
        }
        if (clickerCount == 2)
        {
            isStabbing = true;
            if (Input.GetMouseButtonUp(0))
            {
                knifeAnim.SetBool("IsStabbing", true);
                camAnim.enabled = true;
                camAnim.SetBool("IsStabbing", true);
                sparksAnim.SetBool("IsStabbing", true);
            }
            clickerCount = 0;
            startResetTimer = true;
            slashTimer = 0;
        }
        if (startResetTimer)
            resetTimer += Time.deltaTime;
        if(resetTimer > .05f)
        {
            camAnim.SetBool("IsSlashing", false);
            knifeAnim.SetBool("IsSlashing", false);
            sparksAnim.SetBool("IsSlashing", false);
            lineRendAnim.SetBool("IsSlashing", false);
        }
        if (resetTimer >= .1f)
        {
            knifeAnimating = false;
            knifeAnim.SetBool("IsStabbing", false);
            camAnim.SetBool("IsStabbing", false);
            sparksAnim.SetBool("IsStabbing", false);
            resetTimer = 0;
            startResetTimer = false;
        }
    }
}
