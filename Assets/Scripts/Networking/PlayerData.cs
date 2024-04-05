
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerData
{
    private const string CUSTOMDATA_INDEX_PING = "Ping";
    private const string CUSTOMDATA_INDEX_PASSWORD_VERIFIED = "Password Verified";
    private const string CUSTOMDATA_INDEX_PLAYER_NAME = "Player Name";

    public readonly Player player;

    public readonly EventVariable<PlayerData, string> playerName;
    public readonly EventVariable<PlayerData, int> ping;
    public readonly EventVariable<PlayerData, bool> passwordVerified;

    public PlayerData(Player player)
    {
        this.player = player;

        playerName = new EventVariable<PlayerData, string>(this, player.NickName);
        ping = new EventVariable<PlayerData, int>(this, 0);
        passwordVerified = new EventVariable<PlayerData, bool>(this, false);

        Set_PlayerName(player.NickName);
        PlayerDataChanged(player.CustomProperties);
    }

    public void PlayerDataChanged(Hashtable changes)
    {
        if (changes.ContainsKey(CUSTOMDATA_INDEX_PING))
            ping.value = (int) changes[CUSTOMDATA_INDEX_PING];

        if (changes.ContainsKey(CUSTOMDATA_INDEX_PASSWORD_VERIFIED))
            passwordVerified.value = (bool)changes[CUSTOMDATA_INDEX_PASSWORD_VERIFIED];

        if (changes.ContainsKey(CUSTOMDATA_INDEX_PLAYER_NAME))
            playerName.value = (string)changes[CUSTOMDATA_INDEX_PLAYER_NAME];
    }

    public void Set_PlayerName(string value)
    {
        Hashtable hash = new Hashtable();
        hash.Add(CUSTOMDATA_INDEX_PLAYER_NAME, value);
        player.SetCustomProperties(hash);
        player.NickName = value;
        playerName.value = value;
    }

    public void Set_Ping(int value)
    {
        Hashtable hash = new Hashtable();
        hash.Add(CUSTOMDATA_INDEX_PING, value);
        player.SetCustomProperties(hash);
        ping.value = value;
    }

    public void Set_PasswordVerified(bool value)
    {
        Hashtable hash = new Hashtable();
        hash.Add(CUSTOMDATA_INDEX_PASSWORD_VERIFIED, value);
        player.SetCustomProperties(hash);
        passwordVerified.value = true;
    }
}
