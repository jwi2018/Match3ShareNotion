using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ChallengeSystem : Singleton<ChallengeSystem>
{
    private bool isChallengeStage = false;
    
    public bool IsChallengeStage
    {
        get => isChallengeStage;
        set
        {
            if (value)
            {
                FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_enter"));
            }
            else
            {
                FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_out"));
            }
            isChallengeStage = value;
        }
    }
    
    /// <summary>
    /// 챌린지 도달했던 스테이지인지 아닌지 판단
    /// </summary>
    /// <param name="stageNum"></param>
    /// <returns></returns>
    public bool GetPassChallengeStage(int stageNum)
    {
        if (ChallengeSystem.GetInstance!=null)
        {
            if (PlayerData.GetInstance != null)
            {
                if (PlayerData.GetInstance.GetChallengeStage().Contains(stageNum))
                {
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    /// <summary>
    /// 해당 스테이지를 PlayData의 List에 저장
    /// </summary>
    /// <param name="stageNum"></param>
    /// <returns></returns>
    public void SetPassChallengeStage(int stageNum)
    {
        if (ChallengeSystem.GetInstance!=null)
        {
            if (PlayerData.GetInstance != null)
            {
                PlayerData.GetInstance.SaveChallengeData(stageNum);
            }
        }
    }

    /// <summary>
    /// 매 스테이지 클리어마다 동작하여 챌린지 스테이지인가 판별하는 함수
    /// </summary>
    /// <param name="stageNum"></param>
    /// <returns>true : 챌린지 팝업 활성화</returns>
    /// <returns>false : 챌린지 팝업 비활성화</returns>
    public bool ConfirmChallengeStage(int stageNum)
    {
        if (ChallengeSystem.GetInstance!=null)
        {
            var isChallengeStage = stageNum % 20;

            /*
            if (PlayerData.GetInstance.GetChallengeStage() == null)
            {
                SetPassChallengeStage(stageNum);
                return true;
            }
            */

            if (stageNum.Equals(50))
            {
                if (GetPassChallengeStage(stageNum))
                {
                    return false;
                }
                else
                {
                    SetPassChallengeStage(stageNum);
                    return true;
                }
            }

            if (stageNum >= 200 && isChallengeStage.Equals(0))
            {
                if (GetPassChallengeStage(stageNum))
                {
                    return false;
                }
                else
                {
                    SetPassChallengeStage(stageNum);
                    return true;
                }
            }
            return false;
        }
        return false;
    }

    public void ShowFailPopup()
    {
        if(IsChallengeStage)
        {
            var popupManagerObject = GameObject.Find("PopupManager");
            if (popupManagerObject != null)
            {
                var popupManager = popupManagerObject.GetComponent<PopupManager>();
                
                GetInstance.IsChallengeStage = false;
                popupManager.ShowChallengeFailPopup();
            }
        }
    }
}