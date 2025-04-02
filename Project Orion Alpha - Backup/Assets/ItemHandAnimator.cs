using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHandAnimator : MonoBehaviour
{

    Animator anim;
    public PlayerMovement mMent;

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
            if (mMent.isWalking || mMent.isSprinting && mMent.grounded)
            {
                anim.SetBool("IsWalking", true);
                anim.SetBool("IsIdle", false);
            }
            if(mMent.currentState == PlayerMovement.MovementState.air)
            {
                anim.SetBool("IsWalking", false);
                anim.SetBool("IsIdle", true);
            }
            if (mMent.isCrouching)
            {
                anim.SetBool("IsIdle", true);
                anim.SetBool("IsWalking", false);
            }
        }
    }
}
