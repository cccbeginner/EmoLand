#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PlantHelper : MonoBehaviour
{
    public GameObject Stem;
    public GameObject[] Leaf;
    [SerializeField]
    public int[] LeafCount;

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

    private void GenerateLeaves(GameObject leaf, int leafCnt)
    {
        for (int i = 0; i < leafCnt; i++)
        {
            Vector3 pos = leaf.transform.position + transform.position;
            Quaternion rot = leaf.transform.rotation * transform.rotation * Quaternion.Euler(0, i * 360 / leafCnt, 0);
            Transform parent = transform;
            GameObject gameObject = Instantiate(leaf, pos, rot, parent);
            gameObject.name = MakePrefix(leaf) + i;
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

    private bool UpdateLeaf(GameObject leaf, int leafCnt)
    {
        if (leaf == null)
        {
            Debug.Log("No leaf assigned!");
            return false;
        }
        if (leafCnt < 0)
        {
            Debug.Log("The value of Leaf Count should be > 0!");
            return false;
        }
        DestroyRelativeObjects(leaf);
        GenerateLeaves(leaf, leafCnt);
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
        for (int i = 0; i < Mathf.Min(LeafCount.Length, Leaf.Length); i++)
        {
            bool success = UpdateLeaf(Leaf[i], LeafCount[i]);
            if (success == false)
            {
                Debug.Log($"Update Leaf {i} failed.");
            }
        }
        Debug.Log("Update Plant finished.");
    }
}
#endif