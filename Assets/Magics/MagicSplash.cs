using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSplash : MonoBehaviour
{
    [SerializeField] Player player;
    [SerializeField] GameObject SplashParticle;
    Vector3 m_InitScale;

    public void Start()
    {
        m_InitScale = SplashParticle.transform.localScale;
        player.droplet.OnResize.AddListener(SetParticleSize);
        SetParticleSize(player.droplet.size);
    }

    private void SetParticleSize(int size)
    {
        float scale = Mathf.Pow(size, 1f / 3f);
        SplashParticle.transform.localScale = m_InitScale * scale;
    }
}
