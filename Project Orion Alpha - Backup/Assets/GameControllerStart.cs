using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControllerStart : MonoBehaviour
{

    public GameObject startButton;
    public GameObject levelButton;
    public GameObject settingsButton;
    public GameObject creditsButton;
    public GameObject backToMainButton;

    public GameObject poLogo;
    public GameObject escapePodLogo;

    public GameObject level1Button;
    public GameObject level2Button;

    public Button waterLevelButton;
    public Image waterLevelImage;

    public GameObject lockedText;
    public GameObject controlsText;


    bool level1Passed;


    // Start is called before the first frame update
    void Start()
    {
        level1Button.SetActive(false);
        level2Button.SetActive(false);
        backToMainButton.SetActive(false);
        lockedText.SetActive(false);
        controlsText.SetActive(false);


        Cursor.lockState = CursorLockMode.None;


        Screen.fullScreen = true;


        if (PlayerPrefs.GetInt("Level1Complete", 0) == 1) // Default to 0 if not set
        {
            waterLevelButton.onClick.AddListener(OnButtonClick);
            level1Passed = true;
        }
        else
        {
            level1Passed = false;
        }



    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }


    public void AdvanceToLevel1()
    {
        SceneManager.LoadScene("Generation");
    }

    public void AdvanceToShip()
    {
        SceneManager.LoadScene("Ship");
    }

    public void AdvanceToLevel2()
    {
        SceneManager.LoadScene("OilRig");
    }

    public void AdvanceToLevelSelect()
    {
        startButton.SetActive(false);
        levelButton.SetActive(false);
        settingsButton.SetActive(false);
        creditsButton.SetActive(false);

        backToMainButton.SetActive(true);
        level1Button.SetActive(true);
        level2Button.SetActive(true);
        if (!level1Passed)
        {
            lockedText.SetActive(true);
            waterLevelImage.color = new Color32(87, 87, 87, 255);
        }
        else
        {
            lockedText.SetActive(false);
            waterLevelImage.color = new Color32(255, 255, 255, 255);
        }
        
    }

    public void ReturnToMainMenu()
    {
        startButton.SetActive(true);
        levelButton.SetActive(true);
        settingsButton.SetActive(true);
        creditsButton.SetActive(true);
        lockedText.SetActive(false);
        poLogo.SetActive(true);
            level1Button.SetActive(false);
        backToMainButton.SetActive(false);
        level2Button.SetActive(false);
        controlsText.SetActive(false);
    }

    void OnButtonClick()
    {
        AdvanceToLevel2();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SettingsMenu()
    {
        startButton.SetActive(false);
        levelButton.SetActive(false);
        settingsButton.SetActive(false);
        creditsButton.SetActive(false);

        backToMainButton.SetActive(true);
        controlsText.SetActive(true);


    }



}
