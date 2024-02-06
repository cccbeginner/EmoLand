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
    public enum SpawnMode
    {
        Move,
        Click
    }
    public MagicData(Sprite icon = null, GameObject magicPrefab = null, SpawnMode spawnOp = SpawnMode.Move)
    {
        Icon = icon;
        Prefab = magicPrefab;
        SpawnOption = spawnOp;
    }
}
