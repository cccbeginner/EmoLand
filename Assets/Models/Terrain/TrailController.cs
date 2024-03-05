using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class TrailController : MonoBehaviour
{
    [SerializeField]
    VisualEffect m_TransparentTrailVFX;
    [SerializeField]
    int MovingSpawnRate = 5;
    [SerializeField]
    int IdleSpawnRate = 1;
    [SerializeField]
    Camera m_RenderTextureCamera;
    [SerializeField]
    Material m_TrailMaterial;
    [SerializeField]
    Vector2 m_FieldSize = new Vector2(100, 100);
    [SerializeField]
    Vector2 m_FieldPosition = new Vector2(0,0);
    private WithMainPlayer m_WithMainPlayer;

    private void Start()
    {
        m_TransparentTrailVFX.pause = true;
        m_WithMainPlayer = GetComponent<WithMainPlayer>();
        m_WithMainPlayer.OnMainPlayerJoin.AddListener(OnPlayerJoin);
        InitShaderParams();
    }

    private void Update()
    {
        UpdateShaderParams();
        if (Player.main != null)
        {
            if (Player.main.Velocity.sqrMagnitude < 0.1)
            {
                m_TransparentTrailVFX.SetInt("SpawnRate", IdleSpawnRate);
            }
            else
            {
                m_TransparentTrailVFX.SetInt("SpawnRate", MovingSpawnRate);
            }
        }
    }

    Vector2 tiling;
    private void InitShaderParams()
    {
        float camSize = m_RenderTextureCamera.orthographicSize;
        tiling = m_FieldSize / camSize;
        m_TrailMaterial.SetVector("_RT_Tiling", tiling);
    }

    private void UpdateShaderParams()
    {
        float camSize = m_RenderTextureCamera.orthographicSize;
        float camPosX = m_RenderTextureCamera.transform.position.x - m_FieldPosition.x;
        float camPosZ = m_RenderTextureCamera.transform.position.z - m_FieldPosition.y;
        Vector2 offset = new Vector2(camPosX, camPosZ) / camSize * 0.5f;
        offset -= tiling / 2f - 0.5f * Vector2.one;
        m_TrailMaterial.SetVector("_RT_Offset", offset);
    }

    private void OnPlayerJoin()
    {
        if (Player.main.IsGrounded)
        {
            m_TransparentTrailVFX.pause = true;
        }
        else
        {
            m_TransparentTrailVFX.pause = false;
        }
        Player.main.OnLeaveGround.AddListener(OnPlayerLeaveGround);
        Player.main.OnTouchGround.AddListener(OnPlayerTouchGround);
        Player.main.playerSize.OnResize.AddListener(SetTrailSize);
        SetTrailSize(Player.main.Size);
    }

    private void OnPlayerLeaveGround()
    {
        m_TransparentTrailVFX.pause = true;
    }

    private void OnPlayerTouchGround()
    {
        m_TransparentTrailVFX.pause = false;
    }

    private void SetTrailSize(int playerSize)
    {
        float trailSize = Mathf.Pow(playerSize, 1f/3f);
        m_TransparentTrailVFX.SetFloat("TrailSize", trailSize);
    }
}
