using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Coffee.UIEffects;
using System;
using System.Linq;

[System.Serializable]
public class FreeGoldEntity
{
    public EDailyQuestRewardType freeGoldReward;
    public int rewardAmount;
}

public class Pop_FreeGold : PopupSetting
{
    [SerializeField] private List<FreeGoldEntity> rewardEntity = new List<FreeGoldEntity>();

    [SerializeField] private Slider sliderSavingGauge = null;
    [SerializeField] private Text txComplete = null;
    [SerializeField] private Button btnADWatch = null;

    [SerializeField] private GameObject gobReward1 = null;

    [SerializeField] private List<Text> listRewardText = new List<Text>();
    [SerializeField] private Text txReward1 = null;
    [SerializeField] private GameObject gobReward2 = null;
    [SerializeField] private GameObject gobReward2OpenDisable = null;

    [SerializeField] private Text txFreeGoldCooltimer = null;
    [SerializeField] private Text txFreeGoldExplain = null;
    [SerializeField] private GameObject Home_btn = null;

    private Vector3 vecTargetPosition;

    private FreeGoldStatus linkedFreegoldStatus = null;

    [ContextMenu("SetNextTestValue")]
    public void SetNextTestValue()
    {
        PlayerData.GetInstance.SavingCoin += 1000;
        OnPopupSetting();
    }

    private void Start()
    {
        OnPopupSetting();
    }

    private void DataSetup()
    {
        DateTime savedDate;

        DateTime.TryParse(PlayerData.GetInstance.FreeGoldDate, out savedDate);

        bool isEqualDay = savedDate.CompareDateTimes(DateTime.Now);
        if (isEqualDay == false)
        {
            PlayerData.GetInstance.FreeGoldCount = 0;
            PlayerData.GetInstance.FreeGoldDate = DateTime.Now.ToString();
        }

        GameObject gobTarget = GameObject.Find("Text_Gold");
        vecTargetPosition = gobTarget.transform.position;
    }

    private void UISetup()
    {
        sliderSavingGauge.maxValue = 4;
        sliderSavingGauge.value = PlayerData.GetInstance.FreeGoldCount;

        txComplete.text = string.Format($"{PlayerData.GetInstance.FreeGoldCount} / 4");
        
        if (PlayerData.GetInstance.FreeGoldCount < 4)
        {
            txFreeGoldExplain.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("FreeCoinExplain"), 4 - PlayerData.GetInstance.FreeGoldCount);
        }
        else
        {
            if (BaseSystem.GetInstance != null)
            {
                if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
                {
                    txFreeGoldExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("FreeGoldComplete_2");
                }
                else
                {
                    txFreeGoldExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("FreeGoldComplete");
                }
            }
            else
            {
                txFreeGoldExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("FreeGoldComplete");
            }
            //txFreeGoldExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("FreeGoldComplete_2");
            if(Home_btn != null) Home_btn.SetActive(true);
            if(gobReward1 != null) gobReward1.SetActive(false);
        }


        if (null != txReward1)
        {
            txReward1.text = rewardEntity[PlayerData.GetInstance.FreeGoldCount].rewardAmount.ToString("#,##");
        }
        else
        {
            for (int i = 0; i < listRewardText.Count; i++)
            {
                listRewardText[i].text = rewardEntity[i].rewardAmount.ToString("#,##");

                if (PlayerData.GetInstance.FreeGoldCount >= (i+1))
                {
                    listRewardText[i].gameObject.SetActive(false);   
                }
                else listRewardText[i].gameObject.SetActive(true);
            }
        }

