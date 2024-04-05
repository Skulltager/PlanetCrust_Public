
using Photon.Realtime;
using SheetCodes;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyHostScreen : MonoBehaviour
{
    [SerializeField] private Button startButton = default;
    [SerializeField] private Button backButton = default;
    [SerializeField] private ConnectedPlayerRowHost connectedPlayerRowPrefab = default;
    [SerializeField] private Transform connectedPlayerRowsContainer = default;

    private readonly List<ConnectedPlayerRowHost> connectedPlayerRowInstances = default;

    private LobbyHostScreen()
    {
        connectedPlayerRowInstances = new List<ConnectedPlayerRowHost>();
    }

    private void Awake()
    {
        startButton.onClick.AddListener(OnPress_StartButton);
        backButton.onClick.AddListener(OnPress_BackButton);
    }

    private void OnEnable()
    { 
        NetworkManager.instance.connectedPlayers.onAdd += OnAdd_ConnectedPlayers;
        NetworkManager.instance.connectedPlayers.onRemove += OnRemove_ConnectedPlayers;
        foreach (PlayerData playerData in NetworkManager.instance.connectedPlayers)
            OnAdd_ConnectedPlayers(playerData);

        NetworkManager.instance.onDisconnectedFromRoom += OnEvent_DisconnectedFromRoom;
        NetworkManager.instance.onDisconnectedFromMaster += OnEvent_DisconnectedFromMaster;
    }

    private void OnEvent_DisconnectedFromRoom()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.KickedFromLobby);
        MainMenuNavigation.instance.ShowScreen_ServerList();
    }

    private void OnEvent_DisconnectedFromMaster()
    {
        PopupManager.instance.QueueMessage(PopupMessageIdentifier.DisconnectedFromServer);
        MainMenuNavigation.instance.ShowScreen_StartMenu();
    }

    private void OnAdd_ConnectedPlayers(PlayerData playerData)
    {
        ConnectedPlayerRowHost instance = GameObject.Instantiate(connectedPlayerRowPrefab, connectedPlayerRowsContainer);
        instance.data = playerData;
        connectedPlayerRowInstances.Add(instance);
        instance.onKickButtonPressed += OnPress_KickButton;
    }

    private void OnPress_KickButton(ConnectedPlayerRowHost source)
    {
        NetworkManager.instance.KickPlayer(source.data);
    }

    private void OnRemove_ConnectedPlayers(PlayerData playerData)
    {
        ConnectedPlayerRowHost instance = connectedPlayerRowInstances.Find(i => i.data == playerData);
        connectedPlayerRowInstances.Remove(instance);
        instance.onKickButtonPressed -= OnPress_KickButton;
        GameObject.Destroy(instance.gameObject);
    }

    private void OnPress_StartButton()
    {

    }

    private void OnPress_BackButton()
    {
        NetworkManager.instance.CloseRoom();
        MainMenuNavigation.instance.ShowScreen_ServerList();
    }

    private void OnDisable()
    {
        NetworkManager.instance.connectedPlayers.onAdd -= OnAdd_ConnectedPlayers;
        NetworkManager.instance.connectedPlayers.onRemove -= OnRemove_ConnectedPlayers;
        foreach (PlayerData playerData in NetworkManager.instance.connectedPlayers)
            OnRemove_ConnectedPlayers(playerData);

        NetworkManager.instance.onDisconnectedFromRoom -= OnEvent_DisconnectedFromRoom;
        NetworkManager.instance.onDisconnectedFromMaster -= OnEvent_DisconnectedFromMaster;
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(OnPress_StartButton);
        backButton.onClick.RemoveListener(OnPress_BackButton);
    }
}