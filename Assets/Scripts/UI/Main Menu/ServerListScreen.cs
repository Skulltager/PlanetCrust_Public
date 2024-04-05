
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using SheetCodes;

public class ServerListScreen : MonoBehaviour
{
    [SerializeField] private InputField playerNameField = default;
    [SerializeField] private RectTransform serverListItemsContainer = default;
    [SerializeField] private ServerListItem serverListItemPrefab = default;
    [SerializeField] private Button hostButton = default;
    [SerializeField] private Button joinButton = default;
    [SerializeField] private Button backButton = default;

    private readonly List<ServerListItem> serverListItemInstances;

    private ServerListScreen()
    {
        serverListItemInstances = new List<ServerListItem>();
    }

    private void Awake()
    {
        hostButton.onClick.AddListener(OnPress_HostButton);
        joinButton.onClick.AddListener(OnPress_JoinButton);
        backButton.onClick.AddListener(OnPress_BackButton);
        playerNameField.onValueChanged.AddListener(OnValueChanged_PlayerNameField);
        ServerListItem.selectedServerListItem.onValueChange += OnValueChanged_SelectedListItem;

        CheckHostButton();
        CheckJoinButton();

        NetworkManager.instance.localPlayer.onValueChangeImmediate += OnValueChanged_LocalPlayer;
    }

    private void OnEnable()
    {
        NetworkManager.instance.rooms.onAdd += OnAdd_ServerRoom;
        NetworkManager.instance.rooms.onRemove += OnRemove_ServerRoom;
        foreach (RoomInfoLink roomInfo in NetworkManager.instance.rooms)
            OnAdd_ServerRoom(roomInfo);

        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
    }

    private void OnValueChanged_PlayerName(string oldValue, string newValue)
    {
        playerNameField.text = newValue;
    }

    private void OnAdd_ServerRoom(RoomInfoLink roomInfo)
    {
        ServerListItem instance = GameObject.Instantiate(serverListItemPrefab, serverListItemsContainer);
        instance.data = roomInfo;
        serverListItemInstances.Add(instance);
    }

    private void OnRemove_ServerRoom(RoomInfoLink roomInfo)
    {
        ServerListItem item = serverListItemInstances.Find(i => i.data == roomInfo);
        serverListItemInstances.Remove(item);
        GameObject.Destroy(item.gameObject);
    }

    private void OnValueChanged_PlayerNameField(string value)
    {
        NetworkManager.instance.localPlayer.value.Set_PlayerName(value);
        CheckHostButton();
        CheckJoinButton();
    }

    private void CheckHostButton()
    {
        bool nameFilledIn = !string.IsNullOrEmpty(playerNameField.text);
        hostButton.interactable = nameFilledIn;
    }

    private void CheckJoinButton()
    {
        bool nameFilledIn = !string.IsNullOrEmpty(playerNameField.text);
        joinButton.interactable = nameFilledIn && ServerListItem.selectedServerListItem.value != null;
    }

    private void OnValueChanged_SelectedListItem(ServerListItem oldValue, ServerListItem newValue)
    {
        CheckJoinButton();
    }

    private void OnPress_HostButton()
    {
        MainMenuNavigation.instance.ShowScreen_CreateGame();
    }

    private void OnPress_JoinButton()
    {
        bool requirePassword = (bool) ServerListItem.selectedServerListItem.value.data.roomInfo.value.CustomProperties[NetworkManager.ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD];
        if (requirePassword)
        {
            MainMenuNavigation.instance.ShowOverlay_EnterPassword(ServerListItem.selectedServerListItem.value.data);
            return;
        }

        MainMenuNavigation.instance.ShowOverlay_WaitingForLobbyEntered();
        NetworkManager.instance.JoinRoom(ServerListItem.selectedServerListItem.value.data.roomInfo.value);
    }

    private void OnPress_BackButton()
    {
        NetworkManager.instance.DisconnectFromMaster();
        MainMenuNavigation.instance.ShowScreen_StartMenu();
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.DisconnectedFromServer);
        MainMenuNavigation.instance.ShowScreen_StartMenu();
    }

    private void OnDisable()
    {
        NetworkManager.instance.rooms.onAdd -= OnAdd_ServerRoom;
        NetworkManager.instance.rooms.onRemove -= OnRemove_ServerRoom;
        foreach (RoomInfoLink roomInfo in NetworkManager.instance.rooms)
            OnRemove_ServerRoom(roomInfo);

        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }

    private void OnValueChanged_LocalPlayer(PlayerData oldValue, PlayerData newValue)
    {
        if(oldValue != null)
        {
            oldValue.playerName.onValueChange -= OnValueChanged_PlayerName;
        }
        if(newValue != null)
        {
            newValue.playerName.onValueChangeImmediate += OnValueChanged_PlayerName;
        }
    }

    private void OnDestroy()
    {
        hostButton.onClick.RemoveListener(OnPress_HostButton);
        joinButton.onClick.RemoveListener(OnPress_JoinButton);
        backButton.onClick.RemoveListener(OnPress_BackButton);
        playerNameField.onValueChanged.RemoveListener(OnValueChanged_PlayerNameField);
        ServerListItem.selectedServerListItem.onValueChange -= OnValueChanged_SelectedListItem;
        NetworkManager.instance.localPlayer.onValueChangeImmediate -= OnValueChanged_LocalPlayer;
    }
}
