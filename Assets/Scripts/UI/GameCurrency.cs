using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class GameCurrency : MonoBehaviour
{
    public enum Type
    {
        WaterDrop
    }

    public Type CurrencyType;
    public int MinNumber = 0;
    public int MaxNumber = 100;
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
        UpdateUI();
    }

    void Update()
    {
        if (CurrencyType == Type.WaterDrop)
        {
            if (Player.main != null)
            {
                int newNum = Math.Clamp(Player.main.droplet.size, MinNumber, MaxNumber);
                if (m_CurrentNumber != newNum)
                {
                    m_CurrentNumber = newNum;
                    UpdateUI();
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (m_Icon != null)
        {
            if (m_CurrentNumber == MinNumber)
            {
                m_Icon.color = MinIconColor;
            }
            else if (m_CurrentNumber == MaxNumber)
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
            if (m_CurrentNumber == MinNumber)
            {
                m_Text.color = MinTextColor;
            }
            else if (m_CurrentNumber == MaxNumber)
            {
                m_Text.color = MaxTextColor;
            }
            else
            {
                m_Text.color = NormalTextColor;
            }
            m_Text.text = m_CurrentNumber.ToString();
        }
    }
}
