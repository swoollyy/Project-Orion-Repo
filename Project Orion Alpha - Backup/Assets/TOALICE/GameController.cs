using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.AI;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using PrimeTween;

public class GameController : MonoBehaviour
{
    public GameObject scorpion;
    public GameObject scorpionRig;
    public GameObject invBar;
    public GameObject player;
    public GameObject weaponBar;
    public GameObject trap;
    public TMP_Text hp;
    public TMP_Text helperText;
    public GameObject wall;
    public GameObject helmetUI;
    public GameObject ctrlTextHolder;
    public GameObject currentHitObject;
    public GameObject trapObj;
    public LizardStateController currentLizard;
    public TutorialController tutController;
    public Inventory playerInv;
    public LayerMask layMask;
    public TMP_Text wasdText;
    public TMP_Text spaceText;
    public TMP_Text shiftText;
    public TMP_Text ctrlText;
    public TMP_Text crouchText;
    public bool openGate;
    public Image crossHair;
    public Image helmetimageUI;
    public Image invBarUI;
    public Image invBarBGUI;
    public Image hpBarUI;
    public Image stamBarUI;
    public Image indicatorUI;
    public GameObject slot1;
    public GameObject slot2;
    public GameObject slot3;
    public GameObject slot4;
    public float helmetUITimer;
    public float crosshairUITimer;
    public bool wakeUpScorp;
    public bool isCurrentlyLooking;
    public bool openedLocker;
    public GameObject openableLocker;
    public Mesh lockerOpen;
    public Mesh lockerClosed;
    public byte helmetAlpha;
    bool turnOffHelmet;
    public bool isEnableHelmet;
    public bool isEnableCrosshair;
    public StateController scorpController;
    public GameObject trappedLizard;
    bool whileScorpSleep = true;
    bool enable = false;
    public TrapObject trapScript;
    public GameObject trapPrefab;
    public bool canTrapScorpion;
    public AudioMixerSnapshot tutorialMixer;
    public AudioMixerSnapshot arenaMixer;
    public AudioSource itemEquip;
    public AudioSource weaponEquip;
    public AudioSource UIAudio;
    public AudioSource hissAudio;
    public Animator knifeAnim;
    public Animator itemAnim;
    public Animator playerAnim;
    public Animator tamedLizardAnim;
    public bool playTextUI;


    public float gunCooldown;
    public float gunCurrentCooldown;

    public TMP_Text wailaText;

    public KeyCode pickupKey = KeyCode.E;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode throwKey = KeyCode.F;

    public PlayerMovement mMent;
    public MouseLook mLook;
    public OpeningScene opScene;
    public MoveCamera moveCam;

    public ItemDuplicateCheck playerInvDupCheck;

    public AnimatorEnable animEnable;

    int LayerDefault;


    public int currentCharacter;
    public int currentSecCharacter;

    public int tutTextMaxIndex;


    public TMP_Text tutText;
    public TMP_Text tutSecText;

    public string introText;
    public string introSecText;

    float textTimer;
    float secTextTimer;

    float introTextTimer;

    float camTransitionTimer;
    float camTransitionTimer2;

    bool startSecondIntroText;
    bool lookAtTargets;

    public bool deadScorpion;
    public bool startSuccTimer;

    float succTimer;

    bool waitToStop;
    bool untilComplete = true;
    bool zoomComplete = false;

    public bool zoomToFace;
    public bool animate;
    bool zoomIn;
    bool quickBlur;
    bool endZoomInCS;

    float reBurrowTimer;
    float toPlayerTimer;
    float waitTimer;

    public bool deadLizard;

    bool doOnce = false;
    public bool doOnce2 = false;
    bool doOnce3 = false;
    bool doOnce4 = false;
    bool doOnce5 = false;

    bool scorpWakeUp;
    bool foundLizard;
    public bool lizardCutscene;
    public bool lizardEruption = true;
    public float wakeUpTime;
    public float scorpEnableTimer;

    public bool lizardNestDialogue;

    public GameObject lizardSpawner;

    public GunDamage gunDmg;


