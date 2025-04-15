using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using PrimeTween;


public class OpeningScene : MonoBehaviour
{

    public PlayerMovement mMent;
    public MouseLook mLook;
    public MoveCamera moveCam;
    public GameController gc;

    public GameObject camHolder;

    public bool startSequence;
    public bool endSequence;


    public AudioMixerSnapshot tutorialMixer;
    public AudioMixerSnapshot openSceneMixer;

    public Animator playerAnim;

    float timer;

    bool doOnce = false;
    bool shakeCam;
    bool shakeCam2;
    bool transitionMixer;

    public bool beginTutorial;
    public bool moveThroughText;

    public GameObject controlsText;

    // Start is called before the first frame update
    void Start()
    {
        startSequence = true;
        endSequence = false;
        shakeCam = true;
        shakeCam2 = true;

        camHolder.transform.localPosition = new Vector3(-10.41f, 0.477f, -2.85f);
        camHolder.transform.localRotation = Quaternion.Euler(-30.993f, 93.17f, 6.578f);
    }

    // Update is called once per frame
    void Update()
    {
        if (startSequence)
        {
            controlsText.SetActive(false);
            mMent.enabled = false;
            mLook.enabled = false;
            moveCam.enabled = false;

            timer += Time.deltaTime;
            if (shakeCam)
            {
                Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(.6f, .6f, .6f), duration: 3f, frequency: 40);
                shakeCam = false;
            }

            if (timer >= 2.5f)
            {
                if (shakeCam2)
                {
                    Tween.ShakeLocalPosition(camHolder.transform, strength: new Vector3(1f, 1f, 1f), duration: 7f, frequency: 60);
                    shakeCam2 = false;
                }
            }

        }

        if (endSequence)
        {
            if (!doOnce)
            {
                gc.tutText.text = "";
                gc.currentCharacter = 0;
                gc.tutText.transform.position += new Vector3(0f, 30f, 0f);
                gc.tutText.fontSize = 36f;


                gc.introText = "Welcome, HUNTER, to the beginning of your journey with us at Project Orion, the next step in human evolution. " +
    "Operating all across the Great Belt, we are seeking to ensure settlement within each planet in the system goes smoothly.";
                doOnce = true;
            }
            gc.tutText.enabled = true;

            moveThroughText = true;
            gc.tutSecText.enabled = false;
            startSequence = false;
            tutorialMixer.TransitionTo(1.5f);
            playerAnim.enabled = false;
            camHolder.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
            camHolder.transform.localPosition = new Vector3(0f, 0.5580001f, 0f);
        }

        if (beginTutorial)
        {
            print("wrongness");
            endSequence = false;
            playerAnim.enabled = true;
            transitionMixer = true;
            mMent.enabled = true;
            mLook.enabled = true;
            moveCam.enabled = true;
            beginTutorial = false;
        }
    }
}
