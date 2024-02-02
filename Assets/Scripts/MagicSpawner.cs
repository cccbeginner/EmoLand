using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MagicSpawner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Magic SpawnMagic;

    private GameObject m_PrespawnedObject;
    private NetworkObject m_SpawnedObject;
    private NetworkRunner m_NetworkRunner;
    private MagicPicture m_MagicPic;

    private void Start()
    {
        m_MagicPic = GetComponent<MagicPicture>();
        m_MagicPic.SetBrightness(0.85f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 spawnPos;
        if (FindSpawnPos(eventData.position, out spawnPos))
        {
            UpdatePrespawn(spawnPos);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        StartPrespawn();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        EndPrespawn();
    }

    // Find network runner that is able to spawn network object.
    private bool FindNetworkRunner()
    {
        foreach (NetworkRunner runner in NetworkRunner.Instances)
        {
            if (runner.isActiveAndEnabled && runner.IsPlayer)
            {
                m_NetworkRunner = runner;
            }
        }
        return m_NetworkRunner != null;
    }

    // Find a spawn point due to a screen position,
    //     and calculations for collision objects.
    private bool FindSpawnPos(Vector2 screenPos, out Vector3 hitPoint)
    {
        RaycastHit hit;
        Camera cam = Camera.main;
        Vector3 mouseScreenPos = new Vector3(screenPos.x, screenPos.y, cam.nearClipPlane);

        Ray ray = cam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray.GetPoint(0), ray.direction, out hit, 100))
        {
            hitPoint = hit.point;
            return true;
        }
        hitPoint = Vector3.zero;
        return false;
    }

    private void StartPrespawn()
    {
        Deprespawn();
        Despawn();
    }

    private void UpdatePrespawn(Vector3 worldPos)
    {
        Prespawn(worldPos);
    }

    private void EndPrespawn()
    {
        Spawn();
        Deprespawn();
    }

    // Preview the spawned object when draging.
    // I make a "Prespawned Object" for it.
    // Would check if there is existing prespawned object first.
    private void Prespawn(Vector3 worldPos)
    {
        if (m_PrespawnedObject == null && SpawnMagic != null)
        {
            if (SpawnMagic.Prefab != null)
            {
                m_PrespawnedObject = Instantiate(SpawnMagic.Prefab);
            }
        }
        if (m_PrespawnedObject != null)
        {
            m_PrespawnedObject.transform.position = worldPos;
        }
    }

    // Destroy my "Prespawned Object" for it if there exists.
    private void Deprespawn()
    {
        if (m_PrespawnedObject != null)
        {
            Destroy(m_PrespawnedObject);
            m_PrespawnedObject = null;
        }
    }

    // Spawn a network Object that everyone can see.
    // The Spawn position and rotation is same as Prespawned Object.
    // Will Spawn if PrespawnedObject exists, doesn't matter whether SpawnedObject exists.
    private void Spawn()
    {
        if (m_NetworkRunner == null)
        {
            if (FindNetworkRunner() == false)
            {
                return;
            }
        }
        if (m_PrespawnedObject != null)
        {
            Vector3 pos = m_PrespawnedObject.transform.position;
            Quaternion rot = m_PrespawnedObject.transform.rotation;
            m_SpawnedObject = m_NetworkRunner.Spawn(SpawnMagic.Prefab, pos, rot);
        }
        if (m_SpawnedObject != null)
        {
            m_MagicPic.SetBrightness(1f);
        }
    }

    // Despawn network Object if exists.
    private void Despawn()
    {
        if (m_NetworkRunner == null)
        {
            if (FindNetworkRunner() == false)
            {
                return;
            }
        }
        if (m_SpawnedObject != null)
        {
            m_NetworkRunner.Despawn(m_SpawnedObject);
        }
        m_MagicPic.SetBrightness(0.85f);
    }
}
