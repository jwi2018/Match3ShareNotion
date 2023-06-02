using System;
using System.Collections.Generic;
using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct DailyQuestRewardSprite
{
    public EDailyQuestRewardType eRewarditem;
    public Sprite spriteReward;
}

public class DailyQuestPopup : PopupSetting
{
    public GameObject gobDummy;
    public Transform trSlotParent;
    public Text title = null;

    [SerializeField] private List<DailyQuestRewardSprite> dailyQuestRewardSprite = new List<DailyQuestRewardSprite>();

    public Dictionary<EDailyQuestRewardType, DailyQuestRewardSprite> dicDailyQuestRewardSprite = new Dictionary<EDailyQuestRewardType, DailyQuestRewardSprite>();

    private List<DailyQuestEntity> loadedQuestEntity = new List<DailyQuestEntity>();

    [SerializeField] private EDailyQuestType testQuestType = EDailyQuestType.GAMECLEAR;
    [SerializeField] private int testDailyQuestAmount = 1;

    [SerializeField] private DailyQuestEntity questEntityDAILYQUESTCLEAR = null;        // New 시스템 후르츠 가든

    public Sprite ADCoin = null;

    private void Awake()
    {
        gobDummy.SetActive(false);
        dicDailyQuestRewardSprite = dailyQuestRewardSprite.ToDictionary(t => t.eRewarditem, t => t);
    }

    private void Start()
    {
        OnPopupSetting();
        InitUI();
    }

    [ContextMenu("SetTESTQuest")]
    public void SetTESTQuest()
    {
        DailyQuestManager.CollectMission(testQuestType, testDailyQuestAmount);
        RefreshEntity();
    }

    public void InitUI()
    {
        DailyQuestSaveData currentDayData = DailyQuestManager.GetInstance.dailyQuestCurrentData;

        string strDailyDay = I2.Loc.LocalizationManager.GetTermTranslation("DailyDay");
        if (null != title)
        {
            title.text = string.Format(strDailyDay, currentDayData.day);
        }

        loadedQuestEntity.Clear();

        int iDailyQuestClear = 0;
        int currentQuestClear = 0;
        foreach (DailyQuestData saveDAta in currentDayData.dailyQuestSaveDatas)
        {
            switch (saveDAta.convertedQuestType)
            {
                case EDailyQuestType.DAILYQUESTCLEAR:
                    {
                        if (true == saveDAta.isGetReward)
                        {
                            DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEARAD, 1);
                        }
                    }
                    break;

                case EDailyQuestType.DAILYQUESTCLEARAD:
                    break;

                default:
                    iDailyQuestClear++;
                    if (true == saveDAta.isGetReward)
                    {
                        currentQuestClear++;
                    }

                    break;
            }
        }

        int index = 1;
        foreach (DailyQuestData saveDAta in currentDayData.dailyQuestSaveDatas)
        {
            bool isNewInstanciate = true;
            switch (saveDAta.convertedQuestType)
            {
                case EDailyQuestType.DAILYQUESTCLEAR:
                    {
                        saveDAta.questCount = iDailyQuestClear;
                        if (questEntityDAILYQUESTCLEAR != null)
                        {
                            loadedQuestEntity.Add(questEntityDAILYQUESTCLEAR);
                            questEntityDAILYQUESTCLEAR.SetQuestEntityData(saveDAta, this, index);
                            isNewInstanciate = false;
                        }
                    }
                    break;
            }

            if (isNewInstanciate == true)
            {
                var loadedDailyMission = GameObject.Instantiate(gobDummy, trSlotParent);
                DailyQuestEntity dqe = loadedDailyMission.GetComponent<DailyQuestEntity>();
                loadedDailyMission.SetActive(true);
                dqe.SetQuestEntityData(saveDAta, this, index);
                loadedQuestEntity.Add(dqe);
            }
            index++;
        }

        if (currentQuestClear >= iDailyQuestClear)
        {
            DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEAR, currentQuestClear);
        }
    }

    public void RefreshEntity()
    {
        foreach (DailyQuestEntity saveDAta in loadedQuestEntity)
        {
            saveDAta.SetUI();
        }
    }

    public override void OnPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("missionquest_enter");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public override void OffPopupSetting()
    {
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void AdsFree()
    {
    }

    public void ViewMore(bool isbool = true)
    {
    }

    public void RestorePurchases()
    {
        Purchaser.GetInstance.RestorePurchases();
    }

    public void ChangeShopList(EShopKind kind)
    {
    }
}