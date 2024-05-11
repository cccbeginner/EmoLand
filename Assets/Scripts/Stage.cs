using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Stage : MonoBehaviour
{
    [SerializeField]
    private int StageNum = 0;
    public UnityEvent InitStageDone, InitStageNotYetDone;

    private bool IsStageDone()
    {
        return PlayerDataSystem.currentStage >= StageNum;
    }
    void Start()
    {
        if (IsStageDone())
        {
            InitStageDone.Invoke();
        }
        else
        {
            InitStageNotYetDone.Invoke();
        }
    }

    public void SaveStage()
    {
        PlayerDataSystem.currentStage = StageNum;
    }
}
