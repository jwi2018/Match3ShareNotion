using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EventLevelStatus : PopupSetting
{
    [SerializeField] private Text remainDate;
    [SerializeField] private GameObject alram;

    private void Start()
    {
        if (PlayerData.GetInstance.PresentLevel < StaticGameSettings.iLimitStageEventLevel)
        {
            if (PopupList.GetInstance.Pop_Event_ComingSoon == null)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(true);
            if (EventLevelSystem.GetInstance != null)
            {
                EventLevelSystem.GetInstance.EventMapOpenPickNumber();

                EventLevelSystem.GetInstance.WeeklyPickRandomNum();
                EventLevelSystem.GetInstance.EventLevelNum = PlayerData.GetInstance.GetEventStageNum();
            }
        }
        if (remainDate != null)
            remainDate.text = "";
        StaticScript.SetActiveCheckNULL(alram, false);

        CalcNextMonday();
    }

    public void CalcNextMonday()
    {
        if (PlayerData.GetInstance.PresentLevel >= StaticGameSettings.iLimitStageEventLevel)
        {
            if (PlayerData.GetInstance.GetIsEventMapAllClear() == true)
            {
                if (remainDate != null)
                {
                    DateTime dateToday = DateTime.Today;
                    int daysUntilMonday = ((int)DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7) % 7;
                    DateTime nextMonday = dateToday.AddDays(daysUntilMonday);
                    TimeSpan span = nextMonday.Subtract(dateToday);
                    if (span.TotalDays != 0)
                    {
                        if (remainDate != null)
                            remainDate.text = string.Format("D-{0}", span.TotalDays);
                    }
                }
            }
            else
            {
                if (remainDate != null)
                    remainDate.text = "";
                StaticScript.SetActiveCheckNULL(alram, true);
            }
        }
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public void ShowPopup()
    {
        StaticShowPopup();
    }

    public static void StaticShowPopup()
    {
        if (PlayerData.GetInstance.PresentLevel < StaticGameSettings.iLimitStageEventLevel)
        {
            if (PopupList.GetInstance.Pop_Event_ComingSoon != null)
            {
                PopupManager.instance.ShowEventLevelCommingSoon();
            }
        }
        else
        {
            PopupManager.instance.OnClickEventLevelButton();
        }
    }
}