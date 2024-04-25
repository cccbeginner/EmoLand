using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shansu : MonoBehaviour
{
    [SerializeField]
    GameObject m_Leaf;
    [SerializeField]
    Material m_GrowMaterial;
    [SerializeField]
    Material m_ThrowMaterial;
    public Vector3 m_ThrowMagnitude;
    public bool HasGrown { get; private set; }
    public bool triggerThrow = false;
    public bool triggerGrow = false;

    void Awake()
    {
        m_Leaf.GetComponent<MeshRenderer>().material = m_GrowMaterial;
        //DontGrow();
        //StartGrow();

    }

    void Update()
    {
        if (triggerThrow)
        {
            triggerThrow = false;
            ThrowPlayer(0.8f);
        }
        if (triggerGrow)
        {
            triggerGrow = false;
            StartGrow();
        }
    }

    public void StartGrow()
    {
        m_Leaf.GetComponent<Collider>().enabled = true;
        m_Leaf.GetComponent<MeshRenderer>().material = m_GrowMaterial;
        m_Leaf.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", Time.fixedTime);
        HasGrown = true;
    }

    public void DontGrow()
    {
        m_Leaf.GetComponent<Collider>().enabled = false;
        m_Leaf.GetComponent<MeshRenderer>().material = m_GrowMaterial;
        m_Leaf.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", 999999f);
        HasGrown = false;
    }

    public void ThrowPlayer(float delaySec)
    {
        MeshRenderer mr = m_Leaf.GetComponent<MeshRenderer>();
        mr.material = m_ThrowMaterial;
        mr.material.SetFloat("_TimeOffset", Time.fixedTime);
        StartCoroutine(AddPlayerImpactAfterSecond(delaySec));
    }

    IEnumerator AddPlayerImpactAfterSecond(float delaySec)
    {
        yield return new WaitForSeconds(delaySec);
        if (Player.main != null)
        {
            Player.main.rigidBody.AddForce(m_ThrowMagnitude, ForceMode.Impulse);
        }
    }
}
