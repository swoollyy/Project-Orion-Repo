using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionForces2 : MonoBehaviour
{
    bool startGroundDragTimer = false;
    bool startGunHitbackTimer = false;

    public MouseLook mouseLook;

    float groundDragTimer;
    float gunHitbackTimer;

    public Animator animator;

    public PhysicMaterial slideyMat;

    public PlayerMovementLevel2 mMent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (startGroundDragTimer)
            groundDragTimer += Time.deltaTime;
        if (startGunHitbackTimer)
            gunHitbackTimer += Time.deltaTime;

        if (groundDragTimer >= .3f)
        {
            mMent.enabled = true;
            GetComponent<CapsuleCollider>().material = null;
            mouseLook.enabled = true;
            groundDragTimer = 0;
            startGroundDragTimer = false;

        }
        if (gunHitbackTimer >= .1f)
        {
            mMent.enabled = true;
            mouseLook.enabled = true;
            if (mMent.grounded)
            {
                GetComponent<CapsuleCollider>().material = null;
                startGunHitbackTimer = false;
                gunHitbackTimer = 0;
            }
        }


    }

    public void NormalAttackHitback(Vector3 backDirection, float backForce, Vector3 upDirection, float upForce)
    {
        mouseLook.enabled = false;
        mMent.enabled = false;
        GetComponent<CapsuleCollider>().material = slideyMat;
        GetComponent<Rigidbody>().AddForce(backDirection * backForce + upDirection * upForce, ForceMode.Impulse);
        startGroundDragTimer = true;
    }

    public void GunHitback(Vector3 backDirection, float backForce)
    {
        mouseLook.enabled = false;
        mMent.enabled = false;
        GetComponent<CapsuleCollider>().material = slideyMat;
        Vector3 newVelocity = GetComponent<Rigidbody>().velocity;
        newVelocity.y = 0f;
        GetComponent<Rigidbody>().velocity = newVelocity;
        GetComponent<Rigidbody>().AddForce(backDirection * backForce, ForceMode.Impulse);
        startGunHitbackTimer = true;
    }

    public void DisableMovement()
    {
        mouseLook.enabled = false;
        mMent.enabled = false;
    }

    public void SunGlareStop()
    {
        animator.SetBool("StopGlare", true);
        mMent.enabled = true;
        mouseLook.enabled = true;
    }

}
