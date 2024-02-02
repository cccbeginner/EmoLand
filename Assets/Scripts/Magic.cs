using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Magic
{
    public Sprite Icon;
    public GameObject Prefab;
    public bool FollowPlayer;
    public Magic(Sprite icon = null, GameObject magicPrefab = null, bool followPlayer = false)
    {
        Icon = icon;
        Prefab = magicPrefab;
        FollowPlayer = followPlayer;
    }
}
