using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LimitedPackageState : MonoBehaviour
{
   [SerializeField] private Text timeDisplay;
   [Tooltip("패키지 재구매 쿨타임.분으로 적으면 됨")][SerializeField] private int coolTime;

   private void Start()
   {
      StartCoroutine(CoolTimeRefresh());
   }

   IEnumerator CoolTimeRefresh()
   {
      int min = 0;
      int sec = 0;

      var button = GetComponent<Button>();
      button.enabled = false;
      
      while (PlayerData.GetInstance.BuyLimitedPackageTime>DateTime.Now)
      {
         var leftTime = PlayerData.GetInstance.BuyLimitedPackageTime - DateTime.Now;
         var leftMin = leftTime.Minutes;
         var leftSec = leftTime.Seconds;

         timeDisplay.text = string.Format($"{leftMin:D2}:{leftSec:D2}");
         yield return new WaitForSeconds(0.1f);
      }
      
      PlayerData.GetInstance.BuyLimitedPackageTime=DateTime.MinValue;
      button.enabled = true;
      //timeDisplay.text = "스페셜 패키지"/*I2.Loc.LocalizationManager.GetTermTranslation("SpecialPackage")*/;
   }

   public void PopupOpen()
   {
      var popup = PopupManager.instance.ShowLimitedPackage();
      popup.Init(() =>
      {
         PlayerData.GetInstance.BuyLimitedPackageTime=DateTime.Now+TimeSpan.FromMinutes(coolTime);
         StartCoroutine(CoolTimeRefresh());
      });
   }
}
