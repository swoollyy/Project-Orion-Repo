using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{

    public float maxHealth = 100;
    public float health;

    public bool isOnFire;
    public bool wasOnFire;

    PlayerMovementLevel2 mMent;
    public GameControllerLevel2 gc;

    float uiTimer;

    float debugTimer;

    public Image healthBar;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        mMent = GetComponent<PlayerMovementLevel2>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.fillAmount = Mathf.Lerp(.548f, .828f, health / maxHealth);

        if (health <= 0f)
            SceneManager.LoadScene("OilRig");

        if(mMent.isOily && wasOnFire)
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


    }

    public void TakeDamage(float amount)
    {
        health-=amount;
        gc.ShowUI();

    }

}
