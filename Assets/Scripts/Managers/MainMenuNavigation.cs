using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuNavigation : MonoBehaviour
{
    public static MainMenuNavigation instance { private set; get; }
    private readonly EventVariable<MainMenuNavigation, GameObject> selectedScreen;

    [SerializeField] private StartMenu startMenuScreen = default;
    [SerializeField] private ServerListScreen serverListScreen = default;
    [SerializeField] private HostGameScreen hostGameScreen = default;
    [SerializeField] private LobbyHostScreen lobbyHostScreen = default;
    [SerializeField] private LobbyClientScreen lobbyClientScreen = default;

    [SerializeField] private EnterPasswordOverlay enterPasswordOverlay = default;
    [SerializeField] private WaitingForLobbyEnteredOverlay waitingForLobbyEnteredOverlay = default;
    [SerializeField] private WaitingForPasswordConfirmationOverlay waitingForPasswordConfirmationOverlay = default;
    [SerializeField] private WaitingForMasterConnectionOverlay waitingForMasterConnectionOverlay = default;

    private MainMenuNavigation()
    {
        selectedScreen = new EventVariable<MainMenuNavigation, GameObject>(this, null);
    }

    private void Awake()
    {
        instance = this;
        selectedScreen.onValueChange += OnValueChanged_SelectedScreen;

        startMenuScreen.gameObject.SetActive(false);
        serverListScreen.gameObject.SetActive(false);
        hostGameScreen.gameObject.SetActive(false);
        lobbyHostScreen.gameObject.SetActive(false);
        lobbyClientScreen.gameObject.SetActive(false);

        enterPasswordOverlay.gameObject.SetActive(false);
        waitingForLobbyEnteredOverlay.gameObject.SetActive(false);
        waitingForPasswordConfirmationOverlay.gameObject.SetActive(false);

        ShowScreen_StartMenu();
    }

    private void OnValueChanged_SelectedScreen(GameObject oldValue, GameObject newValue)
    {
        if (oldValue != null)
            oldValue.SetActive(false);

        if (newValue != null)
            newValue.SetActive(true);
    }

    public void ShowScreen_StartMenu()
    {
        selectedScreen.value = startMenuScreen.gameObject;
    }

    public void ShowScreen_ServerList()
    {
        selectedScreen.value = serverListScreen.gameObject;
    }

    public void ShowScreen_CreateGame()
    {
        selectedScreen.value = hostGameScreen.gameObject;
    }

    public void ShowScreen_LobbyHost()
    {
        selectedScreen.value = lobbyHostScreen.gameObject;
    }

    public void ShowScreen_LobbyClient()
    {
        selectedScreen.value = lobbyClientScreen.gameObject;
    }

    public void ShowOverlay_EnterPassword(RoomInfoLink roomInfoLink)
    {
        enterPasswordOverlay.Initialize(roomInfoLink);
        enterPasswordOverlay.gameObject.SetActive(true);
    }

    public void HideOverlay_EnterPassword()
    {
        enterPasswordOverlay.gameObject.SetActive(false);
    }

    public void ShowOverlay_WaitingForLobbyEntered()
    {
        waitingForLobbyEnteredOverlay.Initialize(string.Empty);
        waitingForLobbyEnteredOverlay.gameObject.SetActive(true);
    }

    public void ShowOverlay_WaitingForLobbyEntered(string password)
    {
        waitingForLobbyEnteredOverlay.Initialize(password);
        waitingForLobbyEnteredOverlay.gameObject.SetActive(true);
    }

    public void HideOverlay_WaitingForLobbyEntered()
    {
        waitingForLobbyEnteredOverlay.gameObject.SetActive(false);
    }

    public void ShowOverlay_WaitingForPasswordConfirmation()
    {
        waitingForPasswordConfirmationOverlay.gameObject.SetActive(true);
    }

    public void HideOverlay_WaitingForPasswordConfirmation()
    {
        waitingForPasswordConfirmationOverlay.gameObject.SetActive(false);
    }

    public void ShowOverlay_WaitingForMasterConnection()
    {
        waitingForMasterConnectionOverlay.gameObject.SetActive(true);
    }

    public void HideOverlay_WaitingForMasterConnection()
    {
        waitingForMasterConnectionOverlay.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        selectedScreen.onValueChange -= OnValueChanged_SelectedScreen;
    }
}