    public float uiTextSFXtime;
    public float uiTextSpeed;

    bool startDelayTimer;
    public float delayTimer;
    bool startDisableEatTimer;
    float disableEatTimer;

    public GameObject scorpEruptCamLocation;
    public GameObject scorpFaceCamLocation;
    public GameObject camHolder;
    public GameObject ship;

    bool hasSuccd;

    public int textCounter = -1;
    int deadLizCounter = 0;

    public LizardStateController lizardPrefab;


    public List<string> UIAllText = new List<string>();
    
    public bool hasShot;
    public bool overheatTimerBool;
    public float overheatTimer;
    public ParticleSystem rechargeParticles;
    public ParticleSystem loadUpPart;
    public bool rechargePart = true;

    public AudioSource rechargeSFX;
    public AudioSource overheatSFX;

    public VolumeProfile volumeProfile;
    DepthOfField dof;
    float focalLengthValue;
    float focusDistance;

    float checkLizardTimer;

    public GameObject[] lizardFoundTag;
    public List<GameObject> lizardsFound = new List<GameObject>();

    public GameObject dustParticle1;
    public GameObject dustParticle2;


    // Start is called before the first frame update
    void Start()
    {
        uiTextSFXtime = .025f;
        uiTextSpeed = .025f;
        scorpion.SetActive(false);

        introText = "ORION PROJECT. HUNT 0-0-1-2.";

        introSecText = "DESIGNATION: SAIPH. TRAINING MODULE: SCORPIO. INITIATION START.";

        if (!volumeProfile.TryGet(out dof)) throw new System.NullReferenceException(nameof(dof));
        focalLengthValue = 1f;
        focusDistance = 1f;
        dof.focalLength.Override(focalLengthValue);
        dof.focusDistance.Override(focusDistance);

        tutText.enabled = true;

        LayerDefault = LayerMask.NameToLayer("Default");

        playerAnim.enabled = false;
        helmetimageUI.enabled = false;
        invBarUI.enabled = false;
        invBarBGUI.enabled = false;
        hpBarUI.enabled = false;
        stamBarUI.enabled = false;
        crossHair.enabled = false;
        indicatorUI.enabled = false;
        isEnableHelmet = false;
        slot1.SetActive(false);
        slot2.SetActive(false);
        slot3.SetActive(false);
        slot4.SetActive(false);


    }

