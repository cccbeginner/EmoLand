using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSize : NetworkBehaviour
{
    public Player player { get { return GetComponent<Player>(); } }

    public Vector3 UnitSizeScale;//(1,1,1)
    public float UnitSizeRadius = 0.25f;
    public int InitSize = 1;
    public int SizeMin = 1;
    public int SizeMax = 10;

    [SerializeField]
    private GameObject m_SlimeModel;

    public UnityEvent<int> OnResize;

    [Networked]
    private bool nt_SizeChanged { get; set; }
    [Networked]
    private int nt_Size { get; set; }
    public int Size
    {
        get
        {
            return nt_Size;
        }
        set
        {
            if (value < 0) return;
            nt_Size = value;
            nt_SizeChanged = true;
            if (nt_Size < SizeMin) nt_Size = SizeMin;
            if (nt_Size > SizeMax) nt_Size = SizeMax;
            OnResize.Invoke(nt_Size);
        }
    }


    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        Size = InitSize;
    }

    private void NT_ReloadSize()
    {
        float sqrt3 = Mathf.Pow(nt_Size, 1f / 3f);
        float r = sqrt3 * UnitSizeRadius;
        m_SlimeModel.transform.localScale = sqrt3 * UnitSizeScale;
        player.PlayerController.center = new Vector3(0, r, 0);
        player.PlayerController.radius = r;
        player.PlayerController.height = 2 * r;
        nt_SizeChanged = false;
    }

    public override void FixedUpdateNetwork()
    {
        if (nt_SizeChanged)
        {
            NT_ReloadSize();
        }
    }
}
