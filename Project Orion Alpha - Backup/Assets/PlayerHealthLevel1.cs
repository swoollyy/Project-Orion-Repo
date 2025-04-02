using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthLevel1 : MonoBehaviour
{

    public float maxHealth = 100;
    public float health;

    public bool isPoisoned;
    bool isTakingDamage;

    PlayerMovement mMent;
    public GameController gc;

    int poisonTicks;
    int currentPoisonTicks;


    float poisonTimer;
    float damageTimer;

    public Image healthBar;



    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        mMent = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(.548f, .828f, health / maxHealth);

        if (health <= 0f)
            SceneManager.LoadScene("Generation");

        if(health < maxHealth && !isTakingDamage)
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
        health += .05f;
    }

    public void PoisonDamage()
    {
        isPoisoned = true;
        poisonTimer = 2;
        poisonTicks = Random.Range(3, 8);
    }

}
