using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthLevel1 : MonoBehaviour
{

    public float maxHealth = 100;
    public float maxStamina = 100;
    public float health;
    public float stamina;

    public bool isPoisoned;
    bool isTakingDamage;

    PlayerMovement mMent;
    public GameController gc;
    public TutorialController tutController;

    int poisonTicks;
    int currentPoisonTicks;


    float poisonTimer;
    float damageTimer;
    float recoverStamTimer;

    public Image healthBar;
    public Image stamBar;

    bool doOnceStamina;



    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        stamina = maxStamina;
        mMent = GetComponent<PlayerMovement>();
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

        if (stamina >= 10f && mMent.unableToSprint)
        {
            mMent.unableToSprint = false;
        }

        if (stamina >= maxStamina)
        {
            stamina = maxStamina;
        }


        if (!mMent.isSprinting)
        {
            recoverStamTimer += Time.deltaTime;
            if (recoverStamTimer > 2.5f)
            {
                stamina += .08f;
            }
        }
        else recoverStamTimer = 0f;

        if (health < maxHealth && !isTakingDamage)
        {
            RecoverHealth();
        }
        if(health >= maxHealth)
        {
            health = maxHealth;
        }

        if (health <= 10)
            mMent.isCrippled = true;
        else mMent.isCrippled = false;

        if (isPoisoned && currentPoisonTicks <= poisonTicks)
        {
            poisonTimer += Time.deltaTime;
            if (poisonTimer > 2f)
            {
                TakeDamage(5f);
                currentPoisonTicks++;
                poisonTimer = 0;
            }


        }
        else if (currentPoisonTicks > poisonTicks)
        {
            isPoisoned = false;
            currentPoisonTicks = 0;
        }

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
        isTakingDamage = true;
        damageTimer = 0f;
        health-=amount;
        gc.PowerUpHelm();

    }

    void RecoverHealth()
    {
        gc.PowerUpHelm();
        health += .01f;
    }

    public void PoisonDamage()
    {
        isPoisoned = true;
        poisonTimer = 2;
        poisonTicks = Random.Range(3, 8);
    }

    public void UpdateStamina(float amount)
    {
        stamina -= amount;
        if(tutController.currentState == tutController.fieldState)
        gc.PowerUpHelm();

    }

}
