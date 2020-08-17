using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenuCamera;
    public GameObject playerCamera;

    public GameObject mainMenuUI;
    public GameObject gameMenuUI;
    public GameObject inGameUI;
    public GameObject settingsUI;

    public GameObject playerObject;

    [HideInInspector]
    public GameObject currentUI;
    [HideInInspector]
    public bool inGame;


    GameObject lastUI;
    GameObject[] UIs;
    bool willReset = false;

    // Start is called before the first frame update
    void Start()
    {
        mainMenuCamera.SetActive(true);
        playerCamera.SetActive(false);

        UIs = new GameObject[4] { mainMenuUI, gameMenuUI, inGameUI, settingsUI };
        SwitchUI(mainMenuUI);
        inGame = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentUI == gameMenuUI)
            {
                SwitchUI(inGameUI);
            }
            else if (currentUI == inGameUI)
            {
                SwitchUI(gameMenuUI);
            }
            else if (currentUI == settingsUI)
            {
                if (lastUI == gameMenuUI)
                {
                    SwitchUI(inGameUI);
                }
                else if (lastUI == mainMenuUI)
                {
                    SwitchUI(mainMenuUI);
                }
            }
        }
    }

    void SwitchUI(GameObject newUI)
    {
        foreach (GameObject ui in UIs)
        {
            ui.SetActive(false);
        }
        newUI.SetActive(true);
        currentUI = newUI;
        inGame = (newUI == inGameUI);
        Cursor.lockState = (newUI == inGameUI) ? CursorLockMode.Locked : CursorLockMode.None;
    }

    void SwapCamera()
    {
        mainMenuCamera.SetActive(!mainMenuCamera.activeSelf);
        playerCamera.SetActive(!playerCamera.activeSelf);
    }

    public void PlayButton()
    {
        SwitchUI(inGameUI);
        SwapCamera();
    }

    public void SettingsButton()
    {
        lastUI = currentUI;
        SwitchUI(settingsUI);
    }

    public void QuitButton()
    {
        Application.Quit();
    }

    public void BackToMainMenu()
    {
        SwitchUI(mainMenuUI);
        SwapCamera();
    }

    public void Reset()
    {
        willReset = true;
    }

    public void BackFromSettings()
    {
        SwitchUI(lastUI);
    }

    public void BackToGame()
    {
        SwitchUI(inGameUI);
    }

    private void LateUpdate()
    {
        if (willReset)
        {
            SaveManager.LoadGame(playerObject);
            willReset = false;
        }
    }
}
