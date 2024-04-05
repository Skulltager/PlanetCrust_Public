
using SheetCodes;
using UnityEngine;

public class WaitingForMasterConnectionOverlay : MonoBehaviour
{
    private void OnEnable()
    {
        NetworkManager.instance.onConnectedToMaster += OnEvent_ConnectedToMaster;
        NetworkManager.instance.onFailedToConnectToMaster += OnEvent_OnFailedToConnectToMaster;
    }

    private void OnEvent_ConnectedToMaster()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForMasterConnection();
    }

    private void OnEvent_OnFailedToConnectToMaster()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForMasterConnection();
        MainMenuNavigation.instance.ShowScreen_StartMenu();
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.FailedToConnectToMaster);

    }

    private void OnDisable()
    {
        NetworkManager.instance.onConnectedToMaster -= OnEvent_ConnectedToMaster;
        NetworkManager.instance.onFailedToConnectToMaster -= OnEvent_OnFailedToConnectToMaster;
    }
}