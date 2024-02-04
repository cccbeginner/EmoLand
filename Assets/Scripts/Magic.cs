using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
public class Magic
{
    public Sprite Icon;
    public GameObject Prefab;
    public SpawnMode SpawnOption;
    public enum SpawnMode
    {
        Move,
        Click
    }
    public Magic(Sprite icon = null, GameObject magicPrefab = null, SpawnMode spawnOp = SpawnMode.Move)
    {
        Icon = icon;
        Prefab = magicPrefab;
        SpawnOption = spawnOp;
    }
}
