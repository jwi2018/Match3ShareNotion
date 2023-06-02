using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class ChallengeStage_ClearPopup : PopupSetting, ICoroutineAnimationController
{
    [SerializeField] private Text continueStageButtonText;
    [SerializeField] private GameObject _rewardItemList;
    [SerializeField] private List<Image> RewardImage = new List<Image>();
    [SerializeField] private Localize challengeText;
    [SerializeField] private Localize challengeText_NextButton;
    public bool IsNext { get; set; }
    private bool GetButton;
    private int nextSceneValue;
    private GameObject tempReward;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        var NextStage = StageManager.StageNumber + 1;
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_Hardmode_level_clear"));
        continueStageButtonText.text = "\" " + NextStage + " \"";
        challengeText.SetTerm("Challenge_Clear");
        challengeText_NextButton.SetTerm("Challenge_NextButton");
        ChallengeRewardItemList();
        SaveDataChallengeItem();
        StageManager.GetInstance.SetSkipText(false);
        BlockManager.GetInstance.SetBlockCollider(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearChallengePopup");
    }

    public override void OffPopupSetting()
    {
        OnClickGoMain();
        BlockManager.GetInstance.SetBlockCollider(true);
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }
    
    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }
    
    public void OnClickGoMain()
    {
        if (!GetButton)
        {
            if (PlayerData.GetInstance.IsRateUs)
            {
                GetButton = true;
            }
            else
            {
                GetButton = false;
            }
            nextSceneValue = 0;
            StartCoroutine(MissionClearCoroutine());
        }
    }
    
    public void OnClickNextStage()
    {
        if (!GetButton)
        {
            GetButton = true;

            //ADManager.GetInstance.ShowCycleInterstitial();
            StageManager.StageNumber++;
            nextSceneValue = 2;
            ChallengeSystem.GetInstance.IsChallengeStage = false;
            StartCoroutine(MissionClearCoroutine());
        }
    }
    
    public GameObject CreateChallengeRewardItem()
    {
        tempReward = PopupManager.instance.ShowChallengeRewardPopup();
        tempReward.transform.parent = _rewardItemList.transform;
        tempReward.transform.localScale = new Vector3(1f, 1f, 1f);
        return tempReward;
    }
    
    public void ChallengeRewardItemList()
    {
        var rewardList = DataContainer.GetInstance.GetChallengeRewardList(StageManager.StageNumber);

        foreach (var item in rewardList.Keys)
        {
            CreateChallengeRewardItem();
            tempReward.GetComponentInChildren<Text>().text = rewardList[item];
            switch (item)
            {
                case EChallengeRewardType.Acorn:
                    tempReward.GetComponent<Image>().sprite = RewardImage[0].sprite;
                    break;
                case EChallengeRewardType.Coin:
                    tempReward.GetComponent<Image>().sprite = RewardImage[1].sprite;
                    break;
                case EChallengeRewardType.Hammer:
                    tempReward.GetComponent<Image>().sprite = RewardImage[2].sprite;
                    break;
                case EChallengeRewardType.Bomb:
                    tempReward.GetComponent<Image>().sprite = RewardImage[3].sprite;
                    break;
                case EChallengeRewardType.Rainbow:
                    tempReward.GetComponent<Image>().sprite = RewardImage[4].sprite;
                    break;
            }
        }
    }

    public void SaveDataChallengeItem()
    {
        var rewardList = DataContainer.GetInstance.GetChallengeRewardList(StageManager.StageNumber);

        foreach (var item in rewardList.Keys)
        {
            switch (item)
            {
                case EChallengeRewardType.Acorn:
                    var tempAcorn = PlayerData.GetInstance.Acorn + Int32.Parse(rewardList[EChallengeRewardType.Acorn]);
                    PlayerData.GetInstance.Acorn = tempAcorn;
                    break;
                case EChallengeRewardType.Coin:
                    var tempCoin = PlayerData.GetInstance.Gold + Int32.Parse(rewardList[EChallengeRewardType.Coin]);
                    PlayerData.GetInstance.Gold = tempCoin;
                    break;
                case EChallengeRewardType.Hammer:
                    var tempItemHammer = PlayerData.GetInstance.ItemHammer + Int32.Parse(rewardList[EChallengeRewardType.Hammer]);
                    PlayerData.GetInstance.ItemHammer = tempItemHammer;
                    break;
                case EChallengeRewardType.Bomb:
                    var tempItemBomb = PlayerData.GetInstance.ItemBomb + Int32.Parse(rewardList[EChallengeRewardType.Bomb]);
                    PlayerData.GetInstance.ItemBomb = tempItemBomb;
                    break;
                case EChallengeRewardType.Rainbow:
                    var tempItemRainbow = PlayerData.GetInstance.ItemColor +
                                          Int32.Parse(rewardList[EChallengeRewardType.Rainbow]);
                    PlayerData.GetInstance.ItemColor = tempItemRainbow;
                    break;
            }
        }
    }
    
    private IEnumerator MissionClearCoroutine()
    
    {
        var popupManager = transform.parent.GetComponent<PopupManager>();

        if (popupManager == null) yield return null;
        IsNext = true;
        var isRateUs = false;

        if (PlayerData.GetInstance.PresentLevel % 15 == 0)
            if (PlayerData.GetInstance != null && !PlayerData.GetInstance.IsRateUs)
            {
                if (PlayerData.GetInstance.RateUsYear == 0 && PlayerData.GetInstance.RateUsMonth == 0 &&
                    PlayerData.GetInstance.RateUsDay == 0)
                {
                    isRateUs = true;
                    PlayerData.GetInstance.RateUsYear = DateTime.Now.Year;
                    PlayerData.GetInstance.RateUsMonth = DateTime.Now.Month;
                    PlayerData.GetInstance.RateUsDay = DateTime.Now.Day;
                }
                else
                {
                    var time = new DateTime(PlayerData.GetInstance.RateUsYear, PlayerData.GetInstance.RateUsMonth,
                        PlayerData.GetInstance.RateUsDay);
                    var resultTime = time - DateTime.Now;
                    if (resultTime.Days < 0)
                    {
                        isRateUs = true;
                        PlayerData.GetInstance.RateUsYear = DateTime.Now.Year;
                        PlayerData.GetInstance.RateUsMonth = DateTime.Now.Month;
                        PlayerData.GetInstance.RateUsDay = DateTime.Now.Day;
                    }
                }
            }

        if (isRateUs)
        {
            IsNext = false;
            popupManager.OnClickRateUs();
        }

        yield return new WaitWhile(() => IsNext == false);
        IsNext = false;
        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.ClearCount > 9)
            {
                PlayerData.GetInstance.ClearCount = 0;
                popupManager.CallLimitedShop();
            }
            else
            {
                IsNext = true;
            }
        }

        yield return new WaitWhile(() => IsNext == false);

        switch (nextSceneValue)
        {
            case 0:
                //AdmobManager.GetInstance.ShowAd(EInterstitialKind.WORLDMAP);
                popupManager.CallLoadingTutorialPop("MainScene", 100);
                break;

            case 1:
                //AdmobManager.GetInstance.ShowAd(EInterstitialKind.CONTINUESTAGE);
                popupManager.CallLoadingTutorialPop("GameScene");
                break;

            case 2:
                //AdmobManager.GetInstance.ShowAd(EInterstitialKind.NEXTSTAGE);
                popupManager.CallLoadingTutorialPop("GameScene");
                break;
        }

        yield return new WaitForEndOfFrame();
    }
}