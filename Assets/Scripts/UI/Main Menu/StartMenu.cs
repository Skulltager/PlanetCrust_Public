using UnityEngine;
using UnityEngine.UI;
using SheetCodes;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private SceneIdentifier startScene = default;
    [SerializeField] private Button startGameButton = default;
    [SerializeField] private Button multiplayerButton = default;
    [SerializeField] private Button exitGameButton = default;

    private void Awake()
    {
        startGameButton.onClick.AddListener(OnPress_StartGame);
        multiplayerButton.onClick.AddListener(OnPress_Multiplayer);
        exitGameButton.onClick.AddListener(OnPress_ExitGame);
    }

    private void OnPress_StartGame()
    {
        SceneLoader.instance.LoadGameScene(startScene);
    } 

    private void OnPress_Multiplayer()
    {
        MainMenuNavigation.instance.ShowScreen_ServerList();
        MainMenuNavigation.instance.ShowOverlay_WaitingForMasterConnection();
        NetworkManager.instance.ConnectToMaster();
    }

    private void OnPress_ExitGame()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        startGameButton.onClick.RemoveListener(OnPress_StartGame);
        exitGameButton.onClick.RemoveListener(OnPress_ExitGame);
    }
}
