using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameControllerShip : MonoBehaviour
{

    public float gunCooldown;
    public float gunCurrentCooldown;

    public GameObject currentHitObject;

    public GameObject level1Panel;
    public GameObject level2Panel;

    public GameObject ui;

    public Renderer level1Renderer;
    public Renderer level2Renderer;

    public bool panel1Hovered;
    public bool panel2Hovered;

    bool whileSelectingLevel;

    public TMP_Text helperText;

    bool level1Passed = true;

    float uiTimer;

    // Start is called before the first frame update
    void Start()
    {
        whileSelectingLevel = false;
        level1Panel.SetActive(false);
        level2Panel.SetActive(false);


        if (PlayerPrefs.GetInt("Level1Complete") == 1) // Default to 0 if not set
        {
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
        gunCurrentCooldown -= Time.deltaTime;

        if (currentHitObject != null)
        {
            ShowUI();
            if (currentHitObject.tag == "ControlPanel")
            {
                if (!whileSelectingLevel)
                {
                    helperText.text = "Press 'E' to set your destination.";
                    helperText.enabled = true;
                    if (Input.GetKeyDown("e"))
                    {
                        LevelSelect();
                    }
                }
            }
            else
            {
                helperText.text = "";
                helperText.enabled = false;
            }

            if (currentHitObject.tag == "LevelPanels")
            {

                if (currentHitObject.name == "Saiph")
                {
                    panel1Hovered = true;
                    level1Renderer.material.color = new Color32(255, 255, 255, 255);
                    level2Renderer.material.color = new Color32(87, 87, 87, 255);
                    level1Panel.transform.localScale = new Vector3(.087f, .087f, .087f);

                    if(Input.GetMouseButtonDown(0))
                    {
                        SceneManager.LoadScene("Generation");
                    }

                }
                else
                {
                    panel1Hovered = false;
                    level1Renderer.material.color = new Color32(87, 87, 87, 255);
                    level1Panel.transform.localScale = new Vector3(.068f, .068f, .068f);

                }

                if (currentHitObject.name == "Alnitak")
                {
                    if(level1Passed)
                    {
                        panel2Hovered = true;
                        level2Renderer.material.color = new Color32(255, 255, 255, 255);
                        level1Renderer.material.color = new Color32(87, 87, 87, 255);
                        level2Panel.transform.localScale = new Vector3(.087f, .087f, .087f);
                        if (Input.GetKeyDown("e"))
                        {
                                SceneManager.LoadScene("OilRig");
                        }
                    }


                }
                else
                {
                    panel2Hovered = false;
                    level2Renderer.material.color = new Color32(87, 87, 87, 255);
                    level2Panel.transform.localScale = new Vector3(.068f, .068f, .068f);
                }
            }
            else
            {
                panel1Hovered = false;
                panel2Hovered = false;
                level1Renderer.material.color = new Color32(87, 87, 87, 255);
                level2Renderer.material.color = new Color32(87, 87, 87, 255);
                level1Panel.transform.localScale = new Vector3(.068f, .068f, .068f);
                level2Panel.transform.localScale = new Vector3(.068f, .068f, .068f);
            }

        }
        else
        {
            level1Renderer.material.color = new Color32(87, 87, 87, 255);
            level2Renderer.material.color = new Color32(87, 87, 87, 255);
            level1Panel.transform.localScale = new Vector3(.068f, .068f, .068f);
            level2Panel.transform.localScale = new Vector3(.068f, .068f, .068f);

            helperText.text = "";
            helperText.enabled = false;

        }

        if(ui.activeSelf)
        {
            uiTimer += Time.deltaTime;
        }
        if (uiTimer >= 3f)
        {
            ui.SetActive(false);
            uiTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }

    }

    public void LevelSelect()
    {
        whileSelectingLevel = true;
        level1Panel.SetActive(true);
        level2Panel.SetActive(true);
    }

    public void ShowUI()
    {
        uiTimer = 0f;
        ui.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
    }

}
