using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class PlayerSizeUI : MonoBehaviour
{
    public Color MinIconColor = Color.yellow;
    public Color NormalIconColor = Color.white;
    public Color MaxIconColor = Color.red;
    public Color MinTextColor = Color.yellow;
    public Color NormalTextColor = Color.white;
    public Color MaxTextColor = Color.red;

    [SerializeField]
    Image m_Icon;
    [SerializeField]
    TMPro.TMP_Text m_Text;

    private int m_CurrentNumber = 1;

    private void Awake()
    {
        if (m_Icon == null)
        {
            Debug.LogWarning("Pls set Icon for currency!");
        }
        if (m_Text == null)
        {
            Debug.LogWarning("Pls set text field for currency!");
        }
    }

    void Update()
    {
        if (Player.main != null)
        {
            UpdateUI();
        }
        m_Icon.GetComponent<RectTransform>().rotation *= Quaternion.Euler(0,0,100*Time.deltaTime);
    }

    private void UpdateUI()
    {
        m_CurrentNumber = Player.main.droplet.size;
        int minNumber = Player.main.droplet.SizeMin;
        int maxNumber = Player.main.droplet.SizeMax;
        if (m_Icon != null)
        {
            if (m_CurrentNumber == minNumber)
            {
                m_Icon.color = MinIconColor;
            }
            else if (m_CurrentNumber == maxNumber)
            {
                m_Icon.color = MaxIconColor;
            }
            else
            {
                m_Icon.color = NormalIconColor;
            }
        }
        if (m_Text != null)
        {
            if (m_CurrentNumber == minNumber)
            {
                m_Text.color = MinTextColor;
            }
            else if (m_CurrentNumber == maxNumber)
            {
                m_Text.color = MaxTextColor;
            }
            else
            {
                m_Text.color = NormalTextColor;
            }
            m_Text.text = (m_CurrentNumber - 1).ToString();
        }
    }
}
