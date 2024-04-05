using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class ServerListItem : DataDrivenUI<RoomInfoLink>
{
    [SerializeField] private Text serverNameText = default;
    [SerializeField] private Text playerCountInfoText = default;
    [SerializeField] private Text serverPingText = default;
    [SerializeField] private Text hasPasswordText = default;
    [SerializeField] private Button selectButton = default;
    [SerializeField] private GameObject selectedContent = default;
    [SerializeField] private GameObject unselectedContent = default;

    public readonly EventVariable<ServerListItem, bool> selected;
    public static readonly EventVariable<ServerListItem, ServerListItem> selectedServerListItem;

    static ServerListItem()
    {
        selectedServerListItem = new EventVariable<ServerListItem, ServerListItem>(null, null);
    }

    private ServerListItem()
    {
        selected = new EventVariable<ServerListItem, bool>(this, false);
    }

    private void Awake()
    {
        selectButton.onClick.AddListener(OnPress_SelectButton);
        selectedServerListItem.onValueChangeImmediate += OnValueChanged_SelectedServerListItem;
        selected.onValueChangeImmediate += OnValueChanged_Selected;
    }

    private void OnValueChanged_SelectedServerListItem(ServerListItem oldValue, ServerListItem newValue)
    {
        selected.value = newValue == this;
    }

    private void OnValueChanged_Selected(bool oldValue, bool newValue)
    {
        selectedContent.SetActive(newValue);
        unselectedContent.SetActive(!newValue);
    }

    private void OnPress_SelectButton()
    {
        selectedServerListItem.value = this;
    }

    protected override void OnValueChanged_Data(RoomInfoLink oldValue, RoomInfoLink newValue)
    {
        if(oldValue != null)
        {
            oldValue.roomInfo.onValueChange -= OnValueChanged_RoomInfo;
        }

        if(newValue != null)
        {
            newValue.roomInfo.onValueChangeImmediate += OnValueChanged_RoomInfo;
        }
    }

    private void OnValueChanged_RoomInfo(RoomInfo oldValue, RoomInfo newValue)
    {
        if (newValue == null)
            return;

        serverNameText.text = data.roomInfo.value.Name;

        int serverPing = (int)data.roomInfo.value.CustomProperties[NetworkManager.ROOM_CUSTOMDATA_INDEX_PING];
        bool hasPassword = (bool)data.roomInfo.value.CustomProperties[NetworkManager.ROOM_CUSTOMDATA_INDEX_HAS_PASSWORD];

        playerCountInfoText.text = string.Format("{0}/{1}", data.roomInfo.value.PlayerCount, data.roomInfo.value.MaxPlayers);
        serverPingText.text = serverPing + " ms";
        hasPasswordText.text = hasPassword ? "LOCK" : "";
    }

    private void OnDestroy()
    {
        selected.onValueChange -= OnValueChanged_Selected;
        selectButton.onClick.RemoveListener(OnPress_SelectButton);

        if (selectedServerListItem.value == this)
            selectedServerListItem.value = null;
    }
}