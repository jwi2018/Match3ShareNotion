using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemValue
{
    public int Level;
    public string Item;
    public string Item_1;
    public int Count;
    public int Count_1;

    public int itemCount=1;
}

[Serializable]
public class ItemValueList
{
    public ItemValue[] ItemData;
    public ItemValue[] ItemPrimiumData;
}


public class PrimiumTicketSystem : Singleton<PrimiumTicketSystem>
{
    public static int MAXLEVEL= 30;
    private const int MAXDAY = 15;
    public bool IsLevelup = false;
    private int getGameClearStarCount;
    public int RemainDay;
    //0,30 세트
    public Dictionary<int, ItemValue> ItemValue = new Dictionary<int, ItemValue>();
 

    public int GETGAMECLEARSTARCOUNT
    {
        get => getGameClearStarCount;
        set { getGameClearStarCount = value; }
    }

    public int LEVEL
    {
        get => PlayerData.GetInstance.PrimiumTicketLevel;
        set { PlayerData.GetInstance.PrimiumTicketLevel = value; }
    }

    public int RECEIVELEVEL_PRIMIUM
    {
        get => PlayerData.GetInstance.PrimiumTicketReceiveLevel_primium;
        set { PlayerData.GetInstance.PrimiumTicketReceiveLevel_primium = value; }
    }

    public int RECEIVELEVEL_FREE
    {
        get => PlayerData.GetInstance.PrimiumTicketReceiveLevel_free;
        set { PlayerData.GetInstance.PrimiumTicketReceiveLevel_free = value; }
    }

    public int PRIMIUMTICKETSTAR
    {
        get => PlayerData.GetInstance.PrimiumTicketStar;
        set { PlayerData.GetInstance.PrimiumTicketStar = value; }
    }

    public bool ISBUYPRIMIUMTICKET
    {
        get => PlayerData.GetInstance.IsBuyPrimiumTicket;
        set { PlayerData.GetInstance.IsBuyPrimiumTicket = value; }
    }

    public void Init()
    {

       
        if (PlayerData.GetInstance != null)
        {
            //처음 값 넣어줌
            if (PlayerData.GetInstance.PrimiumTicketRewardDate == DateTime.MinValue)
            {
                PlayerData.GetInstance.PrimiumTicketRewardDate = DateTime.Now;
                RemainDay = 15;
            }
            else
            {
                var date_1 = PlayerData.GetInstance.PrimiumTicketRewardDate;
                var date_2 = DateTime.Now;
                
                var Day = date_2 - date_1;
                RemainDay = MAXDAY - Day.Days;
                if (Day.Days >= 15)
                {
                    LEVEL = 0;
                    PRIMIUMTICKETSTAR = 0;
                    RECEIVELEVEL_PRIMIUM = 0;
                    RECEIVELEVEL_FREE = 0;
                    ISBUYPRIMIUMTICKET = false;
                    PlayerData.GetInstance.PrimiumTicketRewardDate = DateTime.Now;
                    RemainDay = MAXDAY;
                }
                
            }


            if (PlayerData.GetInstance.PrimiumTicketLevel.Equals(0)) PlayerData.GetInstance.PrimiumTicketLevel = 1;
            //PlayerData.GetInstance.IsBuyPrimiumTicket = true;

        }

        //여기서 ItemValue 세팅
        var items = ResourceLoader<ItemValueList>.LoadResource("PrimiumTicketData");

        //무료아이템 저장
        for (int i = 0; i < items.ItemData.Length; i++)
        {
            var level = items.ItemData[i].Level;


            var LevelItemvalue = new ItemValue();
            LevelItemvalue.Level = level;
            LevelItemvalue.Item = items.ItemData[i].Item;

            if (items.ItemData[i].Item_1 != null)
            {
                LevelItemvalue.Item_1 = items.ItemData[i].Item_1;
                items.ItemData[i].itemCount++;
            }
            
            LevelItemvalue.Count = items.ItemData[i].Count;
            LevelItemvalue.Count_1 = items.ItemData[i].Count_1;
            LevelItemvalue.itemCount = items.ItemData[i].itemCount;


            if (!ItemValue.ContainsKey(i)) ItemValue.Add(i, LevelItemvalue);
        }

        // 프리미엄 아이템 저장
        for (int i = 0; i < items.ItemPrimiumData.Length; i++)
        {
            var level = items.ItemPrimiumData[i].Level;


            var LevelItemvalue = new ItemValue();
            LevelItemvalue.Level = level;
            LevelItemvalue.Item = items.ItemPrimiumData[i].Item;

            if (items.ItemPrimiumData[i].Item_1 != null)
            {
                LevelItemvalue.Item_1 = items.ItemPrimiumData[i].Item_1;
                items.ItemPrimiumData[i].itemCount++;
            }

            LevelItemvalue.Count = items.ItemPrimiumData[i].Count;
            LevelItemvalue.Count_1 = items.ItemPrimiumData[i].Count_1;
            LevelItemvalue.itemCount = items.ItemPrimiumData[i].itemCount;

            if (!ItemValue.ContainsKey(i + MAXLEVEL)) ItemValue.Add(i + MAXLEVEL, LevelItemvalue);
        }
    }
}
