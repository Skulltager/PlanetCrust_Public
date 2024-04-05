using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SheetCodes;

public class PauseMenuUI : MonoBehaviour
{ 
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private Button mainMenuButton = default;
    [SerializeField] private Button loadLevel1Button = default;
    [SerializeField] private Button loadLevel2Button = default;
    [SerializeField] private Button closeButton = default;
    [SerializeField] private Button exitGameButton = default;

    private bool visible;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(OnPress_MainMenu);
        loadLevel1Button.onClick.AddListener(OnPress_LoadLevel1);
        loadLevel2Button.onClick.AddListener(OnPress_LoadLevel2);
        closeButton.onClick.AddListener(OnPress_Close);
        exitGameButton.onClick.AddListener(OnPress_ExitGame);

        canvasGroup.Hide();
    }

    public void ResetUI()
    {
        visible = false;
        canvasGroup.Hide();
    }

    private void Show()
    {
        canvasGroup.Show();
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        visible = true;
    }

    private void Hide()
    {
        canvasGroup.Hide();
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        visible = false;
    }

    private void OnPress_LoadLevel1()
    {
        SceneLoader.instance.LoadGameScene(SceneIdentifier.Level1);
    }

    private void OnPress_LoadLevel2()
    {
        SceneLoader.instance.LoadGameScene(SceneIdentifier.Level2);
    }

    private void OnPress_MainMenu()
    {
        SceneLoader.instance.LoadMainMenu();
    }

    private void OnPress_Close()
    {
        Hide();
    }

    private void OnPress_ExitGame()
    {
        Application.Quit();
    }

    private void Update()
    {
        if (SceneLoader.instance.isLoading)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (visible)
                Hide();
            else
                Show();
        }
    }

    private void OnDestroy()
    {
        mainMenuButton.onClick.RemoveListener(OnPress_MainMenu);
        loadLevel1Button.onClick.RemoveListener(OnPress_LoadLevel1);
        loadLevel2Button.onClick.RemoveListener(OnPress_LoadLevel2);
        closeButton.onClick.RemoveListener(OnPress_Close);
        exitGameButton.onClick.RemoveListener(OnPress_ExitGame);
    }
}
