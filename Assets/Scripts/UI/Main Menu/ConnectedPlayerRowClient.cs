
using UnityEngine;
using UnityEngine.UI;

public class ConnectedPlayerRowClient : DataDrivenUI<PlayerData>
{
    [SerializeField] private Color hostColor = default;
    [SerializeField] private Color clientColor = default;
    [SerializeField] private Text playerNameText = default;
    [SerializeField] private Text pingText = default;
    [SerializeField] private CanvasGroup canvasGroup = default;
    [SerializeField] private LayoutElement layoutElement = default;

    protected override void OnValueChanged_Data(PlayerData oldValue, PlayerData newValue)
    {
        if (oldValue != null)
        {
            oldValue.ping.onValueChange -= OnValueChanged_Ping;
            oldValue.passwordVerified.onValueChange -= OnValueChanged_PasswordVerified;
            oldValue.playerName.onValueChange -= OnValueChanged_PlayerName;
        }
        if (newValue != null)
        {
            newValue.ping.onValueChangeImmediate += OnValueChanged_Ping;
            newValue.passwordVerified.onValueChangeImmediate += OnValueChanged_PasswordVerified;
            newValue.playerName.onValueChangeImmediate += OnValueChanged_PlayerName;
            playerNameText.color = data.player.IsMasterClient ? hostColor : clientColor;
        }
    }

    private void OnValueChanged_PasswordVerified(bool oldValue, bool newValue)
    {
        if(!newValue)
        {
            canvasGroup.Hide();
            layoutElement.ignoreLayout = true;
        }
        else
        {
            canvasGroup.Show();
            layoutElement.ignoreLayout = false;
        }
    }

    private void OnValueChanged_Ping(int oldValue, int newValue)
    {
        pingText.text = string.Format("{0} ms", newValue);
    }

    private void OnValueChanged_PlayerName(string oldValue, string newValue)
    {
        playerNameText.text = newValue;
    }
}