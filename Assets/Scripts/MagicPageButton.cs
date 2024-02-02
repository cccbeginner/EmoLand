using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class MagicPageButton : MonoBehaviour
{
    public int ColumnNum = 1;

    [SerializeField]
    private Vector2 m_PictureSize = new Vector2(45, 45);
    [SerializeField]
    private GameObject MagicColumn;
    [SerializeField]
    private GameObject MagicColumnField;
    [SerializeField]
    private GameObject MagicPage;
    private bool m_IsColumnActive = false;

    private void Start()
    {
        if (MagicPage != null )
        {
            MagicPage.SetActive( false );
        }
        MagicColumn.GetComponent<RectTransform>().SetAsFirstSibling();
    }

    public void Pressed()
    {
        if (m_IsColumnActive)
        {
            ShowPage();
        }
        else
        {
            ShowColumn();
        }
    }

    private void ConfigMagicPic(GameObject magicPic)
    {
        RectTransform rectTransform = magicPic.GetComponent<RectTransform>();
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.sizeDelta = m_PictureSize;

        MagicSpawner magicColumnField = magicPic.AddComponent<MagicSpawner>();
        magicColumnField.SpawnMagic = null;
    }

    public void ShowPage()
    {
        MagicPage.SetActive(true);
    }


    public void HidePage()
    {
        MagicPage.SetActive(false);
    }

    public void ShowColumn()
    {
        if (m_IsColumnActive) return;
        for (int i = 0; i < ColumnNum; i++)
        {
            GameObject newField = Instantiate(MagicColumnField, MagicColumn.transform);
            ConfigMagicPic(newField);
            RectTransform rtTransform = newField.GetComponent<RectTransform>();
            rtTransform.anchoredPosition = new Vector2(0, -i * rtTransform.rect.height);
        }
        m_IsColumnActive = true;
    }

    public void HideColumn()
    {
        Debug.Log(gameObject.name);
        if (!m_IsColumnActive) return;
        foreach (GameObject field in MagicColumn.transform)
        {
            Destroy(field);
        }
        m_IsColumnActive = false;
    }
}
