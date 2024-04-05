
using SheetCodes;
using UnityEngine;
using UnityEngine.UI;

public class HostGameScreen : MonoBehaviour
{
    private const int minPlayersLimit = 1;
    private const int maxPlayersLimit = 8;

    [SerializeField] private InputField roomNameField = default;
    [SerializeField] private InputField passwordField = default;
    [SerializeField] private InputField maxPlayersField = default;
    [SerializeField] private Button hostButton = default;
    [SerializeField] private Button backButton = default;

    private void Awake()
    {
        roomNameField.onValueChanged.AddListener(OnValueChanged_GameNameField);
        passwordField.onValueChanged.AddListener(OnValueChanged_PasswordField);
        maxPlayersField.onValueChanged.AddListener(OnValueChanged_MaxPlayersField);
        hostButton.onClick.AddListener(OnPress_HostButton);
        backButton.onClick.AddListener(OnPress_BackButton);
    }

    private void OnEnable()
    {
        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.DisconnectedFromServer);
        MainMenuNavigation.instance.ShowScreen_StartMenu();
    }

    private void OnValueChanged_GameNameField(string newValue)
    {
        Check_HostButton();
    }

    private void OnValueChanged_PasswordField(string newValue)
    {

    }

    private void OnValueChanged_MaxPlayersField(string newValue)
    {
        if (!int.TryParse(newValue, out int playerLimit))
            return;

        maxPlayersField.text = Mathf.Clamp(playerLimit, minPlayersLimit, maxPlayersLimit).ToString();
        Check_HostButton();
    }

    private void Check_HostButton()
    {
        if (roomNameField.text.Length == 0)
        {
            hostButton.interactable = false;
            return;
        }

        if (!int.TryParse(maxPlayersField.text, out int playerLimit))
        {
            hostButton.interactable = false;
            return;
        }

        if (playerLimit < minPlayersLimit || playerLimit > maxPlayersLimit)
        {
            hostButton.interactable = false;
            return;
        }

        hostButton.interactable = true;
    }

    private void OnPress_HostButton()
    {
        MainMenuNavigation.instance.ShowScreen_LobbyHost();
        int playerLimit = int.Parse(maxPlayersField.text);
        NetworkManager.instance.CreateRoom(roomNameField.text, playerLimit, passwordField.text);
    }

    private void OnPress_BackButton()
    {
        MainMenuNavigation.instance.ShowScreen_ServerList();
    }

    private void OnDisable()
    {
        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }

    private void OnDestroy()
    {
        roomNameField.onValueChanged.RemoveListener(OnValueChanged_GameNameField);
        passwordField.onValueChanged.RemoveListener(OnValueChanged_PasswordField);
        maxPlayersField.onValueChanged.RemoveListener(OnValueChanged_MaxPlayersField);
        hostButton.onClick.RemoveListener(OnPress_HostButton);
        backButton.onClick.RemoveListener(OnPress_BackButton);
    }
}