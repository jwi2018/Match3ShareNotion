#if UNITY_IOS
//namespace AudienceNetwork
//{
//    public static class AdSettings
//    {
//        [DllImport("__Internal")]
//        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

//        public static void SetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled)
//        {
//            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(advertiserTrackingEnabled);
//        }
//    }
//}

#endif

using System;
using System.Collections.Generic;
using UnityEngine;

public static class StageStatus
{
    public static bool isStageClear = false;
    public static bool isFirstClear = false;
}

public static class StaticScript
{
    public static void GiveRewardFromName(string strName, int RewardAmount)
    {
        if (strName.Contains("DoubleBo"))
        {
            PlayerData.GetInstance.ItemBomb += RewardAmount * 2;
        }
        if (strName.Contains("HamAndBom"))
        {
            PlayerData.GetInstance.ItemHammer += RewardAmount;
            PlayerData.GetInstance.ItemBomb += RewardAmount;
        }
        if (strName.Contains("RainAndBom"))
        {
            PlayerData.GetInstance.ItemColor += RewardAmount;
            PlayerData.GetInstance.ItemBomb += RewardAmount;
        }
        if (strName.Contains("coin"))
        {
            PlayerData.GetInstance.Gold += RewardAmount;
        }
        if (strName.Contains("Hammer"))
        {
            PlayerData.GetInstance.ItemHammer += RewardAmount;
        }
        if (strName.Contains("Rainbow"))
        {
            PlayerData.GetInstance.ItemColor += RewardAmount;
        }
        if (strName.Contains("Bomb"))
        {
            PlayerData.GetInstance.ItemBomb += RewardAmount;
        }
        if (strName.Contains("jackpot"))
        {
            PlayerData.GetInstance.ItemHammer += RewardAmount;
            PlayerData.GetInstance.ItemColor += RewardAmount;
            PlayerData.GetInstance.ItemBomb += RewardAmount;
        }

        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            popupManager.GoldRefresh();
        }
    }

    public static bool SetActiveCheckNULL(GameObject _gob, bool isActive)
    {
        bool rIsNotNull = false;
        if (_gob != null)
        {
            if (_gob.activeSelf != isActive)
            {
                _gob.SetActive(isActive);
                rIsNotNull = true;
            }
        }

        return rIsNotNull;
    }
}

public static class GameVariable
{
    public static int showRemainStarCount = 3000;
    public static int remainStar = 20;

    /// <summary>
    /// 보물 상자 여는데 필요한 별 갯수
    /// </summary>
    /// <param name="count">PlayerData.GetInstance.StarBoxOpenCount이 들어가면 해당 횟수에는 몇개의 별이 필요한지 자동으로 계산</param>
    /// <returns></returns>
    public static int NeedOpenStarCount(int count)
    {
        int temp = 0;
        temp += calculatorNeedStar(PlayerData.GetInstance.StarBoxOpenCount);
        
        return temp;
    }
    
    public static int calculatorNeedStar(int count)
    {
        int baseDeductionStar = 5;

        if (count.Equals(0))
            return baseDeductionStar;
        for (int i = 0; i < count; i++)
        {
            baseDeductionStar += (i + 2) * 5;
        }
        return baseDeductionStar;
    }
    

    public static int RemainStarCount(int count)
    {
        int temp = PlayerData.GetInstance.GetTotalStarCount();

        if (count.Equals(0))
            return temp;
        temp -= calculatorRemainStar(PlayerData.GetInstance.StarBoxOpenCount);
        return temp;
    }

    public static int calculatorRemainStar(int count)
    {
        int totalRemain = 0;
        int main = 0;
        int inc;
        List<int> Star = new List<int>();
        
        if (count.Equals(0))
            return 0;
        
        // count = 1 이상
        for (int i = 0; i < count; i++)
        {
            if (i.Equals(0))
            {
                Star.Add(5);
                continue;
            }

            inc = 10 + ((i - 1) * 5);
            main = Star[i - 1] + inc;
            Star.Add(main);
        }

        foreach (var total in Star)
        {
            totalRemain += total;
        }
        return totalRemain;
    }

    public static int GetNeedOpenStarCount()
    {
        if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
        {
            int StarTemp = Mathf.Max(NeedOpenStarCount(PlayerData.GetInstance.StarBoxOpenCount),0);
            return StarTemp;
        }

        return remainStar;
    }

    public static int GetRemainStarCount()
    {
        if (PlayerData.GetInstance == null)
            return 0;

        var _totalStar = PlayerData.GetInstance.GetTotalStarCount();

        var NeedStar = 0;
        var RemainStar = 0;
        var moreStar = 0;
        for (var nCnt = 0; nCnt <= PlayerData.GetInstance.StarBoxOpenCount; nCnt++)
        {
            if (nCnt < 3)
            {
                NeedStar += 20;
                RemainStar = 20;
            }
            else if (nCnt < 6)
            {
                NeedStar += 20;
                RemainStar = 20;
            }
            else
            {
                NeedStar += 20;
                RemainStar = 20;
            }

            if (nCnt == 0)
            {
            }
            else if (nCnt < 4)
            {
                moreStar += 20;
            }
            else if (nCnt < 7)
            {
                moreStar += 20;
            }
            else
            {
                moreStar += 20;
            }
        }

        if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
        {
            int StarTemp = Mathf.Max(RemainStarCount(PlayerData.GetInstance.StarBoxOpenCount),0);
            return StarTemp;
        }
        else
        {
            int StarTemp = Mathf.Max(_totalStar - (PlayerData.GetInstance.StarBoxOpenCount * GameVariable.remainStar), 0);
            return StarTemp;
        }
        
        //if 서커스 아님
        //int StarTemp = Mathf.Max(_totalStar - (PlayerData.GetInstance.StarBoxOpenCount * GameVariable.remainStar), 0);
        
        //else 서커스임
        //int StarTemp = Mathf.Max(RemainStarCount(PlayerData.GetInstance.StarBoxOpenCount),0);

        //return StarTemp;
    }
}

public static class SavingInfomation
{
    public static int isSavingCoinLevel1 = 3000;
    public static int isSavingCoinLevel2 = 5000;
    public static int isTotalSavingCoin = 9000;

    public static int GetSavingCoin(int _level)
    {
        int rCoin = 0;
        switch (_level)
        {
            case 0:
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_piggy_3000coin");
                rCoin = isSavingCoinLevel1;
                break;

            case 1:
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_piggy_5000coin");
                rCoin = isSavingCoinLevel2;
                break;

            case 2:
                FirebaseManager.GetInstance.FirebaseLogEvent("intro_piggy_9000coin");
                rCoin = isTotalSavingCoin;
                break;
        }

        return rCoin;
    }

    public static bool CompareDateTimes(this DateTime firstDate, DateTime secondDate)
    {
        return firstDate.Day == secondDate.Day && firstDate.Month == secondDate.Month && firstDate.Year == secondDate.Year;
    }

    public static void SetActiveSelf(this GameObject _gob, bool isActive)
    {
        if (_gob.activeSelf != isActive)
        {
            _gob.SetActive(isActive);
        }
    }
}

public interface ICoroutineAnimationController
{
    bool IsNext { set; get; }
}