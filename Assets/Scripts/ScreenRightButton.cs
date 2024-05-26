using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ScreenRightButton : MonoBehaviour
{
    [SerializeField] Image RoundBgImage;
    [SerializeField] InputAction OnPress;
    Color m_BgImageColor;
    private Coroutine m_Coroutine;
    private Vector3 m_InitScale;
    private float m_PressTime = 0;

    private void Start()
    {

        m_BgImageColor = RoundBgImage.color;
        m_InitScale = RoundBgImage.GetComponent<RectTransform>().localScale;

        OnPress.performed += _ =>
        {
            m_Coroutine = StartCoroutine(HoldAnimeRoutine());
            m_PressTime = Time.time;
        };
        OnPress.canceled += _ =>
        {
            if (Time.time - m_PressTime < 0.4f)
            {
                StopCoroutine(m_Coroutine);
                m_BgImageColor.a = 0;
                RoundBgImage.color = m_BgImageColor;
            }
        };
    }

    private void OnEnable()
    {
        OnPress.Enable();
    }

    private void OnDisable()
    {
        OnPress.Disable();
    }



    IEnumerator HoldAnimeRoutine()
    {
        float animeTime = 1.4f;
        float timeNow = 0;
        float maxaTime = 0.4f;
        float maxa = 0.2f;
        float alpha, scale = 0;
        while (timeNow < animeTime)
        {
            timeNow += Time.deltaTime;
            timeNow = Mathf.Min(timeNow, animeTime);
            if (timeNow < maxaTime)
            {
                scale = Mathf.Lerp(0, 1, timeNow / maxaTime);
                alpha = Mathf.Lerp(0, maxa, timeNow / maxaTime);
            }
            else
            {
                alpha = Mathf.Lerp(maxa, 0, (timeNow - maxaTime) / (animeTime - maxaTime));
            }
            RoundBgImage.GetComponent<RectTransform>().localScale = scale * m_InitScale;
            m_BgImageColor.a = alpha;
            RoundBgImage.color = m_BgImageColor;
            yield return null;
        }
        m_BgImageColor.a = 0;
        RoundBgImage.color = m_BgImageColor;
    }
}
