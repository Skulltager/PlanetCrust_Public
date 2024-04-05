
using SheetCodes;
using UnityEngine;

public class WaitingForPasswordConfirmationOverlay : MonoBehaviour
{
    private void OnEnable()
    {
        NetworkManager.instance.onNetworkMessage_CorrectPassword += OnNetworkMessage_CorrectPassword;
        NetworkManager.instance.onNetworkMessage_InCorrectPassword += OnNetworkMessage_InCorrectPassword;
    }

    private void OnNetworkMessage_CorrectPassword()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForPasswordConfirmation();
    }

    private void OnNetworkMessage_InCorrectPassword()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForPasswordConfirmation();
        MainMenuNavigation.instance.ShowScreen_ServerList();

        NetworkManager.instance.LeaveRoom();

        PopupManager.instance.QueueMessage(PopupMessageIdentifier.IncorrectPassword);
    }

    private void OnDisable()
    {
        NetworkManager.instance.onNetworkMessage_CorrectPassword -= OnNetworkMessage_CorrectPassword;
        NetworkManager.instance.onNetworkMessage_InCorrectPassword -= OnNetworkMessage_InCorrectPassword;
    }
}
