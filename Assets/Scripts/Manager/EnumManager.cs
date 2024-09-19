using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnumManager
{
    public enum ClassEnemy
    {
        Normal,
        Boss,
    }

    public enum ETabSetting: int
    {
        Gameplay = 0,
        Graphic = 1,
        Sound = 2,
    }
}
