using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreeMapNodesUI : MonoBehaviour
{
    [SerializeField] List<Image> NodeList;
    [SerializeField] Color LightColor, DarkColor;
    [SerializeField] float AnimeZoomTime = 1;
    [SerializeField] float AnimeLightTime = 1;
    [SerializeField] float AnimeZoomScale = 5;
    List<bool> m_IsLighten;
    Quaternion m_Rotation;
    int m_NextLightId = -1;
    RectTransform m_RectTransform;
    Vector2 m_InitPos;

    private void Awake()
    {
        m_IsLighten = new List<bool>(new bool[NodeList.Count]);
        m_Rotation = Quaternion.identity;
        m_RectTransform = GetComponent<RectTransform>();
        m_InitPos = m_RectTransform.anchoredPosition;
    }

    private void Start()
    {
        int curStage = PlayerDataSystem.currentStage;
        for (int i = 0; i < NodeList.Count; i++)
        {
            int refVal = i;
            if (i == 0) refVal = 1;
            if (curStage >= refVal)
            {
                LightNode(i);
            }
            else
            {
                DarkNode(i);
            }
        }
    }

    private void OnEnable()
    {
        m_RectTransform.anchorMin = new Vector2(0f, 1f);
        m_RectTransform.anchorMax = new Vector2(0f, 1f);
        m_RectTransform.anchoredPosition = m_InitPos;
        m_RectTransform.localScale = Vector3.one;
        if (m_NextLightId != -1)
        {
            StartCoroutine(LightUpAnimation(m_NextLightId));
        }
    }

    private void OnDisable()
    {
        if (m_NextLightId != -1)
        {
            LightNode(m_NextLightId);
            m_NextLightId = -1;
        }
    }

    private void Update()
    {
        // rotate the lights as vfx

        m_Rotation *= Quaternion.Euler(0, 0, Time.deltaTime * 100);

        for(int i = 0; i < NodeList.Count; i++)
        {
            if (m_IsLighten[i])
            {
                NodeList[i].rectTransform.rotation = m_Rotation;
            }
        }
    }

    private void LightNode(int id)
    {
        m_IsLighten[id] = true;
        NodeList[id].color = LightColor;
    }
    private void DarkNode(int id)
    {
        m_IsLighten[id] = false;
        NodeList[id].color = DarkColor;
    }

    // id from 0 to 6
    public void LightUpTree(int id)
    {
        if (gameObject.activeInHierarchy) StartCoroutine(LightUpAnimation(id));
        else m_NextLightId = id;
    }

    IEnumerator ZoomInRoutine()
    {
        // zoom in
        float time = 0;
        Vector3 worldPos = m_RectTransform.position;
        m_RectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        m_RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        m_RectTransform.position = worldPos;
        Vector2 initPos = m_RectTransform.anchoredPosition;
        while (time < AnimeZoomTime)
        {
            time += Time.deltaTime;
            Vector2 curPos = Vector3.Lerp(initPos, Vector2.zero, Mathf.SmoothStep(0, 1, time / AnimeZoomTime));
            Vector3 curScale = Vector3.one * Mathf.SmoothStep(1, AnimeZoomScale, time / AnimeZoomTime);
            m_RectTransform.anchoredPosition = curPos;
            m_RectTransform.localScale = curScale;
            yield return null;
        }
    }

    IEnumerator ZoomOutRoutine()
    {
        // zoom out
        float time = 0;
        Vector3 worldPos = m_RectTransform.position;
        m_RectTransform.anchorMin = new Vector2(0f, 1f);
        m_RectTransform.anchorMax = new Vector2(0f, 1f);
        m_RectTransform.position = worldPos;
        Vector3 initPos = m_RectTransform.anchoredPosition;
        while (time < AnimeZoomTime)
        {
            time += Time.deltaTime;
            Vector2 curPos = Vector3.Lerp(initPos, m_InitPos, Mathf.SmoothStep(0, 1, time / AnimeZoomTime));
            Vector3 curScale = Vector3.one * Mathf.SmoothStep(AnimeZoomScale, 1, time / AnimeZoomTime);
            m_RectTransform.anchoredPosition = curPos;
            m_RectTransform.localScale = curScale;
            yield return null;
        }
    }

    IEnumerator LightRoutine(int id)
    {
        float time = 0;
        while (time < AnimeLightTime)
        {
            time += Time.deltaTime;
            Color curColor = Color.Lerp(DarkColor, LightColor, Mathf.SmoothStep(0, 1, time / AnimeLightTime));
            NodeList[id].color = curColor;
            yield return null;
        }
    }

    IEnumerator LightUpAnimation(int id)
    {
        yield return new WaitForSeconds(2f);
        yield return ZoomInRoutine();
        yield return new WaitForSeconds(0.5f);
        yield return LightRoutine(id);
        yield return new WaitForSeconds(0.5f);
        yield return ZoomOutRoutine();

        m_IsLighten[id] = true;
        m_NextLightId = -1;
    }
}
