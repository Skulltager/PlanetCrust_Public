
using SheetCodes;
using UnityEngine;

public class WaitingForLobbyEnteredOverlay : MonoBehaviour
{
    private string password;

    private void OnEnable()
    {
        NetworkManager.instance.onConnectedToRoom += OnEvent_ConnectedToRoom;
        NetworkManager.instance.onFailedToConnectToRoom += OnEvent_FailedToConnectToRoom;
        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
    }

    public void Initialize(string password)
    {
        this.password = password;
    }

    private void OnEvent_ConnectedToRoom()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForLobbyEntered();
        MainMenuNavigation.instance.ShowScreen_LobbyClient();

        if (!string.IsNullOrEmpty(password))
        {
            MainMenuNavigation.instance.ShowOverlay_WaitingForPasswordConfirmation();
            NetworkManager.instance.SendMessage_LobbyPassword(password);
        }
    }

    private void OnEvent_FailedToConnectToRoom(short returncode)
    {
        switch(returncode)
        {
            case 32765:
                PopupManager.instance.QueueMessage(PopupMessageIdentifier.ServerFull);
                break;
            default:
                PopupManager.instance.QueueMessage(PopupMessageIdentifier.HostClosedLobby);
                break;
        }
        MainMenuNavigation.instance.HideOverlay_WaitingForLobbyEntered();
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForLobbyEntered();
    }

    private void OnDisable()
    {
        NetworkManager.instance.onConnectedToRoom -= OnEvent_ConnectedToRoom;
        NetworkManager.instance.onFailedToConnectToRoom -= OnEvent_FailedToConnectToRoom;
        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }
}