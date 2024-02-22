using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    public Vector3 Force;
    public bool Enable = true;

    private Collider m_FieldCollider;
    bool m_PlayerInField = false;

    void Start()
    {
        m_FieldCollider = GetComponent<Collider>();
        if (m_FieldCollider == null)
        {
            Debug.LogWarning("Field collider not found. Add a collider component to present the field area.");
        }
        else
        {
            m_FieldCollider.isTrigger = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (ReferenceEquals(Player.main.gameObject, other.gameObject))
        {
            m_PlayerInField = true;
            StartCoroutine(ApplyPlayerForce());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (ReferenceEquals(Player.main.gameObject, other.gameObject))
        {
            m_PlayerInField = false;
        }
    }

    IEnumerator ApplyPlayerForce()
    {
        while (m_PlayerInField)
        {
            if (Enable)
            {
                Player.main.AddForce(Force * Time.deltaTime);
            }
            yield return null;
        }
    }
}
