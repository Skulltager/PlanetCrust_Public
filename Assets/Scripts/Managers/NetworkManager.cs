
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviourPunCallbacks, IConnectionCallbacks
{
    public const string ROOM_CUSTOMDATA_INDEX_PING = "Room Ping";
    public const string ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD = "Room Has Password";

    [SerializeField] private float timeBetweenPlayerPingRefreshes = default;
    [SerializeField] private float timeBetweenRoomPingRefreshes = default;
    [SerializeField] private float timeForPasswordCall = default;

    public static NetworkManager instance { private set; get; }

    public readonly EventVariable<NetworkManager, PlayerData> localPlayer;
    public readonly EventVariable<NetworkManager, bool> connectedToMaster;
    public readonly EventVariable<NetworkManager, Room> connectedRoom;
    public readonly EventVariable<NetworkManager, bool> isRoomHost;
    public readonly EventList<PlayerData> connectedPlayers;
    public readonly EventList<RoomInfoLink> rooms;

    public event Action onNetworkMessage_CorrectPassword;
    public event Action onNetworkMessage_InCorrectPassword;
    public event Action onNetworkMessage_Kicked;
    public event Action onNetworkMessage_LobbyClosed;

    public event Action onConnectedToMaster;
    public event Action onConnectedToRoom;
    public event Action<short> onFailedToConnectToRoom;

    public event Action onDisconnectedFromRoom;
    public event Action onDisconnectedFromMaster;
    public event Action onFailedToConnectToMaster;

    public string roomPassword { private set; get; }

    private float playerPingRefreshTimer;
    private float roomPingRefreshTimer;

    private NetworkManager()
    {
        rooms = new EventList<RoomInfoLink>();
        localPlayer = new EventVariable<NetworkManager, PlayerData>(this, null);
        isRoomHost = new EventVariable<NetworkManager, bool>(this, false);
        connectedRoom = new EventVariable<NetworkManager, Room>(this, null);
        connectedPlayers = new EventList<PlayerData>();
        connectedToMaster = new EventVariable<NetworkManager, bool>(this, false);
    }

    private void Awake()
    {
        instance = this;
        PhotonNetwork.AddCallbackTarget(this);
        PhotonNetwork.EnableCloseConnection = true;
    }

    private void Update()
    {
        UpdatePlayerPing();
        UpdateRoomPing();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (onFailedToConnectToRoom != null)
            onFailedToConnectToRoom(returnCode);
    }

    public void DisconnectFromMaster()
    {
        PhotonNetwork.Disconnect();
    }

    public void ConnectToMaster()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        connectedPlayers.Find(i => i.player == targetPlayer).PlayerDataChanged(changedProps);
    }

    private void UpdatePlayerPing()
    {
        if (!connectedToMaster.value)
            return;

        if (playerPingRefreshTimer > Time.realtimeSinceStartup)
            return;

        localPlayer.value.Set_Ping(PhotonNetwork.GetPing());
        playerPingRefreshTimer = Time.realtimeSinceStartup + timeBetweenPlayerPingRefreshes;
    }

    private void UpdateRoomPing()
    {
        if (connectedRoom.value == null)
            return;

        if (!isRoomHost.value)
            return;

        if (roomPingRefreshTimer > Time.realtimeSinceStartup)
            return;

        ExitGames.Client.Photon.Hashtable hashTable = new ExitGames.Client.Photon.Hashtable();
        hashTable.Add(ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD, !string.IsNullOrEmpty(roomPassword));
        hashTable.Add(ROOM_CUSTOMDATA_INDEX_PING, PhotonNetwork.GetPing());
        connectedRoom.value.SetCustomProperties(hashTable);
        roomPingRefreshTimer = Time.realtimeSinceStartup + timeBetweenRoomPingRefreshes;
    }

    public void CreateRoom(string roomName, int playerLimit, string password)
    {
        roomPassword = password;
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = (byte)playerLimit;

        ExitGames.Client.Photon.Hashtable hashTable = new ExitGames.Client.Photon.Hashtable();
        hashTable.Add(ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD, !string.IsNullOrEmpty(password));
        hashTable.Add(ROOM_CUSTOMDATA_INDEX_PING, PhotonNetwork.GetPing());
        options.CustomRoomProperties = hashTable;

        bool hasPassword = !string.IsNullOrEmpty(password);
        options.CustomRoomPropertiesForLobby = new string[] { ROOM_CUSTOMDATA_INDEX_PING, ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD };

        PhotonNetwork.CreateRoom(roomName, options);
    }

    public void CloseRoom()
    {
        SendMessage_LobbyClosed();
        connectedRoom.value.IsOpen = false;
        foreach (Player player in connectedRoom.value.Players.Values)
        {
            if (player.IsLocal)
                continue;

            PhotonNetwork.CloseConnection(player);
        }

        LeaveRoom();
    }

    public void KickPlayer(PlayerData playerData)
    {
        SendMessage_PlayerKicked(playerData);
        PhotonNetwork.CloseConnection(playerData.player);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom(false);
    }

    public void JoinRoom(RoomInfo roomInfo)
    {
        localPlayer.value.Set_PasswordVerified(false);
        PhotonNetwork.JoinRoom(roomInfo.Name);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("created room");
        isRoomHost.value = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined room");

        if (onConnectedToRoom != null)
            onConnectedToRoom();

        rooms.Clear();
        connectedRoom.value = PhotonNetwork.CurrentRoom;

        if((bool) connectedRoom.value.CustomProperties[ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD])
        {
            if (localPlayer.value.player.IsMasterClient)
                localPlayer.value.Set_PasswordVerified(true);
            else
            localPlayer.value.Set_PasswordVerified(false);
        }
        else
            localPlayer.value.Set_PasswordVerified(true);

        foreach (Player player in PhotonNetwork.PlayerList)
            connectedPlayers.Add(new PlayerData(player));
    }

    public override void OnLeftRoom()
    {
        if (onDisconnectedFromRoom != null)
            onDisconnectedFromRoom();

        Debug.Log("left room");
        connectedRoom.value = null;
        isRoomHost.value = false;
        roomPassword = string.Empty;
        connectedPlayers.Clear();
    }

    #region RPC Messages
    public void SendMessage_LobbyPassword(string password)
    {
        StartCoroutine(Routine_SendMessage_LobbyPassword(password));
    }

    private IEnumerator Routine_SendMessage_LobbyPassword(string password)
    {
        float timeLimit = Time.realtimeSinceStartup + timeForPasswordCall;
        PlayerData playerData = connectedPlayers.Find(i => i.player.IsMasterClient);
        while(playerData == null)
        {
            if(timeLimit < Time.realtimeSinceStartup)
                yield break;

            yield return null;
            playerData = connectedPlayers.Find(i => i.player.IsMasterClient);
        }

        Debug.Log("Send: " + password);
        photonView.RPC("ReceiveMessage_LobbyPassword", playerData.player, password);
    }

    [PunRPC]
    private void ReceiveMessage_LobbyPassword(string password, PhotonMessageInfo info)
    {
        Debug.Log("Receive: " + password);
        PlayerData playerData = connectedPlayers.Find(i => i.player == info.Sender);

        if (roomPassword != password)
        {
            SendMessage_IncorrectPassword(playerData);
            PhotonNetwork.CloseConnection(info.Sender);
            return;
        }

        SendMessage_CorrectPassword(playerData);
        playerData.Set_PasswordVerified(true);
    }

    public void SendMessage_LobbyClosed()
    {
        foreach (PlayerData playerData in connectedPlayers)
        {
            if (playerData.player.IsLocal)
                continue;

            photonView.RPC("ReceiveMessage_LobbyClosed", playerData.player);
        }
    }

    [PunRPC]
    private void ReceiveMessage_LobbyClosed()
    {
        if (onNetworkMessage_LobbyClosed != null)
            onNetworkMessage_LobbyClosed();
    }

    public void SendMessage_PlayerKicked(PlayerData playerData)
    {
        photonView.RPC("ReceiveMessage_PlayerKicked", playerData.player);
    }

    [PunRPC]
    private void ReceiveMessage_PlayerKicked()
    {
        if (onNetworkMessage_Kicked != null)
            onNetworkMessage_Kicked();
    }

    public void SendMessage_CorrectPassword(PlayerData playerData)
    {
        photonView.RPC("ReceiveMessage_CorrectPassword", playerData.player);
    }

    [PunRPC]
    private void ReceiveMessage_CorrectPassword()
    {
        if (onNetworkMessage_CorrectPassword != null)
            onNetworkMessage_CorrectPassword();
    }

    public void SendMessage_IncorrectPassword(PlayerData playerData)
    {
        photonView.RPC("ReceiveMessage_InCorrectPassword", playerData.player);
    }

    [PunRPC]
    private void ReceiveMessage_InCorrectPassword()
    {
        if (onNetworkMessage_InCorrectPassword != null)
            onNetworkMessage_InCorrectPassword();
    }
    #endregion

    public override void OnPlayerEnteredRoom(Player player)
    {
        connectedPlayers.Add(new PlayerData(player));
    }

    public override void OnPlayerLeftRoom(Player player)
    {
        connectedPlayers.RemoveAt(connectedPlayers.FindIndex(i => i.player == player));
    }

    private string[] CreateCustomRoomProperties(bool hasPassword)
    {
        return new string[]
        {
            PhotonNetwork.GetPing().ToString(),
            hasPassword.ToString(),
        };
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected");

        if (onConnectedToMaster != null)
            onConnectedToMaster();

        localPlayer.value = new PlayerData(PhotonNetwork.LocalPlayer);
        connectedToMaster.value = true;

        playerPingRefreshTimer = Time.realtimeSinceStartup;
        UpdatePlayerPing();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room list updated");
        foreach(RoomInfo roomInfo in roomList)
        {
            int index = rooms.FindIndex(i => i.roomInfo.value.Name == roomInfo.Name);

            if (roomInfo.RemovedFromList)
            {
                if(index >= 0)
                    rooms.RemoveAt(index);

                continue;
            }
            else if(index >= 0)
            {
                rooms[index].roomInfo.value = roomInfo;
            }
            else
                rooms.Add(new RoomInfoLink(roomInfo));
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected");
        if (connectedToMaster.value)
        {
            if (onDisconnectedFromMaster != null)
                onDisconnectedFromMaster();
        }
        else
        {
            if(onFailedToConnectToMaster != null)
                onFailedToConnectToMaster();
        }

        rooms.Clear();
        localPlayer.value = null;
        connectedToMaster.value = false;
    }
}