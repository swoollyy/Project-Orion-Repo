using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using TMPro;

public class GameControllerLevel2 : MonoBehaviour
{

    public KeyCode pickupKey = KeyCode.E;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode throwKey = KeyCode.F;
    public KeyCode uiKey = KeyCode.Tab;

    KeyCode sprintKey = KeyCode.LeftShift;


    public GameObject ui;
    public GameObject player;
    public GameObject enemy;
    public GameObject currentHitObject;
    public GameObject lockedBarrel;
    public GameObject forkLift;
    public GameObject barrel;
    public GameObject explodeCanvas;

    float uiTimer;

    public TMP_Text explodeText;
    public TMP_Text helperText;

    public float gunCooldown;
    public float gunCurrentCooldown;

    public bool deadMonster;

    public Animator knifeAnim;
    public Animator playerAnim;


    public List<oil> oilTanks = new List<oil>();

    int emptyTanks = 0;

    WaterRise interactingLever;
    public Inventory2 inv;



    public bool isLookingAtPanel;
    public bool isLookingAtForklift;

    public bool isLookingAtBarrel;
    public bool playerCanPush;
    public bool hitBarrel;
    public bool startSuccTimer;
    bool hasSuccd;

    float succTimer;
    float explodeTimer;

    public Vector3 barrelHitPoint;

    int oilDropsCount;

    public Fire fireSpawner;


    public PlayerMovementLevel2 mMent;
    public PlayerObjectMovement objectMovement;
    public MouseLook mouseLook;

    public LayerMask groundMask;
    public LayerMask playerLayer;
    public LayerMask noLayer;

    // Start is called before the first frame update
    void Start()
    {
        ui.SetActive(false);
        objectMovement.enabled = false;
        explodeTimer = 100f;
    }

    // Update is called once per frame
    void Update()
    {
        gunCurrentCooldown -= Time.deltaTime;

        if (startSuccTimer)
            succTimer += Time.deltaTime;

        if(succTimer >= 3f)
        {
            knifeAnim.speed = 1f;
            playerAnim.speed = 1f;
            hasSuccd = true;
            startSuccTimer = false;
            succTimer = 0f;
        }

        if(hasSuccd)
        {
            fireSpawner.rapidFire = true;
            explodeCanvas.SetActive(true);
            explodeTimer -= Time.deltaTime;
            explodeText.text = explodeTimer.ToString();
        }


        if (explodeTimer <= 0f || Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("OilRig");
        }


        if (currentHitObject != null)
        {
            ShowUI();

            if (currentHitObject.tag == "FloorLevelLever")
            {
                interactingLever = currentHitObject.GetComponent<WaterRise>();
                if (Input.GetKeyDown(pickupKey))
                {
                    currentHitObject.GetComponent<WaterRise>().Flip();
                }
            }
            else if (currentHitObject.tag == "SecondLevelLever")
            {
                interactingLever = currentHitObject.GetComponent<WaterRise>();
                if (Input.GetKeyDown(pickupKey))
                {
                    currentHitObject.GetComponent<WaterRise>().Flip();
                }
            }
            else if (currentHitObject.tag == "ThirdLevelLever")
            {
                interactingLever = currentHitObject.GetComponent<WaterRise>();
                if (Input.GetKeyDown(pickupKey))
                {
                    interactingLever.Flip();
                }
            }

            if (currentHitObject.tag == "Barrel")
            {
                isLookingAtBarrel = true;
            }

            if (currentHitObject.tag == "ControlPanel")
            {
                isLookingAtPanel = true;
            }

            if (currentHitObject.tag == "Trap")
            {
                isLookingAtForklift = true;
            }

            if (currentHitObject.tag == "Ship" && hasSuccd)
            {
                helperText.text = "Press 'E' to leave this planet";
                helperText.enabled = true;
                if (Input.GetKeyDown("e"))
                {
                    SceneManager.LoadScene("OilRig");
                }
            }

        }
        else
        {
            isLookingAtBarrel = false;
            isLookingAtPanel = false;
            isLookingAtForklift = false;
        }

        if(currentHitObject != null)
        if (isLookingAtBarrel && Input.GetKey(sprintKey) && playerCanPush && currentHitObject.GetComponent<OilBehavior>().canBePushed)
        {
                if(!inv.isHoldingItem && !inv.isHoldingWeapon)
            TransitionMovement();
        }
        if (objectMovement.enabled && Input.GetKeyUp(sprintKey))
        {
            TransitionMovementToNormal();
        }

        if (isLookingAtPanel && Input.GetKeyDown(pickupKey))
        {
            ActivateDrill();
        }
        if(inv.currentHeldItem != null)
        if (isLookingAtForklift && inv.currentHeldItem.name == "Forklift Key" && Input.GetKeyDown(pickupKey))
        {
            ActivateForklift();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }



        if (ui.activeSelf)
        {
            uiTimer += Time.deltaTime;
            if (uiTimer >= 3f)
                ui.SetActive(false);
        }
        else uiTimer = 0f;

        if (Input.GetKeyDown(uiKey))
        {
            ui.SetActive(true);
        }


        if (hitBarrel)
        {
            barrel.GetComponent<OilBehavior>().SpawnOil();
            hitBarrel = false;
        }

    }
    public void UpdateOilTankCount()
    {
        for (int i = 0; i < oilTanks.Count; i++)
        {
            if (oilTanks[i].isEmpty)
                emptyTanks++;
        }

        switch (emptyTanks)
        {
            case 1:
                {
                    enemy.SetActive(true);
                    break;
                }
        }


    }

    public void ShowUI()
    {
        uiTimer = 0f;
        ui.SetActive(true);
    }


    public void TransitionMovement()
    {
        lockedBarrel = currentHitObject;
        Rigidbody rb = lockedBarrel.GetComponent<Rigidbody>();
        MeshCollider col = lockedBarrel.GetComponent<MeshCollider>();
        player.GetComponent<BoxCollider>().enabled = true;
        player.GetComponent<BoxCollider>().isTrigger = false;
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        col.excludeLayers = playerLayer;
        mMent.enabled = false;
        inv.enabled = false;
        objectMovement.enabled = true;
    }
    public void TransitionMovementToNormal()
    {

        lockedBarrel.GetComponent<MeshCollider>().isTrigger = false;
        player.GetComponent<BoxCollider>().isTrigger = true;
        player.GetComponent<BoxCollider>().enabled = false;
        Rigidbody rb = lockedBarrel.GetComponent<Rigidbody>();
        MeshCollider col = lockedBarrel.GetComponent<MeshCollider>();
        lockedBarrel.transform.parent = null;
        rb.constraints = RigidbodyConstraints.None;
        col.excludeLayers = noLayer;
        inv.enabled = true;
        mMent.enabled = true;
        lockedBarrel = null;
        objectMovement.enabled = false;
    }

    void ActivateDrill()
    {
        fireSpawner.startFire = true;
    }

    void ActivateForklift()
    {
        forkLift.GetComponent<ForkliftFunction>().Vroom();
    }

    public void KillMonster()
    {
        enemy.GetComponent<MonsterNav>().enabled = false;
        enemy.transform.GetChild(0).GetComponent<NavMeshAgent>().enabled = false;
        deadMonster = true;
    }

    void QuitGame()
    {
        Application.Quit();
    }


}
