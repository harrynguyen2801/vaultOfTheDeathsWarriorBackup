using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public enum SpawnType : int
    {
        Skeleton = 0,
        MageSkeleton = 1,
        DragonGreen = 2,
        DragonRed = 3,
        EarthElementals = 4,
        LavaElementals = 5,
        Reaper = 6,
        Treant = 7,
    }

    public enum Phase
    {
        Phase1 = 1,
        Phase2 = 2,
        Phase3 = 3,
        Phase4 = 4,
    }
    public SpawnType typeSpawner;
    public Phase phaseSpawner;
}
