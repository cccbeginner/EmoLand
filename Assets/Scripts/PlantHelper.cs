#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class PlantHelper : MonoBehaviour
{
    [Serializable]
    public struct PlantHelperLeafData
    {
        public GameObject Leaf;
        public int LeafCount;
        public Vector3 PositionOffset;
        public Vector3 RotateOffset;
    }
    public GameObject Stem;
    public PlantHelperLeafData[] Leaves;

    private string MakePrefix(GameObject leaf)
    {
        return "_" + leaf.name + "_";
    }

    private void DestroyRelativeObjects(GameObject go)
    {
        // Create an empty list to store the "_leaf" children
        List<GameObject> leavesToRemove = new List<GameObject>();

        // Loop through all child objects
        foreach (Transform child in transform)
        {
            // Check if the child's name starts with "_leaf"
            string namePrefix = MakePrefix(go);
            if (child.name.StartsWith(namePrefix))
            {
                // Add the child to the removal list
                leavesToRemove.Add(child.gameObject);
            }
        }

        // Destroy all children in the removal list
        foreach (GameObject obj in leavesToRemove)
        {
            DestroyImmediate(obj);
        }
    }

    private void GenerateLeaves(PlantHelperLeafData leafData)
    {
        for (int i = 0; i < leafData.LeafCount; i++)
        {
            Transform parent = transform;
            Vector3 pos = leafData.Leaf.transform.position + leafData.PositionOffset * i + parent.position;
            Quaternion rot = leafData.Leaf.transform.rotation * Quaternion.Euler(leafData.RotateOffset * i);
            GameObject gameObject = Instantiate(leafData.Leaf, pos, rot, parent);
            gameObject.name = MakePrefix(leafData.Leaf) + i;
        }
    }

    private void GenerateStem(GameObject stem)
    {
        Vector3 pos = stem.transform.position + transform.position;
        Quaternion rot = stem.transform.rotation * transform.rotation;
        Transform parent = transform;
        GameObject gameObject = Instantiate(stem, pos, rot, parent);
        gameObject.name = MakePrefix(stem);
    }

    private bool UpdateLeaf(PlantHelperLeafData leafData)
    {
        if (leafData.Leaf == null)
        {
            Debug.Log("No leaf assigned!");
            return false;
        }
        if (leafData.LeafCount < 0)
        {
            Debug.Log("The value of Leaf Count should be > 0!");
            return false;
        }
        DestroyRelativeObjects(leafData.Leaf);
        GenerateLeaves(leafData);
        return true;
    }
    private bool UpdateStem()
    {
        if (Stem == null)
        {
            Debug.Log("No stem assigned!");
            return false;
        }
        DestroyRelativeObjects(Stem);
        GenerateStem(Stem);
        return true;
    }

    public void UpdatePlant()
    {
        UpdateStem();
        for (int i = 0; i < Leaves.Count(); i++)
        {
            bool success = UpdateLeaf(Leaves[i]);
            if (success == false)
            {
                Debug.Log($"Update Leaf {i} failed.");
            }
        }
        Debug.Log("Update Plant finished.");
    }
}
#endif