
using Photon.Realtime;

public class RoomInfoLink
{
    public readonly EventVariable<RoomInfoLink, RoomInfo> roomInfo;

    public RoomInfoLink(RoomInfo roomInfo)
    {
        this.roomInfo = new EventVariable<RoomInfoLink, RoomInfo>(this, roomInfo);
    }
}
