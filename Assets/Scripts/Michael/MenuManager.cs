﻿/*
    File name: MenuManager.cs
    Author: Michael Sweetman
    Summary: Manages events triggered by clicking UI buttons such as switching between UIs and exiting the game.
    Creation Date: 29/07/2020
    Last Modified: 15/09/2020
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Cameras")]
    [SerializeField] GameObject mainMenuCamera;
    [SerializeField] GameObject playerCamera;

    [Header("UIs")]
    [SerializeField] GameObject mainMenuUI;
    [SerializeField] GameObject gameMenuUI;
    [SerializeField] GameObject inGameUI;
    [SerializeField] GameObject settingsUI;

    [Header("Player")]
    [SerializeField] GameObject player;
    [SerializeField] MouseLook mouseLook;

    [Header("Settings")]
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Text masterVolumeValueText;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Text musicVolumeValueText;
    [SerializeField] Slider dialogueVolumeSlider;
    [SerializeField] Text dialogueVolumeValueText;
    [SerializeField] Slider soundEffectVolumeSlider;
    [SerializeField] Text soundEffectVolumeValueText;
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] Text sensitivityValueText;
    [SerializeField] Slider fieldOfViewSlider;
    [SerializeField] Text fieldOfViewValueText;
    [SerializeField] Toggle fullScreenToggle;

    [Header("Start State")]
    [SerializeField] bool startInMainMenu = true;

    PlayerMovement playerMovement;
    Tool tool;

    GameObject currentUI;
    GameObject lastUI;
    GameObject[] UIs;
    bool willGoBackToCheckpoint = false;

    [HideInInspector] public float masterVolume = 10.0f;
    [HideInInspector] public float musicVolume = 10.0f;
    [HideInInspector] public float dialogueVolume = 10.0f;
    [HideInInspector] public float soundEffectVolume = 10.0f;

    void Start()
    {
        // get the Player Movement and Tool script from the player
        playerMovement = player.GetComponent<PlayerMovement>();
        tool = player.GetComponent<Tool>();
        
        // activate the camera needed for the start UI, deactivate the other
        mainMenuCamera.SetActive(startInMainMenu);
        playerCamera.SetActive(!startInMainMenu);

        // store the UIs in an array
        UIs = new GameObject[4] { mainMenuUI, gameMenuUI, inGameUI, settingsUI };

        // if the start UI should be the main menu
        if (startInMainMenu)
        {
            // switch to the main menu UI
            SwitchUI(mainMenuUI);
        }
        // if the start UI should instead by the in game UI
        else
        {
            // switch to the in game UI
            SwitchUI(inGameUI);
        }

        // trigger the settings functions to get the inital values from the sliders 
        MasterVolume();
        MusicVolume();
        DialogueVolume();
        SoundEffectVolume();
        Sensitivity();
        FieldOfView();
    }

    private void Update()
    {
        // if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // if the current UI is the in-game menu, switch to the in-game UI
            if (currentUI == gameMenuUI)
            {
                SwitchUI(inGameUI);
            }
            // if the current UI is the in-game UI, switch to the in-game menu
            else if (currentUI == inGameUI)
            {
                // Disable artifact viewer when pausing, but do not enable movement scripts
                player.GetComponent<PlayerInteract>().DisableArtifactViewer(false);

                SwitchUI(gameMenuUI);
            }
            // if the current UI is the settings
            else if (currentUI == settingsUI)
            {
                // if the last UI was the in-game menu, switch to the in-game UI
                if (lastUI == gameMenuUI)
                {
                    SwitchUI(inGameUI);
                }
                // if the last UI was the main menu, switch to the main menu UI
                else if (lastUI == mainMenuUI)
                {
                    SwitchUI(mainMenuUI);
                }
            }
        }
    }

    // sets player controls to be active or inactive as per the argument value
    void TogglePlayerControls(bool enablePlayerControls)
    {
        // change the cursor lockstate
        Cursor.lockState = (enablePlayerControls) ? CursorLockMode.Locked : CursorLockMode.None;

        // toggle player controls
        mouseLook.enabled = enablePlayerControls;
        playerMovement.enabled = enablePlayerControls;
        tool.enabled = enablePlayerControls;
    }

    // switches the canvas UI to the desired UI
    void SwitchUI(GameObject newUI)
    {
        // set each UI to be inactive
        foreach (GameObject ui in UIs)
        {
            ui.SetActive(false);
        }
        // set the desired UI to be active
        newUI.SetActive(true);
        // store that this UI is the current UI
        currentUI = newUI;

        // enable player controls if the current UI is the in Game UI, set them to false otherwise
        TogglePlayerControls((currentUI == inGameUI));
    }

    // toggles which camera is active
    void SwapCamera()
    {
        // toggle the main menu and player camera's activity
        mainMenuCamera.SetActive(!mainMenuCamera.activeSelf);
        playerCamera.SetActive(!playerCamera.activeSelf);
    }

    // triggers when the play button is pressed in the main menu
    public void PlayButton()
    {
        // switch to the in-game UI
        SwitchUI(inGameUI);
        // swap cameras
        SwapCamera();
    }

    // triggers when the settings button is pressed in the main menu or in-game menu
    public void SettingsButton()
    {
        // store the current UI as the last UI
        lastUI = currentUI;
        // switch to the settings UI
        SwitchUI(settingsUI);
    }

    // triggers when the quit button is pressed in the main menu or in-game menu
    public void QuitButton()
    {
        // quit the application
        Application.Quit();
    }

    // triggers when the back to main menu button is pressed in the in-game menu
    public void BackToMainMenu()
    {
        // switch to the main menu UI
        SwitchUI(mainMenuUI);
        // swap cameras
        SwapCamera();
    }

    // triggers when the back to checkpoint button is pressed in the in-game menu
    public void BackToCheckpoint()
    {
        // store that the player needs to return to their previous checkpoint at the end of this frame
        willGoBackToCheckpoint = true;
    }

    // triggers when the back button is pressed in settings
    public void BackFromSettings()
    {
        // switch to the previous UI
        SwitchUI(lastUI);
    }

    // triggers when the back to game button is pressed in the in-game menu
    public void BackToGame()
    {
        // switch to the in-game UI
        SwitchUI(inGameUI);
    }

    // triggers when the value of the master volume slider changes
    public void MasterVolume()
    {
        // update the master volume value text to display the new master volume value
        masterVolumeValueText.text = masterVolumeSlider.value.ToString();
        // store the new master volume value
        masterVolume = masterVolumeSlider.value;
    }

    // triggers when the value of the music volume slider changes
    public void MusicVolume()
    {
        // update the music volume value text to display the new music volume value
        musicVolumeValueText.text = musicVolumeSlider.value.ToString();
        // store the new music volume value
        musicVolume = musicVolumeSlider.value;
    }

    // triggers when the value of the dialogue volume slider changes
    public void DialogueVolume()
    {
        // update the dialogue volume value text to display the new dialogue volume value
        dialogueVolumeValueText.text = dialogueVolumeSlider.value.ToString();
        // store the new dialogue volume value
        dialogueVolume = dialogueVolumeSlider.value;
    }

    // triggers when the value of the sound effect volume slider changes
    public void SoundEffectVolume()
    {
        // update the sound effect volume value text to display the new sound effect volume value
        soundEffectVolumeValueText.text = soundEffectVolumeSlider.value.ToString();
        // store the new sound effect volume value
        soundEffectVolume = soundEffectVolumeSlider.value;
    }

    // triggers when the value of the sensitivity slider changes
    public void Sensitivity()
    {
        // update the sensitivity value text to display the new sensitivity value
        sensitivityValueText.text = sensitivitySlider.value.ToString("F1");
        // set the mouse sensitivity to the value of the slider
        mouseLook.mouseSensitivity = sensitivitySlider.value;
    }

    // triggers when the value of the field of view slider changes
    public void FieldOfView()
    {
        // update the field of view value text to display the new field of view value
        fieldOfViewValueText.text = fieldOfViewSlider.value.ToString();
        // set the player camera field of view to the value of the slider
        playerCamera.GetComponent<Camera>().fieldOfView = fieldOfViewSlider.value;
    }

    // triggers when the fullscreen toggle is clicked
    public void FullScreen()
    {
        // set the game to play in fullscreen if the toggle is on, windowed otherwise
        Screen.fullScreen = fullScreenToggle.isOn;
    }

    private void LateUpdate()
    {
        // if the back to checkpoint button was pressed this frame
        if (willGoBackToCheckpoint)
        {
            // load the player's last checkpoint
            SaveManager.LoadGame(player);
            // store that the player's last checkpoint no longer needs to be loaded
            willGoBackToCheckpoint = false;
            // switch to the in-game UI
            SwitchUI(inGameUI);
        }
    }
}
