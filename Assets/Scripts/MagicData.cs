using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
public class MagicData
{
    public Sprite Icon;
    public GameObject Prefab;
    public SpawnMode SpawnOption;
    // Delay time in second for despawn magic prefab after Spawn.
    // Set to 0 for those magic for infinite time.
    public float AutoDespawnDelay;
    public enum SpawnMode
    {
        Move,
        Click
    }
    public MagicData(Sprite icon = null, GameObject magicPrefab = null, SpawnMode spawnOp = SpawnMode.Move, float autoDespawnDelay = 0f)
    {
        Icon = icon;
        Prefab = magicPrefab;
        SpawnOption = spawnOp;
        AutoDespawnDelay = autoDespawnDelay;
    }
}
