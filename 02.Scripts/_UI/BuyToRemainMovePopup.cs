using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyToRemainMovePopup : PopupSetting
{
    [SerializeField] private GameObject[] _itemList;

    [SerializeField] private List<Text> _payGold = new List<Text>();

    [SerializeField] private UserGold Gold;

    [SerializeField] private GameObject continueButton;

    [SerializeField] private List<GameObject> rewardContinueButtons = new List<GameObject>();

    [SerializeField] private GameObject gobInterstitialAd;
    [SerializeField] private GameObject gobRewardAd;

    public bool _isBuyMove;
    public bool isrewardContinue;

    private int ContinueCount;

    private void Start()
    {
        OnPopupSetting();
        ADManager.GetInstance.SetInterstitialTimer(false);
    }

    private void OnDisable()
    {
        if (null != ADManager.GetInstance)
        {
            ADManager.GetInstance.SetInterstitialTimer(true);
        }
    }

    public override void OnPopupSetting()
    {
        //ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        if (Gold != null)
        {
            var obj = GameObject.Find("PopupManager");
            obj.GetComponent<PopupManager>().GoldAdd(Gold);
        }

        BlockManager.GetInstance.IsSwapAble = false;
        BlockManager.GetInstance.SetBlockCollider(false);
        _isBuyMove = false;

        if (EditorAutoModeControll._isAutoMode)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("Auto_Stage_Fail", "StageNumber", StageManager.StageNumber.ToString());

            if (EditorAutoModeControll.NowCount >= EditorAutoModeControll.TestCount)
            {
                EditorAutoModeControll.NowCount = 0;
                if (EditorAutoModeControll.TestStageList.Count == 1)
                    StageManager.StageNumber++;
                else
                    for (var i = 0; i < EditorAutoModeControll.TestStageList.Count; i++)
                        if (EditorAutoModeControll.TestStageList[i] == StageManager.StageNumber)
                        {
                            i++;
                            if (i == EditorAutoModeControll.TestStageList.Count)
                            {
                                EditorAutoModeControll.TestStageList.Clear();
                                transform.parent.GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene", 100);
                                return;
                            }

                            StageManager.StageNumber = EditorAutoModeControll.TestStageList[i];
                        }
            }
            else
            {
                EditorAutoModeControll.NowCount++;
            }

            transform.parent.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
        }
    }

    public override void OffPopupSetting()
    {
        ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        BlockManager.GetInstance.SetBlockCollider(true);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        GetComponent<Animator>().SetTrigger("Off");
        var obj = GameObject.Find("PopupManager");
        if (Gold != null) obj.GetComponent<PopupManager>().GoldRemove(Gold);
        if (!_isBuyMove && !isrewardContinue)
        {
            obj.GetComponent<PopupManager>().OnMissionFail();
            ContinueCount = Mathf.Min(2, ContinueCount);
            var ContinueName = "Continue_Coin" + (ContinueCount + 1);
            var paramater = new Dictionary<string, string>();
            paramater.Add("Continue", false.ToString());
            FirebaseManager.GetInstance.FirebaseLogEvent(ContinueName, paramater);
        }
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickContinueStage()
    {
        if (StageManager.GetInstance != null)
        {
            if (ContinueCount == 0)
            {
                if (PlayerData.GetInstance.Gold >= 500)
                {
                    var ContinueName = "Continue_Coin" + (ContinueCount + 1);
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("Continue", true.ToString());
                    FirebaseManager.GetInstance.FirebaseLogEvent(ContinueName, paramater);

                    GetComponent<Animator>().SetTrigger("Off");
                    _isBuyMove = true;
                }
                else
                {
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("ShopKinds", "BuyMove_Fail");
                    FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);

                    transform.parent.GetComponent<PopupManager>().OnClickShopPlayButton();
                }
            }
            else if (ContinueCount == 1)
            {
                if (PlayerData.GetInstance.Gold >= 700)
                {
                    GetComponent<Animator>().SetTrigger("Off");
                    _isBuyMove = true;
                    var ContinueName = "Continue_Coin" + (ContinueCount + 1);
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("Continue", true.ToString());
                    FirebaseManager.GetInstance.FirebaseLogEvent(ContinueName, paramater);
                }
                else
                {
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("ShopKinds", "BuyMove_Fail");
                    FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);

                    transform.parent.GetComponent<PopupManager>().OnClickShopPlayButton();
                }
            }
            else if (ContinueCount >= 2)
            {
                if (PlayerData.GetInstance.Gold >= 900)
                {
                    GetComponent<Animator>().SetTrigger("Off");
                    _isBuyMove = true;
                    ContinueCount = Mathf.Min(2, ContinueCount);
                    var ContinueName = "Continue_Coin" + (ContinueCount + 1);
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("Continue", true.ToString());
                    FirebaseManager.GetInstance.FirebaseLogEvent(ContinueName, paramater);
                }
                else
                {
                    var paramater = new Dictionary<string, string>();
                    paramater.Add("ShopKinds", "BuyMove_Fail");
                    FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);
                    transform.parent.GetComponent<PopupManager>().OnClickShopPlayButton();
                }
            }
        }
    }

    public void OnClickShopButton()
    {
        var paramater = new Dictionary<string, string>();
        paramater.Add("ShopKinds", "BuyMove_Shop");
        FirebaseManager.GetInstance.FirebaseLogEvent("Shop", paramater);

        transform.parent.GetComponent<PopupManager>().OnClickShopPlayButton();
    }

    public void WhatCountToContinue(int Count)
    {
        ContinueCount = Count;
        if (Count == 0)
        {
            StaticScript.SetActiveCheckNULL(gobInterstitialAd, true);
            StaticScript.SetActiveCheckNULL(gobRewardAd, false);
            _itemList[0].SetActive(true);
            _itemList[1].SetActive(false);
            _itemList[2].SetActive(false);
            foreach (var item in _payGold) item.text = "500";

            StaticScript.SetActiveCheckNULL(continueButton, false);
            foreach (var item in rewardContinueButtons) item.SetActive(true);
        }
        else if (Count == 1)
        {
            StaticScript.SetActiveCheckNULL(gobInterstitialAd, false);
            StaticScript.SetActiveCheckNULL(gobRewardAd, true);
            _itemList[0].SetActive(true);
            _itemList[1].SetActive(true);
            _itemList[2].SetActive(false);
            foreach (var item in _payGold) item.text = "700";
            StaticScript.SetActiveCheckNULL(continueButton, true);
            foreach (var item in rewardContinueButtons) item.SetActive(true);
        }
        else if (Count >= 2)
        {
            StaticScript.SetActiveCheckNULL(gobInterstitialAd, false);
            StaticScript.SetActiveCheckNULL(gobRewardAd, true);
            _itemList[0].SetActive(true);
            _itemList[1].SetActive(true);
            _itemList[2].SetActive(true);
            foreach (var item in _payGold) item.text = "900";
            StaticScript.SetActiveCheckNULL(continueButton, true);
            foreach (var item in rewardContinueButtons) item.SetActive(true);
        }
    }

    public void ContinueCheck()
    {
        if (isrewardContinue)
            StageManager.GetInstance.ContinueStage();
    }

    public void ShowRewardAD()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Continue_Reward");
            isrewardContinue = true;
            foreach (var item in rewardContinueButtons) item.SetActive(false);
            StaticScript.SetActiveCheckNULL(continueButton, true);
            OffPopupSetting();
        });
    }

    public void ShowAd()
    {
        if (ADManager.GetInstance.IsinterstitialAdLoad(EInterstitialKind.INTERSTITIAL))
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Continue_IS");
            ADManager.GetInstance.ShowInterstitialWithReward(EInterstitialKind.INTERSTITIAL);
            isrewardContinue = true;
            foreach (var item in rewardContinueButtons) item.SetActive(false);
            StaticScript.SetActiveCheckNULL(continueButton, true);
            OffPopupSetting();
        }
        /*
        AD.ShowAd(ERewardedKind.MOVEPLUS, () =>
        {
            isrewardContinue = true;
            foreach (var item in rewardContinueButtons) item.SetActive(false);
            continueButton.SetActive(true);
            OffPopupSetting();
        });
        */
    }
}