    void Awake()
    {
        lizardFoundTag = GameObject.FindGameObjectsWithTag("Lizard");
        lizardsFound = lizardFoundTag.ToList();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

        gunCurrentCooldown -= Time.deltaTime;

        checkLizardTimer += Time.deltaTime;

        if (checkLizardTimer >= 5f)
        {
            lizardFoundTag = GameObject.FindGameObjectsWithTag("Lizard");
            lizardsFound = lizardFoundTag.ToList();
            scorpController.UpdateList();
            FindObjectsOfType<LizardStateController>().ToList().ForEach(LizardStateController => LizardStateController.UpdateList());

            checkLizardTimer = 0f;
        }



        if (scorpController.burrowAttackLizardState.getLizard)
        {
            if (trappedLizard != null)
                scorpion.GetComponent<NavMeshAgent>().SetDestination(trappedLizard.transform.position);
            if (trappedLizard == null)
            {
                if (!startDisableEatTimer)
                    scorpion.transform.GetChild(0).GetComponent<Animator>().SetBool("IsEating", true);
                scorpController.enabled = true;
                startDisableEatTimer = true;
                scorpController.burrowAttackLizardState.eatLizard = false;
                scorpController.burrowAttackLizardState.getLizard = false;
            }
        }
        if (startDisableEatTimer)
            disableEatTimer += Time.deltaTime;

        if (disableEatTimer >= .2f)
        {
            scorpion.transform.GetChild(0).GetComponent<Animator>().SetBool("IsEating", false);
            startDisableEatTimer = false;
            disableEatTimer = 0;
        }

        if (startSuccTimer)
        {
            succTimer += Time.deltaTime;

            if (succTimer >= 3f)
            {
                knifeAnim.speed = 1f;
                playerAnim.speed = 1f;
                startSuccTimer = false;
                hasSuccd = true;
                succTimer = 0;
            }

        }
        if (wakeUpScorp)
        {
            if (animEnable.BBatMax)
            {
                scorpController.isburrowAtkLizard = true;
                scorpion.SetActive(true);
                whileScorpSleep = false;
            }
        }

        if (startDelayTimer)
            delayTimer += Time.deltaTime;

        if (delayTimer >= 5f && textCounter <= UIAllText.Count)
        {
            tutText.text = "";
            currentCharacter = 0;
            introText = UIAllText[textCounter];
            startDelayTimer = false;
            opScene.moveThroughText = true;
            delayTimer = 0;
        }

        if (currentCharacter < introText.Length)
        {
            tutText.enabled = true;
            playTextUI = true;
            DisplayText();
        }
        if (playTextUI)
        {
            introTextTimer += Time.deltaTime;
            if (introTextTimer >= uiTextSFXtime)
            {
                UIAudio.Play();
                introTextTimer = 0;
            }
        }
        if (!playTextUI)
        {
            UIAudio.Stop();
            introTextTimer = 0;
        }
        if (currentCharacter >= introText.Length)
        {
            playTextUI = false;
            if (opScene.beginTutorial)
            {
                currentCharacter = 0;
            }
            if (textCounter == tutTextMaxIndex)
                waitToStop = true;




            if (opScene.moveThroughText && textCounter != tutTextMaxIndex)
            {
                UIText();
                opScene.moveThroughText = false;
            }
            startSecondIntroText = true;
        }
        if (textCounter == 0 && playTextUI)
            opScene.controlsText.SetActive(true);

        if (waitToStop)
            waitTimer += Time.deltaTime;
        else waitTimer = 0f;

        if (waitTimer >= 5f)
        {
            tutText.enabled = false;
            waitToStop = false;
        }


        if (currentSecCharacter >= introSecText.Length)
        {

            playTextUI = false;
        }


        if (startSecondIntroText)
        {
            if (currentSecCharacter < introSecText.Length)
            {
                tutSecText.enabled = true;
                playTextUI = true;
                DisplaySecondaryText();
            }
        }

        if (deadLizard && !doOnce2)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            textCounter = 6;
            tutTextMaxIndex = 6;
            lizardCutscene = true;
            player.GetComponent<CapsuleCollider>().material = null;
            EnableBlackbarsForScorpion();
        }
        if (lizardCutscene)
        {
            mMent.enabled = false;
            mLook.enabled = false;
            playerInv.enabled = false;
            moveCam.enabled = false;
            overheatSFX.enabled = false;
            rechargeSFX.enabled = false;

            print("not doing eruption function");
            if (animEnable.BBatMax)
            {
                if (!foundLizard)
                    BeginScorpEruption();
                ScorpionEruptionCam();

            }

        }

        if (Input.GetKeyDown("r"))
        {
            SceneManager.LoadScene("Generation");
        }

        if (gunCurrentCooldown <= 4f && hasShot)
        {

            if (!rechargeSFX.isPlaying)
            {
                rechargeSFX.Play();
                loadUpPart.Play();
                hasShot = false;
            }


        }

        if (gunCurrentCooldown <= .5f && !rechargePart)
        {
            rechargeParticles.Play();
            rechargePart = true;
        }

        if (overheatTimerBool)
            overheatTimer += Time.deltaTime;

