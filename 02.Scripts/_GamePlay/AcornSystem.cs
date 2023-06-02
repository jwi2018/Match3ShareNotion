using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AcornSystem : Singleton<AcornSystem>
{
    //도토리를 얻을 수 있는 상황 여부
    private bool IsGetting = false;

    private int GetAcornValue = 1;

    public int RewardedStandardValue = 100;
    
    public void GetAcornItem()
    {
        if (AcornSystem.GetInstance!=null)
        {
            if (PlayerData.GetInstance != null)
            {
                ChangeAcornValue(GetAcornValue);
                IsGetting = false;
                if (StageManager.GetInstance != null)
                {
                    PlayerData.GetInstance.SaveGotAcornStageData(StageManager.StageNumber);
                }
            }
        }
    }

    public void ChangeAcornValue(int value)
    {
        if (AcornSystem.GetInstance!=null)
        {
            if (PlayerData.GetInstance != null)
            {
                if (IsGetting)
                {
                    PlayerData.GetInstance.Acorn += value;
                    Debug.Log("[도토리] 도토리 " + value + "개 획득");
                }
            }
        }
    }

    public void SetAcornGetting(bool isGetting)
    {
        if (AcornSystem.GetInstance!=null)
        {
            IsGetting = isGetting;
        }
    }

    public bool GetAcornGetting()
    {
        if (AcornSystem.GetInstance!=null)
        {
            return IsGetting;
        }

        return false;
    }

    public bool IsReceiveItem()
    {
        if (PlayerData.GetInstance.Acorn >= RewardedStandardValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
