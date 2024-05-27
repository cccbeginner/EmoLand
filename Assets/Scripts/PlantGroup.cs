using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantGroup : MonoBehaviour
{
    public void GrowAll()
    {
        GrowAllChildren(transform);
    }
    public void DontGrowAll()
    {
        DontGrowAllChildren(transform);
    }

    private void GrowAllChildren(Transform root)
    {
        // Make all children grow
        //  in recurive way.
        if (root.GetComponent<Grass>() != null)
        {
            root.GetComponent<Grass>().StartGrow();
        }
        else if (root.GetComponent<Flower>() != null)
        {
            root.GetComponent<Flower>().GrowAndOpen();
        }
        else if (root.GetComponent<Clover>() != null)
        {
            root.GetComponent<Clover>().Grow();
        }
        else if (root.GetComponent<TaroLeaf>() != null)
        {
            root.GetComponent<TaroLeaf>().StartGrow();
        }
        else if (root.GetComponent<HeartLeaf>() != null)
        {
            root.GetComponent<HeartLeaf>().Grow();
        }
        else
        {
            for (int i = 0; i < root.childCount; i++)
            {
                GrowAllChildren(root.GetChild(i));
            }
        }
    }
    private void DontGrowAllChildren(Transform root)
    {
        // Make all children grow
        //  in recurive way.
        if (root.GetComponent<Grass>() != null)
        {
            root.GetComponent<Grass>().DontGrow();
        }
        else if (root.GetComponent<Flower>() != null)
        {
            root.GetComponent<Flower>().NotGrow();
        }
        else if (root.GetComponent<Clover>() != null)
        {
            root.GetComponent<Clover>().DontGrow();
        }
        else if (root.GetComponent<TaroLeaf>() != null)
        {
            root.GetComponent<TaroLeaf>().DontGrow();
        }
        else if (root.GetComponent<HeartLeaf>() != null)
        {
            root.GetComponent<HeartLeaf>().DontGrow();
        }
        else
        {
            for (int i = 0; i < root.childCount; i++)
            {
                DontGrowAllChildren(root.GetChild(i));
            }
        }
    }
}
