using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using GooglePlayGames.BasicApi;
using UnityEngine;
using Random = UnityEngine.Random;

public class EventLevelSystem : Singleton<EventLevelSystem>
{
    private const int PickRandomCount = 5;

    private bool isEventLevel;
    private int originLevel;
    private int eventMapStageNum;
    public bool isRetry;

    public bool IsEventLevel
    {
        get => isEventLevel;
        set => isEventLevel = value;
    }

    public int OriginLevel
    {
        get => originLevel;
        set => originLevel = value;
    }

    public int EventMapStageNum
    {
        get => eventMapStageNum;
        set => eventMapStageNum = value;
    }

    public int EventLevelNum = 1;
    public int PickRandomNum;

    private DateTime day = DateTime.Now;

    //처음 시작할때 뽑아줌.(이벤트맵 오픈시)
    public void EventMapOpenPickNumber()
    {
        if (PlayerData.GetInstance.GetEventLevelList().Count.Equals(0))
        {
            for (int i = 0; i < PickRandomCount; i++) PickRandomNumber(i);

            //if (day.DayOfWeek != DayOfWeek.Monday) PlayerData.GetInstance.SaveIsWeeklyOnePick(true);

            
            DateTime dateToday = DateTime.Today;
            int daysUntilMonday;
            
            if (day.DayOfWeek == DayOfWeek.Monday)
            {
                daysUntilMonday = ((int) DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7);
            }
            else
            {
                daysUntilMonday = ((int)DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7) % 7;
            }
            
            DateTime nextMonday = dateToday.AddDays(daysUntilMonday);


            PlayerData.GetInstance.AddEventLevelPickDayCheck(nextMonday);
        }
    }

    /// <summary>
    /// 일주일마다 한번씩 5개의 맵을 가져온다.
    /// </summary>
    public void WeeklyPickRandomNum()
    {
        DateTime pickedMonday = default;
        foreach (var Monday in PlayerData.GetInstance.GetEventLevelPickDay())
        {
            pickedMonday = Monday.Key.Date;
        }
        
        // if (day.DayOfWeek != DayOfWeek.Monday && !PlayerData.GetInstance.GetIsWeeklyOnePick())
        // {
        //     PlayerData.GetInstance.SaveIsWeeklyOnePick(true);
        // }

        //일주일에 한번만(월요일)
        if (pickedMonday <= day.Date)
        {
            if (PlayerData.GetInstance.GetEventLevelList().Count != 0) PlayerData.GetInstance.GetEventLevelList().Clear();

            for (int i = 0; i < PickRandomCount; i++) PickRandomNumber(i);
            
            EventLevelNum = 1;

            PlayerData.GetInstance.SaveEventLevelNumber(EventLevelNum);
            PlayerData.GetInstance.BeforeEventLevelNum = 0;
            //PlayerData.GetInstance.SaveIsWeeklyOnePick(false);
            PlayerData.GetInstance.SaveIsEventMapAllClear(false);


            PlayerData.GetInstance.ClearEventLevelPickDay();
            
            DateTime dateToday = DateTime.Today;
            int daysUntilMonday;
            
            if (day.DayOfWeek == DayOfWeek.Monday)
            {
                daysUntilMonday = ((int) DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7);
            }
            else
            {
                daysUntilMonday = ((int)DayOfWeek.Monday - (int)dateToday.DayOfWeek + 7) % 7;
            }
            
            DateTime nextMonday = dateToday.AddDays(daysUntilMonday);
            
            PlayerData.GetInstance.AddEventLevelPickDayCheck(nextMonday);

        }
    }

    /// <summary>
    /// 랜덤 번호 추가.
    /// </summary>
    private void PickRandomNumber(int _num)
    {
        PickRandomNum = Random.Range(0, DataContainer.GetInstance.ChallengeMapCount);

        //중복이면
        if (PlayerData.GetInstance.GetEventLevelList().Contains(PickRandomNum))
        {
            PickRandomNumber(_num);
        }
        else
        {
            //저장
            if (PlayerData.GetInstance != null)
            {
                PlayerData.GetInstance.SaveEventLevelData(_num, PickRandomNum);
            }
        }
        
    }
}