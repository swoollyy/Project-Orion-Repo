using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100;
    public float maxStamina = 100;
    public float health;
    public float stamina;
    public bool isOnFire;
    public bool wasOnFire;

    PlayerMovementLevel2 mMent;
    public GameControllerLevel2 gc;

    float uiTimer;

    float debugTimer;
    float recoverStamTimer;
    float damageTimer;

    public Image healthBar;
    public Image stamBar;

    bool doOnceStamina;
    bool isTakingDamage;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        mMent = GetComponent<PlayerMovementLevel2>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(.548f, .828f, health / maxHealth);
        stamBar.fillAmount = Mathf.Lerp(0f, 1f, stamina / maxStamina);

        if (health <= 0f)
        {
            gc.GameOver();
        }

        if (stamina <= 0f)
        {
            stamina = 0;
            mMent.unableToSprint = true;
        }

        if(stamina >= 10f && mMent.unableToSprint)
        {
            mMent.unableToSprint = false;
        }


        if (health < maxHealth && !isTakingDamage)
        {
            RecoverHealth();
        }
        if (health >= maxHealth)
        {
            health = maxHealth;
        }
        if(stamina >= maxStamina)
        {
            stamina = maxStamina;
        }

        if (health <= 10)
            mMent.isCrippled = true;
        else mMent.isCrippled = false;

        if (mMent.isOily && wasOnFire)
        {
            TakeDamage(.02f);
        }

        if(wasOnFire)
        {
            debugTimer += Time.deltaTime;
        }

        if(debugTimer >= 3f)
        {
            wasOnFire = false;
            debugTimer = 0f;
        }


        if (!mMent.isSprinting)
        {
            recoverStamTimer += Time.deltaTime;
            if(recoverStamTimer > 2.5f)
            {
                stamina += .08f;
            }
        }
        else recoverStamTimer = 0f;

        if (isTakingDamage)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= 8f)
            {
                isTakingDamage = false;
            }
        }
        else damageTimer = 0f;



    }

    public void TakeDamage(float amount)
    {
        health-=amount;
        if(gc != null)
            gc.ShowUI();
        isTakingDamage = true;

    }

    void RecoverHealth()
    {
        if(gc != null)
        gc.ShowUI();
        health += .01f;

    }

    public void UpdateStamina(float amount)
    {
        stamina -= amount;
        if (gc != null)
            gc.ShowUI();

    }

}
