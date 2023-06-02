using CompleteProject;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyQuestEntity : MonoBehaviour
{
    public Text text_missionguide = null;
    public Text text_rewardCount = null;

    private DailyQuestData questData = null;

    [SerializeField] private Slider completeSlider = null;
    [SerializeField] private Text text_completeRate = null;

    [SerializeField] private GameObject completeDisplay = null;

    [SerializeField] private Image rewardImage = null;
    [SerializeField] private Image disableImage = null;

    [SerializeField] private GameObject buttonDisable;
    private DailyQuestPopup dailyQuestPopup = null;
    private int iSlotINDEX = 0;

    [SerializeField] private Image Item_IMG = null;
    [SerializeField] Animator GetItemAnim;

    private void Awake()
    {
        completeDisplay.SetActive(false);
        if (rewardImage != null)
        {
            rewardImage.gameObject.SetActive(true);
        }
         if(buttonDisable != null)
        {
            buttonDisable.gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        if (null != questData)
        {
            SetUI();
        }
    }

    public void SetQuestEntityData(DailyQuestData _qd, DailyQuestPopup _dailyQuestPopup, int _index)
    {
        questData = _qd;
        dailyQuestPopup = _dailyQuestPopup;
        iSlotINDEX = _index;

        Debug.LogWarningFormat("KKI GetReward {0}", string.Format("Quest_clear{0:00}_button", iSlotINDEX));

        SetUI();
    }

    public void SetUI()
    {
        int iCurrentCount = questData.questCount > questData.saveCount ? questData.saveCount : questData.questCount;
        string strMission = I2.Loc.LocalizationManager.GetTermTranslation($"DailyQuest_{questData.convertedQuestType.ToString()}");

        strMission = string.Format(strMission, questData.questCount.ToString("#,##0"));
        text_missionguide.text = strMission;

        text_rewardCount.text = questData.rewardCount.ToString("#,##0");
        completeSlider.value = (float)iCurrentCount / (float)questData.questCount;
        if (buttonDisable != null)
        {
            if (completeSlider.value >= 1)
            {
                //buttonDisable.SetActive(false);
            }
        }
        text_completeRate.text = string.Format($"{iCurrentCount} / {questData.questCount}");

        if (dailyQuestPopup.dicDailyQuestRewardSprite.ContainsKey(questData.convertedRewardType))
        {
            if (questData.convertedQuestType == EDailyQuestType.DAILYQUESTCLEARAD)
            {
                rewardImage.sprite = dailyQuestPopup.ADCoin;
            }
            else
            {
                rewardImage.sprite = dailyQuestPopup.dicDailyQuestRewardSprite[questData.convertedRewardType].spriteReward;
            }
        }
        else
        {
            Debug.LogWarningFormat("RewardType Doesn't Exist {0}", questData.convertedRewardType);
        }

        if (questData.saveCount >= questData.questCount)
        {
            if (disableImage != null)
            {
                disableImage.gameObject.SetActive(false);
            }
        }
        else
        {
            if (disableImage != null)
            {
                disableImage.gameObject.SetActive(true);
            }
        }

        if (questData.isGetReward == true)
        {
            completeDisplay.SetActive(true);
            if (rewardImage != null)
            {
                rewardImage.gameObject.SetActive(false);
            }
            if (buttonDisable != null)
            {
                buttonDisable.SetActive(false);
            }
        }
    }

    public void GetReward()
    {
        if (questData.saveCount >= questData.questCount)
        {
            if (buttonDisable != null)
            {
                buttonDisable.SetActive(true);
            }
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{iSlotINDEX}_Quest_clear"));

            SoundManager.GetInstance.Play("ClearStar");

            DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEAR, 1);

            GameObject gobTarget = GameObject.Find("Text_Gold");
            Vector3 vecTargetPosition = gobTarget.transform.position;
            switch (questData.convertedQuestType)
            {
                case EDailyQuestType.DAILYQUESTCLEAR:
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("All_Quest_Clear");
                    DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEARAD, 1);
                    ShowRewardPopup(vecTargetPosition);
                    break;

                case EDailyQuestType.DAILYQUESTCLEARAD:
                    ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
                    {
                        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("All_Quest_Clear");

                        GameObject gobTarget = GameObject.Find("Text_Gold");
                        Vector3 vecTargetPosition = gobTarget.transform.position;
                        questData.isGetReward = true;

                        DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEARAD, 1);
                        ShowRewardPopup(vecTargetPosition);
                    });
                    /*AD.ShowAd(ERewardedKind.GETITEM, () =>
                    {
                        GameObject gobTarget = GameObject.Find("Text_Gold");
                        Vector3 vecTargetPosition = gobTarget.transform.position;
                        questData.isGetReward = true;

                        DailyQuestManager.CollectMission(EDailyQuestType.DAILYQUESTCLEARAD, 1);
                        ShowRewardPopup(vecTargetPosition);
                    });*/
                    break;

                default:
                    ShowRewardPopup(vecTargetPosition);
                    break;
            }

            DailyQuestManager.Save();
        }
    }

    private void ShowRewardPopup(Vector3 _vecTargetPosition)
    {
        GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
        Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
        Sprite spReward = dailyQuestPopup.dicDailyQuestRewardSprite[questData.convertedRewardType].spriteReward;

        int iAmount = 1;
        int iValue = questData.rewardCount;
        switch (questData.convertedRewardType)
        {
            case EDailyQuestRewardType.COIN:
                iAmount = 0;
               
                var coin = PopupManager.instance.GetCoin(questData.rewardCount);
                coin.GetComponent<Animator>().SetTrigger("Normal");
                break;

            default:
                iAmount = 0;
                if (Item_IMG != null) Item_IMG.sprite = spReward;
                GetItemAnim.SetTrigger("GetItemrAnim_1");
                break;
        }
        string endSound = GetItem();
        popReward.ShowReward(spReward, iAmount, iValue, endSound, _vecTargetPosition);

    }

    private string GetItem()
    {
        questData.isGetReward = true;

        string r_endSound = "";
        switch (questData.convertedRewardType)
        {
            case EDailyQuestRewardType.COIN:
                PlayerData.GetInstance.Gold += questData.rewardCount;
                r_endSound = "GetCoin";
                break;

            case EDailyQuestRewardType.HAMMER:
                PlayerData.GetInstance.ItemHammer += questData.rewardCount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.BOMB:
                PlayerData.GetInstance.ItemBomb += questData.rewardCount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.COLOR:
                PlayerData.GetInstance.ItemColor += questData.rewardCount;
                r_endSound = "StarBoxGetItem";
                break;

            case EDailyQuestRewardType.HAMMERBOMB:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemBomb += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.HAMMERCOLOR:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.BOMBCOLOR:
                PlayerData.GetInstance.ItemBomb += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "BlockCreateItem";
                break;

            case EDailyQuestRewardType.ALLITEM:
                PlayerData.GetInstance.ItemHammer += 1;
                PlayerData.GetInstance.ItemBomb += 1;
                PlayerData.GetInstance.ItemColor += 1;
                r_endSound = "ClearStar";
                break;
        }

        var popupManager = GameObject.Find("PopupManager").GetComponent<PopupManager>();
        if (popupManager != null)
        {
            popupManager.GoldRefresh();
        }

        dailyQuestPopup.RefreshEntity();

        return r_endSound;
    }
}