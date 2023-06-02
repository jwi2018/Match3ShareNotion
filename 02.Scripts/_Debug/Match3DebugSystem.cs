using Opencoding.CommandHandlerSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Match3DebugSystem : MonoBehaviour
{
    public static Match3DebugSystem instance = null;

    public EID touchChangeEID = EID.NORMAL;
    public bool isChangeEID = false;

    private string Palindrom = "";

    public int nextDay = 1;

    private void Start()
    {
        instance = this;
        isChangeEID = false;
    }

    [ContextMenu("AddGold")]
    public void AddGold()
    {
        //Debug.LogWarningFormat("KKI{0}", findBlockPop(5, 5));
        //pick();
        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            //popupManager.OnClickStarChest();
            //popupManager.OnClickRoulette(true);
            //popupManager.OnClickRateUs();
            //popupManager.OnClickStarChest();
            //popupManager.ShowNoAdsPopup();
            //popupManager.ShowEventLevelClear();

            var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Clear, PopupManager.instance.transform);
        }
        //PlayerData.GetInstance.Gold += 20000;
    }

    [ContextMenu("AddAcorn")]
    public void AddAcorn()
    {
        PlayerData.GetInstance.Acorn += 10;
    }

    [ContextMenu("ResetStarBoxOpenCount")]
    public void TestNEXTContinueDay()
    {
        PlayerData.GetInstance.StarBoxOpenCount = 0;
        //PlayerData.GetInstance.DailyMonthRewardContinueDay = (StaticGameSettings.TestPlusDay + nextDay) - 1;
        StaticGameSettings.TestPlusDay += nextDay;
        var obj = Instantiate(PopupList.GetInstance.Popup_WeeklyBonus, PopupManager.instance.transform);
    }

    [ContextMenu("IncreaseAcorn")]
    public void IncreaseAcorn()
    {
        PlayerData.GetInstance.Acorn += 101;
    }

    private Boolean isPalindrom(string s)
    {
        int m_length = s.Length;

        for (int i = 0; i < m_length / 2; i++)
        {
            if (s.Substring(i, 1) != s.Substring(m_length - i - 1, 1))
                return false;
        }

        return true;
    }

    private void pick()
    {
        string[] m_card = new string[] { "A", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        int m_select = 0;
        List<string> m_list = new List<string>();

        while (m_list.Count < 5)
        {
            m_select = (int)(UnityEngine.Random.value * m_card.Length);
            if (m_select == m_card.Length)
                continue;
            m_list.Add(m_card[m_select]);
            m_list = m_list.Distinct().ToList();
        }

        foreach (string str in m_list)
        {
            Debug.LogWarningFormat("KKI :: {0}", str);
        }
    }

    private int findBlockPop(int n, int m)
    {
        int[,] array = new int[,]
    {
        { 1, 3, 2, 1, 1},
        { 1, 2, 4, 4, 4},
        { 3, 2, 4, 4, 5},
        { 1, 1, 4, 4, 4},
        { 1, 2, 3, 1, 5}
    };

        List<int> m_list = new List<int>();

        for (int i = 0; i < m; i++)
        {
            for (int j = 0; j < n - 2; j++)
            {
                if (array[i, j] == array[i, j + 1] && array[i, j] == array[i, j + 2])
                {
                    m_list.Add((i * 10) + j);
                    m_list.Add((i * 10) + j + 1);
                    m_list.Add((i * 10) + j + 2);
                }
            }
        }

        for (int i = 0; i < m - 2; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (array[i, j] == array[i + 1, j] && array[i, j] == array[i + 2, j])
                {
                    m_list.Add((i * 10) + j);
                    m_list.Add(((i + 1) * 10) + j);
                    m_list.Add(((i + 2) * 10) + j);
                }
            }
        }

        m_list = m_list.Distinct().ToList();

        return m_list.Count;
    }

    private void findBlockPop()
    {
        //int[,] arr = new int[n, m];
        int[,] arr = new int[,]
    {
        { 1, 3, 2, 1, 1},
        { 1, 2, 1, 4, 2},
        { 3, 2, 3, 4, 5},
        { 1, 1, 4, 4, 4},
        { 1, 2, 3, 1, 5}
    };
        int row = 5, col = 5;
        int sequence = 3;
        int total = 0;
        int count = 0;
        int value = -1;

        for (int i = 0; i < row; i++)
        {
            value = arr[i, 0];
            for (int j = 0; j < col; j++)
            {
                if (value != arr[i, j])
                {
                    if (count >= sequence)
                    {
                        total += count;
                    }
                    value = arr[i, j];
                    count = 0;
                }
                if (value == arr[i, j])
                {
                    count++;
                }
            }
            if (count >= sequence)
            {
                total += count;
            }
            count = 0;
        }

        for (int j = 0; j < col; j++)
        {
            value = arr[0, j];
            for (int i = 0; i < row; i++)
            {
                if (value != arr[i, j])
                {
                    if (count >= sequence)
                    {
                        total += count;
                    }
                    value = arr[i, j];
                    count = 0;
                }
                if (value == arr[i, j])
                {
                    count++;
                }
            }
            if (count >= sequence)
            {
                total += count;
            }
            count = 0;
        }
        Debug.LogWarningFormat("KKI : {0}", total);
    }

    [ContextMenu("ResetStarBoxOpenCount")]
    public void ResetStarBoxOpenCount()
    {
        PlayerData.GetInstance.StarBoxOpenCount = 0;
    }

    private void Awake()
    {
        CommandHandlers.RegisterCommandHandlers(this);
    }

    private void OnDestroy()
    {
        CommandHandlers.UnregisterCommandHandlers(this);
    }

    [CommandHandler]
    private void ChangeEID([Autocomplete(typeof(EID), "EIDAutocomplete")] string eidName)
    {
        EID outedEID = EID.NONE;
        Enum.TryParse<EID>(eidName, out outedEID);
        touchChangeEID = outedEID;
        if (outedEID == EID.NONE)
        {
            isChangeEID = false;
        }
        else
        {
            isChangeEID = true;
        }
    }

    public IEnumerable<string> EIDAutocomplete()
    {
        return new[] { EID.NONE.ToString(), EID.COLOR_BOMB.ToString(), EID.RHOMBUS.ToString(), EID.X.ToString(), EID.VERTICAL.ToString(), EID.FISH.ToString() };
    }

    public void SetBlock(NormalBlock _block)
    {
#if UNITY_EDITOR

        if (isChangeEID == true)
        {
            if (DoubleClickSystem.GetInstance != null)
            {
                if (DoubleClickSystem.GetInstance.GetBlockList().Contains(touchChangeEID))
                {
                    _block.SetMadeColor(_block.Color);
                    _block.Setting(EColor.NONE, touchChangeEID);
                    _block.ApplySprite();
                }
                else
                {
                    _block.Setting(_block.Color, touchChangeEID);
                    _block.ApplySprite();
                }
            }
            else
            {
                _block.Setting(_block.Color, touchChangeEID);
                _block.ApplySprite();
            }

            BlockManager.GetInstance.SetPrefabBlock(_block, touchChangeEID);
        }
#endif
    }
}