        if (overheatTimer >= .5f)
            if (!overheatSFX.isPlaying)
            {
                overheatSFX.Play();
                overheatTimer = 0f;
                overheatTimerBool = false;
            }


        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f, layMask))
        {
            currentHitObject = hit.transform.gameObject;
            if (currentHitObject.tag != "Room" && tutController.currentState != tutController.firstRoomState)
            {
                isCurrentlyLooking = true;
                if (currentHitObject.tag == "Plant")
                {
                    if (Input.GetKeyDown("e"))
                    {
                        playerInv.AddItem(currentHitObject);
                    }
                }


            }

            if (currentHitObject.tag == "Ship" && hasSuccd)
            {
                isCurrentlyLooking = true;
                helperText.text = "Press 'E' to leave this planet";
                helperText.enabled = true;
                if (Input.GetKeyDown("e"))
                {
                    PlayerPrefs.SetInt("Level1Complete", 1);
                    PlayerPrefs.Save();
                    SceneManager.LoadScene("Ship");
                }
            }



            if (currentHitObject.tag == "Lizard")
            {
                currentLizard = currentHitObject.GetComponent<LizardStateController>();
                if (Input.GetKeyDown("e"))
                {
                    if (playerInv.currentHeldItem != null)
                        if (playerInv.currentHeldItem.tag == "Plant")
                        {
                            int index = playerInvDupCheck.CheckIndex("Plant");
                            if (mMent.isCrouching)
                            {
                                itemAnim.SetBool("FeedLizard", true);
                                playerAnim.SetBool("FeedLizard", true);
                                tamedLizardAnim = currentHitObject.transform.GetChild(1).GetComponent<Animator>();
                                playerInv.lizzyFriend = currentHitObject;
                                currentHitObject.GetComponent<LizardStateController>().hasBeenTamed = true;
                                playerInv.RemoveItem(index);
                                playerInv.hasTamed = true;
                            }
                        }
                }
            }

            if (playerInv.currentHeldWeapon != null)
            {
                if (playerInv.currentHeldWeapon.name == "Knife")
                {
                    if (currentHitObject.tag == "Enemy")
                    {
                        if (scorpController.currentState == scorpController.trappedState || scorpController.currentState == scorpController.stunnedState)
                        {
                            helperText.text = "Click to stab the scorpion!";
                            helperText.enabled = true;
                            if (Input.GetMouseButtonDown(0))
                            {
                                scorpController.animator.SetTrigger("IsStabbed");
                                scorpController.scorpAudio.PlayDamageSound();
                                scorpController.ChangeState(scorpController.idleState);
                                scorpController.hp -= 1;
                            }
                        }
                    }

                }

            }

        }
        else
        {
            currentHitObject = null;
            if (playerInv.currentHeldWeapon == playerInv.gunHands)
                crossHair.enabled = true;
            else if (playerInv.currentHeldWeapon != playerInv.gunHands)
                crossHair.enabled = false;

            helperText.enabled = false;
            isCurrentlyLooking = false;
        }
        if (playerInv.currentHeldItem != null)
        {
            if (playerInv.currentHeldItem.name == "Trap" || playerInv.currentHeldItem.name == "Trap(Clone)")
            {
                if (currentHitObject != null)
                    if (currentHitObject.tag == "Terrain")
                    {
                        helperText.text = "Press 'E' to place your trap";
                        helperText.enabled = true;
                    }
                if (Input.GetKeyDown("e") && currentHitObject.name == "Area" && Physics.Raycast(ray, out hit, 5f, layMask))
                {
                    GameObject instObj = Instantiate(trapPrefab, new Vector3(hit.point.x, hit.point.y + .1f, hit.point.z), Quaternion.Euler(-90f, 90f, 90f));
                    instObj.name = "Trap";
                    instObj.transform.localScale = new Vector3(.6f, .6f, .6f);
                    instObj.SetActive(true);
                    instObj.layer = LayerDefault;
                    instObj.GetComponent<Item>().isCollected = false;
                    trapScript = instObj.GetComponent<TrapObject>();
                    trapScript.isOpen = true;
                    instObj.GetComponent<TrapObject>().sc = scorpController;
                    playerInv.removeTrap = true;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab) && openedLocker)
        {
            isEnableHelmet = true;
            PowerUpHelm();

        }


        if (lookAtTargets)
        {
            if (trappedLizard != null)
            {
                Camera.main.transform.LookAt(trappedLizard.transform);
                print("looking at - lizard");
            }
            else if (trappedLizard == null && !zoomToFace)
            {
                Camera.main.transform.LookAt(scorpionRig.transform.position);
                print("looking at - scorpion eruption");
            }
            if (zoomToFace)
            {
                zoomIn = true;
                print("looking at - scorpion face");
                Camera.main.transform.LookAt(scorpController.scorpionFace.transform.position);
            }

            mMent.enabled = false;
            playerInv.enabled = false;
            mLook.enabled = false;
            moveCam.enabled = false;

        }



        if (isEnableHelmet)
        {
            helmetUITimer += Time.deltaTime;
        }
        if (isEnableCrosshair)
        {
            crosshairUITimer += Time.deltaTime;
        }
        if (helmetUITimer > 3)
        {
            helmetimageUI.enabled = false;
            invBarUI.enabled = false;
            invBarBGUI.enabled = false;
            hpBarUI.enabled = false;
            stamBarUI.enabled = false;
            crossHair.enabled = false;
            indicatorUI.enabled = false;
            isEnableHelmet = false;
            slot1.SetActive(false);
            slot2.SetActive(false);
            slot3.SetActive(false);
            slot4.SetActive(false);
            helmetUITimer = 0;
        }
        if (crosshairUITimer > 3)
        {
            crossHair.enabled = false;
            indicatorUI.enabled = false;
            isEnableCrosshair = false;
            crosshairUITimer = 0;
        }


        if (scorpController.hp <= 0)
            SceneManager.LoadScene("Generation");


        if (zoomIn && !zoomComplete)
            ZoomInCam();

        if (quickBlur)
            QuickBlur();
        if (endZoomInCS)
            reBurrowTimer += Time.deltaTime;

        if (reBurrowTimer >= 1.8f)
        {
            if (!doOnce)
            {
                scorpController.ChangeState(scorpController.burrowState);
                doOnce = true;
            }
        }

        if (reBurrowTimer >= 3.5f && !doOnce2)
        {
            DisableBlackbarsForScorpion();
        }


        if (!scorpion.activeSelf && !deadLizard)
        {
            foundLizard = false;
            scorpController.isScaringPlayer = false;
            if (trappedLizard != null)
                if (trappedLizard.activeSelf)
                    deadLizard = true;
        }

        if (deadLizard && !scorpion.activeSelf)
        {
            scorpWakeUp = true;
            wakeUpTime = Random.Range(6f, 9f);
            deadLizard = false;
        }
        else if (scorpion.activeSelf)
            scorpWakeUp = false;

        print("scorp timer to WAKE SCORP " + scorpEnableTimer);

        if (scorpWakeUp)
        {
            scorpEnableTimer += Time.deltaTime;


            if (playerInv.isHoldingLizard)
            {
                if (!doOnce3)
                {
                    scorpEnableTimer = 0;
                    doOnce3 = true;
                }
            }
            else doOnce3 = false;



            if (scorpEnableTimer >= wakeUpTime)
            {
                scorpion.SetActive(true);
                print("timer at max WAKE SCORPION");
                if (!playerInv.isHoldingLizard)
                {
                    scorpController.isburrowAtkLizard = true;
                    print("isnt holding lizard");
                    scorpEnableTimer = 0;
                }
                else
                {
                    scorpController.eruptNearPlayer = true;
                    print("is holding lizard");
                    scorpEnableTimer = 0;
                }
                scorpWakeUp = false;
            }
        }

        if (scorpion.transform.position.y <= -25f)
        {
            scorpion.GetComponent<Rigidbody>().velocity = Vector3.zero;
            scorpController.ChangeState(scorpController.burrowState);
            scorpion.transform.position = new Vector3(0f, -19f, 0f);
            scorpWakeUp = false;
            if (trappedLizard == null)
                EndGlobalEvents();
            scorpController.damageArea.SetActive(false);
            scorpion.SetActive(false);
        }

        if (lizardNestDialogue)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            textCounter = 10;
            tutTextMaxIndex = 11;
            lizardNestDialogue = false;
        }

        if (playerInv.isHoldingLizard)
        {
            if (!doOnce4)
            {
                tutText.text = "";
                currentCharacter = 0;
                delayTimer = 5f;
                textCounter = 12;
                tutTextMaxIndex = 13;
                doOnce4 = true;
            }
        }

        if (deadScorpion)
        {
            if (doOnce4)
            {
                tutText.text = "";
                currentCharacter = 0;
                delayTimer = 5f;
                textCounter = 14;
                tutTextMaxIndex = 14;
                doOnce4 = false;
            }
        }

        if (hasSuccd)
        {
            if (!doOnce5)
            {
                tutText.text = "";
                currentCharacter = 0;
                delayTimer = 5f;
                textCounter = 15;
                tutTextMaxIndex = 15;
                doOnce5 = true;
            }
        }

    }

    public void PowerUpHelm()
    {
        isEnableHelmet = true;
        helmetUITimer = 0;
        helmetimageUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        invBarUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        invBarBGUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        hpBarUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        stamBarUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        crossHair.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        indicatorUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);

        slot1.GetComponent<TMP_Text>().color = new Color32(255, 255, 255, 80);
        slot2.GetComponent<TMP_Text>().color = new Color32(255, 255, 255, 80);
        slot3.GetComponent<TMP_Text>().color = new Color32(255, 255, 255, 80);
        slot4.GetComponent<TMP_Text>().color = new Color32(255, 255, 255, 80);


        helmetimageUI.enabled = true;
        invBarUI.enabled = true;
        invBarBGUI.enabled = true;
        hpBarUI.enabled = true;
        stamBarUI.enabled = true;
        crossHair.enabled = true;
        indicatorUI.enabled = true;
        slot1.SetActive(true);
        slot2.SetActive(true);
        slot3.SetActive(true);
        slot4.SetActive(true);
    }

    public void PowerUpCrosshair()
    {
        isEnableCrosshair = true;
        crossHair.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        crossHair.enabled = true;
    }

    public void PowerDownCrosshair()
    {
        isEnableCrosshair = false;
        crossHair.enabled = false;
    }

    public void PowerUpIndicator()
    {
        isEnableCrosshair = true;
        indicatorUI.GetComponent<Image>().color = new Color32(255, 255, 255, 80);
        indicatorUI.enabled = true;
    }
    void DisplayText()
    {
        textTimer += Time.deltaTime;
        if (textTimer > uiTextSpeed)
        {
            tutText.text += introText[currentCharacter];
            currentCharacter++;
            textTimer = 0f;
        }
    }

    void DisplaySecondaryText()
    {
        secTextTimer += Time.deltaTime;
        if (secTextTimer > uiTextSpeed)
        {
            tutSecText.text += introSecText[currentSecCharacter];
            currentSecCharacter++;
            secTextTimer = 0f;
        }
    }

    void UIText()
    {


        if (textCounter < tutTextMaxIndex)
            textCounter++;
        startDelayTimer = true;
    }

    void EnableBlackbarsForScorpion()
    {
        mMent.enabled = false;
        mLook.enabled = false;
        if (playerInv.isHoldingItem)
            playerInv.itemHands.SetActive(false);
        else if (playerInv.isHoldingKnife)
            playerInv.knifeHands.SetActive(false);
        else playerInv.gunHands.SetActive(false);
        playerInv.enabled = false;
        moveCam.enabled = false;
        playerAnim.enabled = false;
        dof.focalLength.Override(focalLengthValue);
        dof.focusDistance.Override(focusDistance);
        animEnable.enableBlackbars = true;
    }

    void DisableBlackbarsForScorpion()
    {

        float duration = 2f;
        toPlayerTimer += Time.deltaTime;
        float t = toPlayerTimer / duration;
        zoomToFace = false;


        animEnable.disableBlackbars = true;
        //camHolder.transform.localPosition = Vector3.Lerp(camHolder.transform.localPosition, new Vector3(0f, .5580f, 0f), t);

        if (t >= .99f)
        {
            print("disable function");
            Camera.main.fieldOfView = 65;
            focalLengthValue = 1f;
            focusDistance = 1f;
            dof.focalLength.Override(focalLengthValue);
            dof.focusDistance.Override(focusDistance);
            lookAtTargets = false;
            mMent.enabled = true;
            playerInv.enabled = true;
            lizardEruption = false;
            playerAnim.enabled = true;
            playerAnim.SetBool("IsADShooting", false);
            playerAnim.SetBool("IsAiming", false);
            gunDmg.isLeftClicking = false;
            gunDmg.isRightClicking = false;
            overheatSFX.enabled = true;
            rechargeSFX.enabled = true;
            mLook.enabled = true;
            moveCam.enabled = true;

            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            textCounter = 7;
            tutTextMaxIndex = 9;
            reBurrowTimer = 0f;
            endZoomInCS = false;
            doOnce2 = true;
        }

    }

    void BeginScorpEruption()
    {
        for (int i = 0; i < lizardsFound.Count; i++)
        {
            if (lizardsFound[i].GetComponent<LizardHealth>().currentHealth <= 0)
            {
                trappedLizard = lizardsFound[i];
                deadLizard = false;
                wakeUpScorp = true;
                deadLizCounter += 2;
                foundLizard = true;
                break;
            }
        }

    }

    void ScorpionEruptionCam()
    {
        float duration = 2f;

        lookAtTargets = true;

        print("ScorpionEruptionCam function");

        if (trappedLizard != null)
            scorpEruptCamLocation.transform.position = new Vector3(trappedLizard.transform.position.x + 10f, 10f, trappedLizard.transform.position.z + 7f);

        if (untilComplete)
        {
            print("panToEruption function");
            camTransitionTimer += Time.deltaTime;
            float t = camTransitionTimer / duration;
            camHolder.transform.position = Vector3.Lerp(camHolder.transform.position, scorpEruptCamLocation.transform.position, t * .03f);
            if (t >= 1.5f)
            {
                lizardCutscene = false;
                untilComplete = false;
                return;
            }
        }
    }

    void ZoomInCam()
    {

        float duration = 2f;
        lookAtTargets = true;

        if (!zoomComplete)
        {
            untilComplete = false;
            scorpFaceCamLocation.transform.position = new Vector3(scorpion.transform.position.x, scorpion.transform.position.y + .5f, scorpion.transform.localPosition.z - 4f);
            print("zoom in function");
            camTransitionTimer2 += Time.deltaTime;
            float ti = camTransitionTimer2 / duration;
            camHolder.transform.position = Vector3.Lerp(camHolder.transform.position, scorpFaceCamLocation.transform.position, ti * .1f);

            if (ti >= 1.1f && ti <= 1.2f)
                if (!hissAudio.isPlaying)
                    hissAudio.Play();

            if (ti >= 1.4f)
            {
                print("hissTimer SHAKESHAKESHAKE");
                Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.5f, .5f, .5f), duration: 1f, frequency: 40);
                endZoomInCS = true;
                quickBlur = true;
                scorpFaceCamLocation.transform.position = new Vector3(scorpion.transform.position.x, scorpion.transform.position.y + .5f, scorpion.transform.localPosition.z - 6f);
                zoomIn = false;
                zoomComplete = true;
                playerAnim.enabled = false;
                return;
            }
        }
    }


    void QuickBlur()
    {

        if (quickBlur)
        {
            focalLengthValue = Mathf.Lerp(focalLengthValue, 85f, Time.deltaTime * 2f);
            focusDistance = Mathf.Lerp(focusDistance, 2.04f, Time.deltaTime * 2f);
            Camera.main.fieldOfView = 82;
            dof.focalLength.Override(focalLengthValue);
            dof.focusDistance.Override(focusDistance);
            if (focalLengthValue >= 83f)
                quickBlur = false;
        }

    }

    public void GlobalEvents()
    {
        dustParticle1.SetActive(true);
        dustParticle2.SetActive(true);
        Camera.main.farClipPlane = 32f;

    }

    public void EndGlobalEvents()
    {
        dustParticle1.SetActive(false);
        dustParticle2.SetActive(false);
        Camera.main.farClipPlane = 1000f;

    }

    void QuitGame()
    {
        Application.Quit();
    }


}
