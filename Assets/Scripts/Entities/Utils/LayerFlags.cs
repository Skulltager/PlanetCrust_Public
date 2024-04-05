
using System;

[Flags]
public enum LayerFlags : uint
{
    Default = 1<<0,
    TransparentFX = 1<<1,
    Ignore_Raycast = 1<<2,

    Water = 1<<4,
    UI = 1<<5,
    Ships = 1<<6,
    Projectiles = 1<<7,
    Walls = 1<<8,
    Ships_Detail = 1<<9,
    Path_Finding = 1<<10,

}