        if(gobReward2 != null)
        {
            if (PlayerData.GetInstance.FreeGoldCount == rewardEntity.Count - 2)
            {
                if (gobReward2.gameObject.activeSelf == false)
                {
                    gobReward2.gameObject.SetActive(true);
                    gobReward1.gameObject.SetActive(false);
                }
                StaticScript.SetActiveCheckNULL(gobReward2OpenDisable, false);
            }
            else
            {
                if (gobReward2.gameObject.activeSelf == true)
                {
                    gobReward2.gameObject.SetActive(false);
                    gobReward1.gameObject.SetActive(true);
                }
                StaticScript.SetActiveCheckNULL(gobReward2OpenDisable, true);
            }
        }
    }

    public void SetPopup(FreeGoldStatus _freegoldStatus)
    {
        linkedFreegoldStatus = _freegoldStatus;
        DataSetup();
        UISetup();
        StartCoroutine(CooltimeCounter());
    }

    private IEnumerator CooltimeCounter()
    {
        while (true)
        {
            if (linkedFreegoldStatus.realtimecoolTime > 0 && PlayerData.GetInstance.FreeGoldCount.Equals(4))
            {
                TimeSpan ts = TimeSpan.FromSeconds(linkedFreegoldStatus.realtimecoolTime);
                txFreeGoldCooltimer.text = ts.ToString("hh':'mm':'ss");
                gobReward2OpenDisable.SetActive(false);
           
                if (btnADWatch.gameObject.activeSelf == true)
                {
                    btnADWatch.gameObject.SetActive(false);
                }
                
            }
            else
            {
                if (btnADWatch.gameObject.activeSelf == false)
                {
                    btnADWatch.gameObject.SetActive(true);
                }
                txFreeGoldCooltimer.text = "";
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public override void OnPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Intro_coinreward_enter");
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("GetCoin");
        }
    }

    public override void OffPopupSetting()
    {
        this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("Popup");
        }
        this.GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("ButtonPush");
        }
    }

    public void BtnShowAD()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            switch (PlayerData.GetInstance.FreeGoldCount)
            {
                case 0:
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WM_reward_01");
                    break;
                case 1:
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WM_reward_02");
                    break;
                case 2:
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WM_reward_03");
                    break;
                case 3:
                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WM_reward_04");
                    break;
            }
            PlayerData.GetInstance.WorldMapYear = System.DateTime.Now.Year;
            PlayerData.GetInstance.WorldMapMonth = System.DateTime.Now.Month;
            PlayerData.GetInstance.WorldMapDay = System.DateTime.Now.Day;
            PlayerData.GetInstance.WorldMapHour = System.DateTime.Now.Hour;
            PlayerData.GetInstance.WorldMapMinute = System.DateTime.Now.Minute;
            PlayerData.GetInstance.WorldMapSecond = System.DateTime.Now.Second;

            GetReward(PlayerData.GetInstance.FreeGoldCount);
            UISetup();
        });
        /*AD.ShowAd(ERewardedKind.WORLDMAPCOIN, () =>
        {
            PlayerData.GetInstance.WorldMapYear = System.DateTime.Now.Year;
            PlayerData.GetInstance.WorldMapMonth = System.DateTime.Now.Month;
            PlayerData.GetInstance.WorldMapDay = System.DateTime.Now.Day;
            PlayerData.GetInstance.WorldMapHour = System.DateTime.Now.Hour;
            PlayerData.GetInstance.WorldMapMinute = System.DateTime.Now.Minute;
            PlayerData.GetInstance.WorldMapSecond = System.DateTime.Now.Second;

            GetReward(PlayerData.GetInstance.FreeGoldCount);
            UISetup();
        });*/
    }

    public void GetReward(int next)
    {
        switch (next)
        {
            case 0:
            case 1:
            case 2:
            case 3:
                GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
                Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
                popReward.GiveReward(EDailyQuestRewardType.COIN, rewardEntity[next].rewardAmount);
                popReward.ShowReward(EDailyQuestRewardType.COIN, 0, rewardEntity[next].rewardAmount, "GetCoin", vecTargetPosition);

                var coin = PopupManager.instance.GetCoin();
                coin.GetComponent<Animator>().SetTrigger("Normal");

                if (next == rewardEntity.Count - 2)
                {
                    Debug.LogWarningFormat("KKI next == rewardEntity.Count - 1 :: {0} :: {1}", next, rewardEntity.Count);
                    linkedFreegoldStatus.CoolTime = 60 * 20;
                    PlayerData.GetInstance.FreeGoldCooltime = 60 * 20;
                    
                    
                    StartCoroutine(CoLastReward(2.5f));
                }
                else
                {
                    linkedFreegoldStatus.CoolTime = 0;
                    PlayerData.GetInstance.FreeGoldCooltime = 0;
                }
                break;
        }
        PlayerData.GetInstance.FreeGoldCount++;
        
        // if (PlayerData.GetInstance.FreeGoldCount > 3)
        // {
        //     PlayerData.GetInstance.FreeGoldCount = 0;
        // }
    }

    private IEnumerator CoLastReward(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        int randomItem = UnityEngine.Random.Range(1, 4);
        EDailyQuestRewardType ranRewardType = (EDailyQuestRewardType)randomItem;
        GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
        Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
        string endSound = popReward.GiveReward(ranRewardType, 1);
        popReward.ShowReward(ranRewardType, 1, 1, endSound, vecTargetPosition);
    }
    
}