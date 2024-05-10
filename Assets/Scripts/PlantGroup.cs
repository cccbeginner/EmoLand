using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGroup : MonoBehaviour
{
    bool m_HasGrown = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        
    }

    public void GrowAll()
    {
        if (m_HasGrown) return;
        m_HasGrown = true;
        GrowAllChildren(transform);
    }

    private void GrowAllChildren(Transform root)
    {
        // Make all children grow
        //  in recurive way.
        if (root.GetComponent<Grass>() != null)
        {
            root.GetComponent<Grass>().StartGrow();
        }else if(root.GetComponent<Flower>() != null)
        {
            root.GetComponent<Flower>().GrowAndOpen();
        }else if(root.GetComponent<Clover>() != null)
        {
            root.GetComponent<Clover>().Grow();
        }
        else if(root.GetComponent<TaroLeaf>() != null)
        {
            root.GetComponent<TaroLeaf>().StartGrow();
        }
        else
        {
            for(int i = 0; i < root.childCount; i++)
            {
                GrowAllChildren(root.GetChild(i));
            }
        }
    }
}
