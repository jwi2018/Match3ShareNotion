using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DailyQuestData
{
    public int day;
    public string questType;
    public int questCount;
    public string rewardtype;
    public int rewardCount;

    public EDailyQuestType convertedQuestType;
    public EDailyQuestRewardType convertedRewardType;
    public int saveCount;
    public bool isGetReward = false;
}

[Serializable]
public class DailyQuestDataList
{
    public DailyQuestData[] dailyQuestDatas;
}

public class DailyQuestDataList_Wrap
{
    public List<DailyQuestData> dailyQuestDatas = new List<DailyQuestData>();
}

public class DailyQuestSaveData
{
    public int day;
    public string date;
    public List<DailyQuestData> dailyQuestSaveDatas = new List<DailyQuestData>();
}

public class DailyQuestManager : Singleton<DailyQuestManager>
{
    private Dictionary<int, DailyQuestDataList_Wrap> _dailyQuestData = new Dictionary<int, DailyQuestDataList_Wrap>();
    //private Dictionary<int, DailyQuestData> _dailyQuestSaveData = new Dictionary<int, DailyQuestData>();

    public DailyQuestSaveData dailyQuestCurrentData = new DailyQuestSaveData();
    private string saveKey = "TseuqYliad";

    private int testDayPlus = 0;

    private void Awake()
    {
        InitData();
    }

    [ContextMenu("SaveTest")]
    public void SaveData()
    {
        string jsonData = JsonUtility.ToJson(dailyQuestCurrentData);
        PlayerPrefs.SetString(saveKey, jsonData);
    }

    [ContextMenu("LoadSaveData")]
    public void LoadSaveData()
    {
        string loadData = PlayerPrefs.GetString(saveKey);
        DailyQuestSaveData loadedData = JsonUtility.FromJson<DailyQuestSaveData>(loadData);

        if (loadedData == null)
        {
            Debug.LogWarningFormat("KKI 1");
            dailyQuestCurrentData.day = 1;
            dailyQuestCurrentData.date = DateTime.Now.ToString();
            dailyQuestCurrentData.dailyQuestSaveDatas = _dailyQuestData[1].dailyQuestDatas;
        }
        else
        {
            DateTime savedDate;
            DateTime.TryParse(loadedData.date, out savedDate);
            if (savedDate.Day + testDayPlus != DateTime.Now.Day)
            {
                int nextDay = loadedData.day + 1;
                if (_dailyQuestData.ContainsKey(nextDay))
                {
                    Debug.LogWarningFormat("KKI 2");
                    dailyQuestCurrentData.day = nextDay;
                    dailyQuestCurrentData.date = DateTime.Now.ToString();
                    dailyQuestCurrentData.dailyQuestSaveDatas = _dailyQuestData[nextDay].dailyQuestDatas;
                }
                else
                {
                    Debug.LogWarningFormat("KKI 3");
                    dailyQuestCurrentData.day = 1;
                    dailyQuestCurrentData.date = DateTime.Now.ToString();
                    dailyQuestCurrentData.dailyQuestSaveDatas = _dailyQuestData[1].dailyQuestDatas;
                }
                SaveData();
            }
            else
            {
                Debug.LogWarningFormat("KKI 4", loadedData.day);
                dailyQuestCurrentData = loadedData;
                if (loadedData.day == 0)
                {
                    dailyQuestCurrentData.day = 1;
                    dailyQuestCurrentData.date = DateTime.Now.ToString();
                    dailyQuestCurrentData.dailyQuestSaveDatas = _dailyQuestData[1].dailyQuestDatas;
                }
            }
        }

        foreach (DailyQuestData qData in dailyQuestCurrentData.dailyQuestSaveDatas)
        {
            ConvertType(qData);
        }
    }

    public static void CheckToday()
    {
        //startDay = DateTime.Parse(PlayerData.GetInstance.LoadGameString("StartDay"));

        //for (var i = 1; i < 8; i++) // 1~7일차 비교
        //    if (DateTime.Today == startDay.AddDays(i)) // (오늘 == startDay + i)
        //        today = i;
    }

    public void InitData()
    {
        LoadDailyQuestData();
        LoadSaveData();
    }

    [ContextMenu("NextDay")]
    public void NextDay()
    {
        testDayPlus += 1;
        LoadSaveData();
    }

    private void ConvertType(DailyQuestData qData)
    {
        qData.convertedQuestType = (EDailyQuestType)Enum.Parse(typeof(EDailyQuestType), qData.questType);
        qData.convertedRewardType = (EDailyQuestRewardType)Enum.Parse(typeof(EDailyQuestRewardType), qData.rewardtype);
    }

    [ContextMenu("LoadDailyQuestData")]
    public void LoadDailyQuestData()
    {
        var dailyQuestList = ResourceLoader<DailyQuestDataList>.LoadResource("DailyQuestData");
        for (var i = 0; i < dailyQuestList.dailyQuestDatas.Length; i++)
        {
            ConvertType(dailyQuestList.dailyQuestDatas[i]);
            if (!_dailyQuestData.ContainsKey(dailyQuestList.dailyQuestDatas[i].day))
            {
                DailyQuestDataList_Wrap questData = new DailyQuestDataList_Wrap();

                questData.dailyQuestDatas.Add(dailyQuestList.dailyQuestDatas[i]);
                _dailyQuestData.Add(dailyQuestList.dailyQuestDatas[i].day, questData);
            }
            else
            {
                _dailyQuestData[dailyQuestList.dailyQuestDatas[i].day].dailyQuestDatas.Add(dailyQuestList.dailyQuestDatas[i]);
            }
        }
        Debug.LogWarningFormat("KKI{0}", this);
    }

    public static void CollectMission(EDailyQuestType questType, int amount)
    {
        if (amount < 0)
        {
            Debug.LogWarningFormat("KKI Plus CollectMission {0} :: {1}", questType, amount);
            return;
        }
        if (null != GetInstance)
        {
            foreach (DailyQuestData dailyqd in GetInstance.dailyQuestCurrentData.dailyQuestSaveDatas)
            {
                if (dailyqd.convertedQuestType == questType)
                {
                    dailyqd.saveCount += amount;
                    if (dailyqd.saveCount == dailyqd.questCount)
                    {
                        FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{dailyqd.questType}{dailyqd.questCount}mission_clear"));
                        Debug.LogWarningFormat($"{dailyqd.questType}{dailyqd.questCount}mission_clear");
                    }

                    //Debug.LogWarningFormat($"KKI CollectMission : {dailyqd.convertedQuestType}, {dailyqd.saveCount}");
                }
            }
        }
    }

    public static void Save()
    {
        if (null != GetInstance)
        {
            GetInstance.SaveData();
        }
    }

    public static void LoadData()
    {
        if (null != GetInstance)
        {
            GetInstance.LoadSaveData();
        }
    }
}