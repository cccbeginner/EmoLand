using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MagicPage : MonoBehaviour
{
    [SerializeField]
    private GameObject m_MagicListContent;
    [SerializeField]
    private GameObject m_MagicColumn;

    [SerializeField]
    private GameObject m_MagicPicPrefab;

    [SerializeField]
    private float m_Gap = 20;
    [SerializeField]
    private int m_ColumnNum = 3;
    [SerializeField]
    private Vector2 m_PictureSize = new Vector2(80, 80);

    [SerializeField]
    private MagicData[] m_MagicList;

    private MagicData m_SelectedMagic;
    private GameObject m_SelectedPicture;
    private GameObject m_UnrealPicture;

    private void OnEnable()
    {
        InitMagicList();
        InitMagicColumn();
    }

    private void OnDisable()
    {
        RestoreMagicColumn();
    }

    private void ConfigMagicListPic(GameObject magicPic, MagicData magic)
    {
        RectTransform rectTransform = magicPic.GetComponent<RectTransform>();
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.sizeDelta = m_PictureSize;

        magicPic.GetComponent<MagicPicture>().SetIcon(magic.Icon);

        // Add listener for draging event.
        EventTrigger eventTrigger = magicPic.AddComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.BeginDrag;
        entry1.callback.AddListener(eventData => { StartDragPicture(eventData, magicPic, magic); });
        eventTrigger.triggers.Add(entry1);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.Drag;
        entry2.callback.AddListener(OnDragPicture);
        eventTrigger.triggers.Add(entry2);

        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.EndDrag;
        entry3.callback.AddListener(EndDragPicture);
        eventTrigger.triggers.Add(entry3);
    }

    private void ConfigMagicColumnPic(GameObject magicPic)
    {
        if (magicPic.GetComponent<EventTrigger>() == null)
        {
            magicPic.AddComponent<EventTrigger>();
        }

        // Add click listener to reset column.
        EventTrigger eventTrigger = magicPic.GetComponent<EventTrigger>();
        EventTrigger.Entry clickEntry = new EventTrigger.Entry();
        clickEntry.eventID = EventTriggerType.PointerClick;
        clickEntry.callback.AddListener(_ => { ResetColumnPic(magicPic); });
        if (!eventTrigger.triggers.Contains(clickEntry))
        {
            eventTrigger.triggers.Add(clickEntry);
        }
    }

    private void RestoreMagicColumnPic(GameObject magicPic)
    {
        EventTrigger eventTrigger = magicPic.GetComponent<EventTrigger>();
        if (eventTrigger == null) return;
        eventTrigger.triggers.Clear();
    }

    private void ConfigUnrealPic()
    {
        if (m_UnrealPicture == null) return;
        RectTransform rect = m_UnrealPicture.GetComponent<RectTransform>();
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        m_UnrealPicture.GetComponent<MagicPicture>().SetAlpha(0.5f);
        m_UnrealPicture.GetComponent<MagicPicture>().Resize(m_PictureSize * 0.5f);
    }

    private void StartDragPicture(BaseEventData _eventData, GameObject magicPic, MagicData magic)
    {
        PointerEventData eventData = (PointerEventData)_eventData;
        if (m_SelectedPicture != null) return;
        m_SelectedPicture = magicPic;
        m_SelectedMagic = magic;
        MakeUnrealPicture(eventData.position);
    }

    private void OnDragPicture(BaseEventData _eventData)
    {
        PointerEventData eventData = (PointerEventData)_eventData;
        UpdateUnrealPicture(eventData.position);
    }

    private void EndDragPicture(BaseEventData _eventData)
    {
        PointerEventData eventData = (PointerEventData)_eventData;
        FindAndSetColumn(eventData.position);
        m_SelectedPicture = null;
        m_SelectedMagic = null;
        ResetUnrealPicture();
    }

    private void MakeUnrealPicture(Vector3 mousePos)
    {
        m_UnrealPicture = Instantiate(m_SelectedPicture, transform.parent);
        ConfigUnrealPic();
        UpdateUnrealPicture(mousePos);
    }

    private void UpdateUnrealPicture(Vector3 mousePos)
    {
        if (m_UnrealPicture == null) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(), mousePos, transform.parent.GetComponent<Canvas>().worldCamera, out Vector2 pos);
        m_UnrealPicture.GetComponent<RectTransform>().anchoredPosition = pos;
    }

    private void ResetUnrealPicture()
    {
        if(m_UnrealPicture != null)
        {
            Destroy(m_UnrealPicture);
        }
        m_UnrealPicture = null;
    }

    private void FindAndSetColumn(Vector3 mousePos)
    {
        foreach (Transform child in m_MagicColumn.transform)
        {
            RectTransform rect = child.GetComponent<RectTransform>();
            if (RectTransformUtility.RectangleContainsScreenPoint(rect, mousePos))
            {
                SetColumnPic(child.gameObject);
            }
        }
    }

    private void SetColumnPic(GameObject magicPic)
    {
        Sprite curIcon = m_SelectedPicture.GetComponent<MagicPicture>().GetIcon();
        magicPic.transform.GetComponent<MagicPicture>().SetIcon(curIcon);
        magicPic.GetComponent<MagicSpawner>().SpawnMagic = m_SelectedMagic;
    }

    private void ResetColumnPic(GameObject magicPic)
    {
        magicPic.transform.GetComponent<MagicPicture>().ResetIcon();
        magicPic.GetComponent<MagicSpawner>().SpawnMagic = null;
    }

    private void InitMagicList()
    {
        // Destroy all content and then add again.
        foreach (Transform child in m_MagicListContent.transform)
        {
            Destroy(child.gameObject);
        }

        Vector2 prefabSize = m_PictureSize;
        m_MagicListContent.GetComponent<RectTransform>().sizeDelta = new Vector2(m_ColumnNum * (prefabSize.y + m_Gap) - m_Gap, (m_MagicList.Length + m_ColumnNum - 1) / m_ColumnNum * (prefabSize.y + m_Gap) - m_Gap);
        for (int i = 0; i < m_MagicList.Length; i++)
        {
            GameObject magicPic = Instantiate(m_MagicPicPrefab, m_MagicListContent.transform);
            ConfigMagicListPic(magicPic, m_MagicList[i]);
            RectTransform rectTransform = magicPic.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2((i % m_ColumnNum) * (prefabSize.y + m_Gap), - (i / m_ColumnNum) * (prefabSize.y + m_Gap));
        }
    }

    private void InitMagicColumn()
    {
        foreach (Transform child in m_MagicColumn.transform)
        {
            ConfigMagicColumnPic(child.gameObject);
        }
    }

    private void RestoreMagicColumn()
    {
        foreach (Transform child in m_MagicColumn.transform)
        {
            RestoreMagicColumnPic(child.gameObject);
        }
    }
}
