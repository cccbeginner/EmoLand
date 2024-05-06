using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TransitionPage : MonoBehaviour
{

    [SerializeField] Image Background;
    [SerializeField] TMP_Text Text;
    Color m_BgColor;
    Color m_TextColor;

    [SerializeField] string[] TextArray;

    void OnEnable()
    {
        m_BgColor = Background.color;
        m_TextColor = Text.color;
        Open();
        StartLoading();
    }

    public void Open()
    {
        StartCoroutine(OpenRoutine());
    }
    public void Close()
    {
        StartCoroutine(CloseRoutine());
    }

    public void StartLoading()
    {
        StartCoroutine(LoadingRoutine());
    }

    private void SetAlpha(float alpha)
    {
        m_TextColor.a = alpha;
        Text.color = m_TextColor;

        m_BgColor.a = alpha;
        Background.color = m_BgColor;
    }

    IEnumerator OpenRoutine()
    {
        float timeNow = 0f;
        float timeOpen = 0.5f;
        while (timeNow / timeOpen < 1f)
        {
            timeNow += Time.deltaTime;
            float alpha = timeNow / timeOpen;
            SetAlpha(alpha);
            yield return null;
        }
    }

    IEnumerator CloseRoutine()
    {
        float timeNow = 0f;
        float timeOpen = 0.5f;
        while (timeNow / timeOpen < 1f)
        {
            timeNow += Time.deltaTime;
            float alpha = 1 - timeNow / timeOpen;
            SetAlpha(alpha);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    IEnumerator LoadingRoutine()
    {
        float time = 0f;
        float timeUpdate = 0.4f;
        int idx = 0;
        while (true)
        {
            time += Time.deltaTime;
            if (time > timeUpdate)
            {
                time = 0f;
                idx += 1;
                if (idx >= TextArray.Length)
                {
                    idx = 0;
                }
                Text.text = TextArray[idx];
            }
            yield return null;
        }
    }
}
