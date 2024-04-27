using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class SpoonLeaf : MonoBehaviour
{
    public float DownforceCo = 30;
    public float Upforce = 10;
    public float Drag = 1f;
    public float BendMax = 20;

    // Save droplets id and their current size.
    // <droplet id, current size>
    Dictionary<DropletNetwork, int> m_TopDroplets = new Dictionary<DropletNetwork, int>();
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
            DropletNetwork droplet = collision.gameObject.GetComponent<DropletNetwork>();
            if (droplet != null)
            {
                m_TopDroplets.Add(droplet, droplet.size);
                m_TotalSize += droplet.size;
                droplet.OnResize.AddListener((size) =>
                {
                    OnDropletResize(droplet, size);
                });
                droplet.OnBeingDestroy.AddListener(() =>
                {
                    OnDropletDestroy(droplet);
                });
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        DropletNetwork droplet = collision.gameObject.GetComponent<DropletNetwork>();
        if (droplet != null && m_TopDroplets.ContainsKey(droplet))
        {
            m_TopDroplets.Remove(droplet);
            m_TotalSize -= droplet.size;
            droplet.OnResize.RemoveListener((size) =>
            {
                OnDropletResize(droplet, size);
            });
            droplet.OnBeingDestroy.RemoveListener(() =>
            {
                OnDropletDestroy(droplet);
            });
        }
    }

    private void OnDropletResize(DropletNetwork droplet, int newSize)
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
