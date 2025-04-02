using UnityEngine.Events; 
using UnityEngine;

public class GunFiring : MonoBehaviour
{
    public UnityEvent onGunShoot;
    public float cooldown;
    public float currentCooldown;

    public bool automatic = false;

    public Animator animator;

    private void Start()
    {
        currentCooldown = cooldown;
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        if (automatic)
        {
            if (Input.GetMouseButton(0))
            {
                if (currentCooldown <= 0f)
                {
                    animator.SetTrigger("Fire");
                    onGunShoot?.Invoke();
                    currentCooldown = cooldown;
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (currentCooldown <= 0f)
                {
                    animator.SetTrigger("Fire");
                    onGunShoot?.Invoke();
                    currentCooldown = cooldown;

                }
            }
        }

        if(Input.GetMouseButton(1))
        {
            print("down");
            animator.SetBool("IsAiming", true);
        }
        else
        {
            animator.SetBool("IsAiming", false);
        }
       

        currentCooldown -= Time.deltaTime;
    }
}
