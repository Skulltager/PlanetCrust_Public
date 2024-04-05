
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class EnterPasswordOverlay : MonoBehaviour
{
    [SerializeField] private InputField passwordField = default;
    [SerializeField] private Button backButton = default;
    [SerializeField] private Button enterButton = default;

    private RoomInfoLink roomInfoLink;

    private void Awake()
    {
        backButton.onClick.AddListener(OnPress_BackButton);
        enterButton.onClick.AddListener(OnPress_EnterButton);
        passwordField.onValueChanged.AddListener(OnValueChanged_PasswordField);
    }

    private void OnEnable()
    {
        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
        passwordField.text = string.Empty;
    }

    public void Initialize(RoomInfoLink roomInfoLink)
    {
        this.roomInfoLink = roomInfoLink;
    }

    private void OnValueChanged_PasswordField(string text)
    {
        enterButton.interactable = !string.IsNullOrEmpty(text);
    }

    private void OnPress_BackButton()
    {
        MainMenuNavigation.instance.HideOverlay_EnterPassword();
    }

    private void OnPress_EnterButton()
    {
        MainMenuNavigation.instance.HideOverlay_EnterPassword();
        MainMenuNavigation.instance.ShowOverlay_WaitingForLobbyEntered(passwordField.text);
        NetworkManager.instance.JoinRoom(ServerListItem.selectedServerListItem.value.data.roomInfo.value);
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        MainMenuNavigation.instance.HideOverlay_WaitingForLobbyEntered();
    }

    private void OnDisable()
    {
        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnPress_BackButton);
        enterButton.onClick.RemoveListener(OnPress_EnterButton);
    }
}