using System;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi;
using UnityEngine;

public class LimitedShopController : Singleton<LimitedShopController>
{
    public int Time = 17;
    
    /*
        조건 1. 팝업 노출 후 노출. (o)
        조건 2. 17시 이후에 노출.
    */

    private void Start()
    {
        CheckTime();
    }

    public void CheckTime()
    {
        if (PlayerData.GetInstance.LimitedShopDay != DateTime.Now.Day)
        {
            PlayerData.GetInstance.IsBuyLimitedPackage = false;
            PlayerData.GetInstance.IsSeeLimitedPackage = false;
            PlayerData.GetInstance.IsWeeklyPopupEnd = false;
        }
        
        var myDt = DateTime.Now;
        //1. 17시 이후(처음시작) - weekly -> limit
        //2. 17시 이후(처음시작 x) - limit
        
        if (myDt.Hour >= Time && PlayerData.GetInstance.IsWeeklyPopupEnd)
        {
            if (!PlayerData.GetInstance.IsLimitedPopupCheck)
            {
                var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
                if (popupManager != null)
                {
                    PlayerData.GetInstance.IsSeeLimitedPackage = false;
                    popupManager.CallLimitedShop();
                    PlayerData.GetInstance.IsLimitedPopupCheck = true;
                }
            }
        }
    }
}
