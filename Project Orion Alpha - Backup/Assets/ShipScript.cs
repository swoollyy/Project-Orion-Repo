using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShipScript : MonoBehaviour
{

    public AnimatorEnable animEnable;
    public OpeningScene opScene;

    bool transitionToGame;
    bool switchToPlayer;

    public Image blackScreen;

    public GameObject camHolder;

    float blackScreenAlpha;

    bool doOnce;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(transitionToGame)
        {
            blackScreenAlpha = Mathf.MoveTowards(blackScreenAlpha, 255f, Time.deltaTime * .2f);
            blackScreen.color = new Color(0f, 0f, 0f, blackScreenAlpha);
        }


        if(blackScreen.color.a >= 1f)
        {
            transitionToGame = false;
            opScene.endSequence = true;
        }

        if(opScene.endSequence)
        {
            animEnable.DisableBlackbars();
            blackScreenAlpha = Mathf.MoveTowards(blackScreenAlpha, 0f, Time.deltaTime * .6f);
            blackScreen.color = new Color(0f, 0f, 0f, blackScreenAlpha);
            doOnce = true;
        }

        if(blackScreen.color.a <= .01f && doOnce)
        {
            opScene.endSequence = false;
            opScene.beginTutorial = true;
            doOnce = false;
        }
        
    }
        
    void EnableBlackbars()
    {
        animEnable.EnableBlackbars();
    }
    void DisableBlackbars()
    {
        animEnable.DisableBlackbars();
    }

    void TransitionToGame()
    {
        transitionToGame = true;
    }



}
