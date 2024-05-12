using System.Collections.Generic;
using UnityEngine;

public class SpoonLeaf : MonoBehaviour
{
    public float DownforceCo = 30;
    public float Upforce = 10;
    public float Drag = 1f;
    public float BendMax = 20;

    // Save droplets id and their current size.
    // <droplet id, current size>
    Dictionary<DropletLocal, int> m_TopDroplets = new Dictionary<DropletLocal, int>();
    Dictionary<DropletNetwork, int> m_TopDropletsNet = new Dictionary<DropletNetwork, int>();
    Quaternion m_InitRot;
    int m_TotalSize = 0;

    private void Start()
    {
        m_InitRot = transform.rotation;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contactCount > 0 && collision.GetContact(0).normal.y < -0.3)
        {
            DropletNetwork dropletNetwork = collision.gameObject.GetComponent<DropletNetwork>();
            DropletLocal dropletLocal = collision.gameObject.GetComponent<DropletLocal>();
            if (dropletNetwork != null)
            {
                m_TopDropletsNet.Add(dropletNetwork, dropletNetwork.size);
                m_TotalSize += dropletNetwork.size;
                dropletNetwork.OnResize.AddListener((size) =>
                {
                    OnDropletResize(dropletNetwork, size);
                });
                dropletNetwork.OnBeingDestroy.AddListener(() =>
                {
                    OnDropletDestroy(dropletNetwork);
                });
            }
            else if (dropletLocal != null)
            {
                m_TopDroplets.Add(dropletLocal, dropletLocal.Size);
                m_TotalSize += dropletLocal.Size;
                dropletLocal.OnResize.AddListener((size) =>
                {
                    OnDropletResize(dropletLocal, size);
                });
                dropletLocal.OnEaten.AddListener((size) =>
                {
                    OnDropletDestroy(dropletLocal);
                });
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        DropletNetwork dropletNetwork = collision.gameObject.GetComponent<DropletNetwork>();
        DropletLocal dropletLocal = collision.gameObject.GetComponent<DropletLocal>();
        if (dropletNetwork != null && m_TopDropletsNet.ContainsKey(dropletNetwork))
        {
            m_TopDropletsNet.Remove(dropletNetwork);
            m_TotalSize -= dropletNetwork.size;
            dropletNetwork.OnResize.RemoveListener((size) =>
            {
                OnDropletResize(dropletNetwork, size);
            });
            dropletNetwork.OnBeingDestroy.RemoveListener(() =>
            {
                OnDropletDestroy(dropletNetwork);
            });
        }
        else if (dropletLocal != null && m_TopDroplets.ContainsKey(dropletLocal))
        {
            m_TopDroplets.Remove(dropletLocal);
            m_TotalSize -= dropletLocal.Size;
            dropletLocal.OnResize.RemoveListener((size) =>
            {
                OnDropletResize(dropletLocal, size);
            });
            dropletLocal.OnEaten.RemoveListener((size) =>
            {
                OnDropletDestroy(dropletLocal);
            });
        }
    }

    private void OnDropletResize(DropletNetwork droplet, int newSize)
    {
        if (m_TopDropletsNet.ContainsKey(droplet))
        {
            int sizeDelta = newSize - m_TopDropletsNet[droplet];
            m_TotalSize += sizeDelta;
            m_TopDropletsNet[droplet] = newSize;
        }
    }
    private void OnDropletResize(DropletLocal droplet, int newSize)
    {
        if (m_TopDroplets.ContainsKey(droplet))
        {
            int sizeDelta = newSize - m_TopDroplets[droplet];
            m_TotalSize += sizeDelta;
            m_TopDroplets[droplet] = newSize;
        }
    }

    private void OnDropletDestroy(DropletNetwork droplet)
    {
        if (m_TopDropletsNet.ContainsKey(droplet))
        {
            m_TotalSize -= m_TopDropletsNet[droplet];
            m_TopDropletsNet.Remove(droplet);
        }
    }
    private void OnDropletDestroy(DropletLocal droplet)
    {
        if (m_TopDroplets.ContainsKey(droplet))
        {
            m_TotalSize -= m_TopDroplets[droplet];
            m_TopDroplets.Remove(droplet);
        }
    }

    float m_BendVelocity = 0;

    private void FixedUpdate()
    {
        float currentBend = -(transform.rotation * Quaternion.Inverse(m_InitRot)).eulerAngles.x;
        if (currentBend < -180) currentBend += 360;
        m_BendVelocity += (m_TotalSize * DownforceCo - currentBend * Upforce) * Time.fixedDeltaTime;
        m_BendVelocity *= (1 - Drag * Time.fixedDeltaTime);

        if (currentBend >= BendMax) m_BendVelocity = Mathf.Min(m_BendVelocity, 0);
        transform.rotation *= Quaternion.Euler(-m_BendVelocity * Time.fixedDeltaTime, 0, 0);
    }
}
