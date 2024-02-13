using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicPicture : MonoBehaviour
{
    private Image m_BgImage, m_FgImage;
    private Color m_InitBgColor, m_InitFgColor;
    private GameObject m_Front;
    private void Awake()
    {
        m_BgImage = GetComponent<Image>();
        m_Front = transform.GetChild(0).gameObject;
        m_FgImage = m_Front.GetComponent<Image>();
        m_InitBgColor = m_BgImage.color;
        m_InitFgColor = m_FgImage.color;
    }
    public void ResetIcon()
    {
        m_Front.SetActive(false);
    }
    public void SetIcon(Sprite sprite)
    {
        m_Front.SetActive(true);
        m_FgImage.sprite = sprite;
    }
    public Sprite GetIcon()
    {
        if (!gameObject.activeInHierarchy)
        {
            return null;
        }
        return m_FgImage.sprite;
    }

    public void SetBrightness(float brightness)
    {
        m_BgImage.color = m_InitBgColor * brightness;
        m_FgImage.color = m_InitFgColor * brightness;
    }

    public void SetAlpha(float alpha)
    {
        m_BgImage.color = new Color(m_InitBgColor.r, m_InitBgColor.g, m_InitBgColor.b, alpha);
        m_FgImage.color = new Color(m_InitFgColor.r, m_InitFgColor.g, m_InitFgColor.b, alpha);
    }

    public void Resize(Vector2 newSize)
    {
        m_BgImage.rectTransform.sizeDelta = newSize;
        m_FgImage.rectTransform.sizeDelta = newSize;
    }
}
