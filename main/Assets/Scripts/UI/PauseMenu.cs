using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public GameObject hud;
    public GameObject buttonsGroup;
    public GameObject settingsMenu;
    public GameObject menu;

    private PlayerControls controls;

    private bool isPaused = false;

    void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Menu.performed += ctx => Menu();
    }

    void Start()
    {
        isPaused = false;
        menu.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(false);
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Menu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (hud != null) hud.SetActive(false);
        menu.SetActive(true);
        if (buttonsGroup != null) buttonsGroup.SetActive(true);
        if (settingsMenu != null) settingsMenu.SetActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (hud != null) hud.SetActive(true);
        menu.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OpenSettings()
    {
        if (buttonsGroup != null) buttonsGroup.SetActive(false);
        if (settingsMenu != null) settingsMenu.SetActive(true);
    }

    public void BackFromSettings()
    {
        if (buttonsGroup != null) buttonsGroup.SetActive(true);
        if (settingsMenu != null) settingsMenu.SetActive(false);
    }

    public void BackToMenu()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (hud != null) Destroy(hud);
        Destroy(gameObject);
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
