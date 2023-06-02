using System;
using System.Collections.Generic;
using UnityEngine;

public static class WeeklyRetentionIndicator
{
    [Tooltip("시작한 날")] private static DateTime startDay;
    [Tooltip("D+1~7일")] private static int today;
    [Tooltip("리워드 동영상 광고 시청 횟수")] public static int showRewardAdCount = 0;
    [Tooltip("전면 동영상 광고 시청 횟수")] public static int showInsAdCount = 0;

    /// <summary>
    ///     유저가 게임을 시작한 날짜를 기록
    /// </summary>
    public static void SetStartDate()
    {
        if (PlayerData.GetInstance.LoadGameString("StartDay") == "") // 기존에 저장된 데이터가 없을 시
        {
            startDay = DateTime.Today;
            PlayerData.GetInstance.SaveGameString("StartDay", startDay.ToString());
        }
    }

    /// <summary>
    ///     유저가 처음 접속한 날짜와 오늘 날짜를 비교
    /// </summary>
    public static void CheckToday()
    {
        startDay = DateTime.Parse(PlayerData.GetInstance.LoadGameString("StartDay"));

        for (var i = 1; i < 8; i++) // 1~7일차 비교
            if (DateTime.Today == startDay.AddDays(i)) // (오늘 == startDay + i) 
                today = i;
    }

    /// <summary>
    ///     트래킹 코드
    /// </summary>
    /// <param name="value">트래킹에 필요한 매개변수 값</param>
    public static void Tracking(string value)
    {
        var paramater = new Dictionary<string, string>();
        paramater.Add("D_" + today, value);
        FirebaseManager.GetInstance.FirebaseLogEvent("Arrival_Stage", paramater);

        if (today == 3) FirebaseManager.GetInstance.FirebaseLogEvent("Day3_Ret");

        if (today == 7) FirebaseManager.GetInstance.FirebaseLogEvent("Day7_Ret");
    }

    public static void StageClearTracking(int value)
    {
        if (value == 60) FirebaseManager.GetInstance.FirebaseLogEvent("Clear_60stage");
        if (value == 100) FirebaseManager.GetInstance.FirebaseLogEvent("Clear_100stage");
        if (value == 150) FirebaseManager.GetInstance.FirebaseLogEvent("Clear_150stage");
    }

    public static void DailyRewordShowAdTracking()
    {
        if (showRewardAdCount == 4) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Reward_4");
        if (showRewardAdCount == 6) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Reward_6");
        if (showRewardAdCount == 8) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Reward_8");
        if (showRewardAdCount == 10) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Reward_10");
    }

    public static void DailyInterstitialShowAdTracking()
    {
        if (showRewardAdCount == 8) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Ins_8");
        if (showRewardAdCount == 13) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Ins_13");
        if (showRewardAdCount == 18) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Ins_18");
        if (showRewardAdCount == 25) FirebaseManager.GetInstance.FirebaseLogEvent("Daily_Ins_25");
    }
}