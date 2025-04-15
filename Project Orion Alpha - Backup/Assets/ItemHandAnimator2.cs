using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandAnimator2 : MonoBehaviour
{

    Animator anim;
    public PlayerMovementLevel2 mMent;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameObject.activeSelf)
        {
            if (mMent.isIdle)
            {
                anim.SetBool("IsIdle", true);
                anim.SetBool("IsWalking", false);
            }
            else if (mMent.isWalking)
            {
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsIdle", false);
            }
            else if (mMent.isCrouching)
            {
                anim.SetBool("IsIdle", true);
                anim.SetBool("IsWalking", false);
            }
        }
    }
}
