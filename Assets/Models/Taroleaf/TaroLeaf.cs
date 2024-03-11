using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaroLeaf : MonoBehaviour
{
    [SerializeField]
    GameObject m_Leaf;
    [SerializeField]
    Material m_Material;

    public float HeightAddition = 0;
    public float MovePeriod = 0;
    public bool HasGrown { get; private set; }

    private float m_CurrentHeightState = 0; // 0->Original, 1->Original+HeightAddition
    private float m_MoveDirection = 1;
    private float m_LeafInitY;
    public bool InitGrow = false;

    void Start()
    {
        m_Leaf.GetComponent<MeshRenderer>().material = m_Material;
        m_LeafInitY = m_Leaf.transform.position.y;
        if (InitGrow)
        {
            StartGrow();
        }
        else
        {
            DontGrow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void DontGrow()
    {
        m_Leaf.GetComponent<Collider>().enabled = false;
        m_Leaf.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", 999999f);
        HasGrown = false;
    }
    public void StartGrow()
    {
        m_Leaf.GetComponent<Collider>().enabled = true;
        m_Leaf.GetComponent<MeshRenderer>().material.SetFloat("_TimeOffset", Time.unscaledTime);
        HasGrown = true;
    }

    void Move()
    {
        float add = Time.deltaTime / (MovePeriod / 2);
        m_CurrentHeightState += add * m_MoveDirection;
        if (m_CurrentHeightState > 1)
        {
            m_MoveDirection = -m_MoveDirection;
            m_CurrentHeightState = 1;
        }
        else if (m_CurrentHeightState < 0)
        {
            m_MoveDirection = -m_MoveDirection;
            m_CurrentHeightState = 0;
        }
        Vector3 leafPos = m_Leaf.transform.position;
        leafPos.y = m_LeafInitY + Mathf.SmoothStep(0, HeightAddition, m_CurrentHeightState);
        m_Leaf.transform.position = leafPos;
    }
}
