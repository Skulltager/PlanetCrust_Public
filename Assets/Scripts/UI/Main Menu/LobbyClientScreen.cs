
using Photon.Realtime;
using SheetCodes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyClientScreen : MonoBehaviour
{
    [SerializeField] private Button backButton = default;
    [SerializeField] private ConnectedPlayerRowClient connectedPlayerRowPrefab = default;
    [SerializeField] private Transform connectedPlayerRowsContainer = default;

    private readonly List<ConnectedPlayerRowClient> connectedPlayerRowInstances = default;

    private LobbyClientScreen()
    {
        connectedPlayerRowInstances = new List<ConnectedPlayerRowClient>();
    }

    private void Awake()
    {
        backButton.onClick.AddListener(OnPress_BackButton);
    }

    private void OnEnable()
    {
        NetworkManager.instance.connectedPlayers.onAdd += OnAdd_ConnectedPlayers;
        NetworkManager.instance.connectedPlayers.onRemove += OnRemove_ConnectedPlayers;
        foreach (PlayerData playerData in NetworkManager.instance.connectedPlayers)
            OnAdd_ConnectedPlayers(playerData);

        NetworkManager.instance.onNetworkMessage_Kicked += OnNetworkMessage_Kicked;
        NetworkManager.instance.onNetworkMessage_LobbyClosed += OnNetworkMessage_LobbyClosed;
        NetworkManager.instance.onDisconnectedFromRoom += OnEvent_DisconnectedFromRoom;
        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
    }
    
    private void OnNetworkMessage_Kicked()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.KickedFromLobby);
    }

    private void OnNetworkMessage_LobbyClosed()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.HostClosedLobby);
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        MainMenuNavigation.instance.ShowScreen_StartMenu();
    }

    private void OnEvent_DisconnectedFromRoom()
    {
        MainMenuNavigation.instance.ShowScreen_ServerList();
    }

    private void OnAdd_ConnectedPlayers(PlayerData playerData)
    {
        ConnectedPlayerRowClient instance = GameObject.Instantiate(connectedPlayerRowPrefab, connectedPlayerRowsContainer);
        instance.data = playerData;
        connectedPlayerRowInstances.Add(instance);
    }

    private void OnRemove_ConnectedPlayers(PlayerData playerData)
    {
        ConnectedPlayerRowClient instance = connectedPlayerRowInstances.Find(i => i.data == playerData);
        connectedPlayerRowInstances.Remove(instance);
        GameObject.Destroy(instance.gameObject);
    }

    private void OnPress_BackButton()
    {
        NetworkManager.instance.LeaveRoom();
        MainMenuNavigation.instance.ShowScreen_ServerList();
    }

    private void OnDisable()
    {
        NetworkManager.instance.connectedPlayers.onAdd -= OnAdd_ConnectedPlayers;
        NetworkManager.instance.connectedPlayers.onRemove -= OnRemove_ConnectedPlayers;
        foreach (PlayerData playerData in NetworkManager.instance.connectedPlayers)
            OnRemove_ConnectedPlayers(playerData);

        NetworkManager.instance.onNetworkMessage_Kicked -= OnNetworkMessage_Kicked;
        NetworkManager.instance.onNetworkMessage_LobbyClosed -= OnNetworkMessage_LobbyClosed;
        NetworkManager.instance.onDisconnectedFromRoom -= OnEvent_DisconnectedFromRoom;
        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }

    private void OnDestroy()
    {
        backButton.onClick.RemoveListener(OnPress_BackButton);
    }
}