using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestStatus : MonoBehaviour
{
    [SerializeField] private Image gaugeBar;
    
    [SerializeField] private Slider completeSlider = null;
    [SerializeField] private Text text_completeRate = null;

    [SerializeField] private Image imgAlram = null;
    [SerializeField] private Text tx_status = null;

    [SerializeField] private DailyQuestPopup DailyQuestPopup = null;
    
    private void Start()
    {
        imgAlram.gameObject.SetActive(false);
       StartCoroutine(CoInit(0.2f));
    }

    private IEnumerator CoInit(float updateTime)
    {
        while (true)
        {
            SetUI();
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void SetUI()
    {
        if (DailyQuestPopup != null)
        {
            DailyQuestPopup.RefreshEntity();
        }
        
        if (DailyQuestManager.GetInstance != null)
        {
            DailyQuestSaveData currentDayData = DailyQuestManager.GetInstance.dailyQuestCurrentData;

            int iCurrentCompleteCount = 0;
            bool isShowAlram = false;
            foreach (DailyQuestData saveData in currentDayData.dailyQuestSaveDatas)
            {
                if (saveData.isGetReward)
                {
                    iCurrentCompleteCount++;
                }
                else
                {
                    if (saveData.saveCount >= saveData.questCount)
                    {
                        isShowAlram = true;
                    }
                }
            }
            if (imgAlram.gameObject.activeSelf != isShowAlram)
            {
                imgAlram.gameObject.SetActive(isShowAlram);
            }

            if (completeSlider != null)
            {
                if (BaseSystem.GetInstance != null)
                {
                    if (BaseSystem.GetInstance.GetSystemList("CircusSystem") ||
                        BaseSystem.GetInstance.GetSystemList("Fantasy"))
                    {
                        completeSlider.maxValue = currentDayData.dailyQuestSaveDatas.Count;
                        completeSlider.value = iCurrentCompleteCount;
                    }
                    else
                    {
                        completeSlider.value = (float)iCurrentCompleteCount / (float)currentDayData.dailyQuestSaveDatas.Count;
                    }
                }
                else
                {
                    completeSlider.value = (float)iCurrentCompleteCount / (float)currentDayData.dailyQuestSaveDatas.Count;
                }
            }

            if (gaugeBar != null)
            {
                gaugeBar.fillAmount= (float)iCurrentCompleteCount / (float)currentDayData.dailyQuestSaveDatas.Count;
            }
            
            if (text_completeRate != null)
            {
                text_completeRate.text = string.Format($"{iCurrentCompleteCount} / {currentDayData.dailyQuestSaveDatas.Count}");
            }

            if (iCurrentCompleteCount == currentDayData.dailyQuestSaveDatas.Count)
            {
                if (tx_status != null)
                {
                    tx_status.text = I2.Loc.LocalizationManager.GetTermTranslation("AllComplete");
                }
            }
            else
            {
            }
        }
    }

    public void test()
    {
        Debug.Log(string.Format("[시간] 현재 시간 : {0}", DateTime.Now.ToString()));
    }
}