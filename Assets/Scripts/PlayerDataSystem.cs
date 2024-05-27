using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataSystem : MonoBehaviour
{
    const string PrefKeyCurrentStage = "Stage";

    public static PlayerDataSystem instance;
    public static int currentStage {
        get { return instance.m_CurrentStage; }
        set { instance.SaveStage(value); } 
    }
    public void SaveStage(int stage)
    {
        PlayerPrefs.SetInt(PrefKeyCurrentStage, stage);
        m_CurrentStage = stage;
    }

    public int GetStage()
    {
        m_CurrentStage = PlayerPrefs.GetInt(PrefKeyCurrentStage, 0);
        return m_CurrentStage;
    }

    private int m_CurrentStage = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            GetStage();
            //SaveStage(0);
        }
        else
        {
            Destroy(this);
        }
    }
}
