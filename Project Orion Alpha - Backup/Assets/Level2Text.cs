using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Level2Text : MonoBehaviour
{
    float uiTextSFXtime;
    float uiTextSpeed;

    float introTextTimer;
    float waitTimer;
    float textTimer;

    int currentCharacter;
    public int textCounter;
    public int tutTextMaxIndex;
    int tempTextCounter;
    int tempIndex;


    public List<string> UIAllText = new List<string>();

    public Item keyItem;

    public AudioSource UIAudio;

    public bool doOnce;
    bool doOnceLeverCheck;
    bool doOnceFlipLever;
    bool doOnceWaterRise;
    bool doOnceSecondLever;
    bool doOnceFinalLever;
    bool doOnceSeeMonster;
    bool startDelayTimer;
    bool playTextUI;
    bool waitToStop;
    public bool oilReleased;
    bool doOnceKeyCollect;
    bool doOnceSucc;
    bool doOnceMonsterBait;
    float delayTimer;



    public TMP_Text tutText;
    public TMP_Text questHelperText;
    public string introText;

    public GameObject player;
    public GameObject monster;
    public LeverCheck leverCheck;
    public GameControllerLevel2 gc;
    public ForkliftHitDetection forkliftDetection;
    public water waterScript;

    Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        uiTextSFXtime = .035f;
        uiTextSpeed = .035f;
        introText = UIAllText[0];
        tutTextMaxIndex = 3;
        cam = Camera.main;
        questHelperText.text = "Stand by.";
    }

    // Update is called once per frame
    void Update()
    {
        if (startDelayTimer)
        {
            delayTimer += Time.deltaTime;
        }

        if (delayTimer >= 5f && textCounter <= UIAllText.Count)
        {
            tutText.text = "";
            currentCharacter = 0;
            doOnce = false;
            introText = UIAllText[textCounter];
            startDelayTimer = false;
            delayTimer = 0;
        }

        if (currentCharacter < introText.Length && !startDelayTimer)
        {
            tutText.enabled = true;
            playTextUI = true;
            DisplayText();
        }
        if (playTextUI)
        {
            introTextTimer += Time.deltaTime;
            if (introTextTimer >= uiTextSFXtime && introText[currentCharacter - 1] != ' ')
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





            if (textCounter <= tutTextMaxIndex && !doOnce)
            {
                UIText();
                print("poopie");
                doOnce = true;
            }



        }

        if (textCounter >= 3 && textCounter < 10)
            questHelperText.text = "Find levers across the rig to refuel your ship";
        if (textCounter >= 10 && textCounter < 13)
            questHelperText.text = "Move and pop open an oil tanker to bait the monster";
        if (textCounter >= 13 && textCounter < 15)
            questHelperText.text = "Find the forklift key on the 1st floor";
        if (textCounter >= 15 && textCounter < 16)
            questHelperText.text = "Use the key to start up the forklift";
        if (textCounter >= 16 && textCounter < 17)
            questHelperText.text = "Activate the drill";
        if (textCounter >= 17 && textCounter < 18)
            questHelperText.text = "Harvest a blood sample";
        if (textCounter >= 18)
            questHelperText.text = "Escape the rig with your refueled ship";


        if (waterScript.isMoving && !doOnceWaterRise)
        {
            if(currentCharacter < introText.Length)
            {
                tempTextCounter = textCounter - 1;
                tempIndex = tutTextMaxIndex;
            }
            else if(currentCharacter == introText.Length)
            {
                tempTextCounter = textCounter;
                tempIndex = tutTextMaxIndex;
            }    
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;
            textCounter = 8;
            tutTextMaxIndex = 8;
            doOnceWaterRise = true;
        }

        if (waitToStop)
            waitTimer += Time.deltaTime;
        else waitTimer = 0f;

        if (waitTimer >= 5f)
        {
            tutText.enabled = false;
            waitToStop = false;
        }

        if(leverCheck.onCount == 3 && !doOnceSecondLever)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            textCounter = 7;
            waitTimer = 0;
            tutTextMaxIndex = 7;
            doOnceSecondLever = true;
        }

        if (leverCheck.onCount == 4 && !doOnceFinalLever)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            textCounter = 10;
            waitTimer = 0;

            tutTextMaxIndex = 11;
            doOnceFinalLever = true;
        }

        if (oilReleased)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 12;
            tutTextMaxIndex = 12;
            oilReleased = false;
        }
        if(monster.GetComponent<MonsterNav>().monsterBaited && !doOnceMonsterBait)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 13;
            tutTextMaxIndex = 14;
            doOnceMonsterBait = true;
        }

        if(keyItem.isCollected && !doOnceKeyCollect)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 15;
            tutTextMaxIndex = 15;
            doOnceKeyCollect = true;
        }

        if(forkliftDetection.monsterPinned)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 16;
            tutTextMaxIndex = 16;
            forkliftDetection.monsterPinned = false;
        }
        if(gc.drillActivate)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 17;
            tutTextMaxIndex = 17;
            gc.drillActivate = false;
        }

        if(gc.hasSuccd && !doOnceSucc)
        {
            tutText.text = "";
            currentCharacter = 0;
            delayTimer = 5f;
            waitTimer = 0;

            textCounter = 18;
            tutTextMaxIndex = 18;
            doOnceSucc = true;
        }

        Plane[] monsterPlane = GeometryUtility.CalculateFrustumPlanes(cam);
        Renderer monsterRend = monster.transform.GetChild(0).GetChild(1).GetComponent<Renderer>();
        Vector3 direction = (monster.transform.position - player.transform.position).normalized;


        if (GeometryUtility.TestPlanesAABB(monsterPlane, monsterRend.bounds) && monster.GetComponent<MonsterNav>().isOnLand && monster.activeSelf)
        {
            if (Physics.Raycast(player.transform.position, direction, out RaycastHit hit, Mathf.Infinity))
            {
                if(hit.transform.gameObject.name != "Box014")
                {
                    print(hit.transform.gameObject + " hitter");
                    Debug.DrawRay(player.transform.position, direction * hit.distance, Color.blue, 2f);
                    if (hit.transform.gameObject.tag == "Enemy" && !doOnceSeeMonster)
                    {
                        if (currentCharacter < introText.Length)
                        {
                            tempTextCounter = textCounter - 1;
                            tempIndex = tutTextMaxIndex;
                        }
                        else if (currentCharacter == introText.Length)
                        {
                            tempTextCounter = textCounter;
                            tempIndex = tutTextMaxIndex;
                        }
                        tutText.text = "";
                        currentCharacter = 0;
                        delayTimer = 5f;
                        waitTimer = 0;

                        textCounter = 9;
                        tutTextMaxIndex = 9;
                        doOnceSeeMonster = true;
                    }
                }

            }



        }

        if (!doOnceLeverCheck || !doOnceFlipLever)
        for(int i = 0; i < leverCheck.levers.Count; i++)
        {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
                Renderer rend = leverCheck.levers[i].GetComponent<Renderer>();
                if (Vector3.Distance(player.transform.position, leverCheck.levers[i].transform.position) < 5f && GeometryUtility.TestPlanesAABB(planes, rend.bounds))
            {
                if(!doOnceLeverCheck)
                {
                    tutText.text = "";
                    currentCharacter = 0;
                    delayTimer = 5f;
                        waitTimer = 0;

                        textCounter = 4;
                    tutTextMaxIndex = 4;
                    doOnceLeverCheck = true;
                }
            }

                if (leverCheck.levers[i].isOn)
                {
                    tutText.text = "";
                    currentCharacter = 0;
                    delayTimer = 5f;
                    waitTimer = 0;

                    textCounter = 5;
                    tutTextMaxIndex = 5;
                    doOnceFlipLever = true;
                }
        }

    }

void UIText()
{
        if(textCounter == tutTextMaxIndex)
        {
            if (textCounter == 8 && !leverCheck.finalSegment)
            {
                textCounter = tempTextCounter;
                tutTextMaxIndex = tempIndex;
                waitToStop = true;
            }
            if(textCounter == 9 && !leverCheck.finalSegment)
            {
                textCounter = tempTextCounter - 1;
                tutTextMaxIndex = tempIndex;
            }
            waitToStop = true;

        }

        if (textCounter < tutTextMaxIndex)
        {
            textCounter++;
            startDelayTimer = true;
        }


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

}
