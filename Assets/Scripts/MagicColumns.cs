using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MagicColumns : MonoBehaviour
{
    public int ColumnNum = 1;

    [SerializeField]
    private GameObject MagicColumnField;
    private RectTransform m_RectTransform;
    private List<GameObject> m_Fields = new List<GameObject>();

    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
    }

    public void ShowColumn()
    {
        if (m_Fields.Count > 0) return;
        for (int i = 0; i < ColumnNum; i++)
        {
            GameObject newField = Instantiate(MagicColumnField, transform.parent);
            RectTransform rtTransform = newField.GetComponent<RectTransform>();
            rtTransform.anchoredPosition = new Vector2(0, -i * rtTransform.rect.height) + m_RectTransform.anchoredPosition;
            rtTransform.SetAsFirstSibling();
            m_Fields.Add(newField);
        }
    }

    public void HideColumn()
    {
        Debug.Log(gameObject.name);
        if (m_Fields.Count == 0) return;
        foreach (GameObject field in m_Fields)
        {
            Destroy(field);
        }
        m_Fields.Clear();
    }
}
