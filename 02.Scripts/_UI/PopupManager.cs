using System;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.Purchasing;
using Random = UnityEngine.Random;

public class PopupManager : MonoBehaviour
{
    public static PopupManager instance = null;

    [SerializeField] private GameObject FireParticle_1 = null;
    [SerializeField] private GameObject FireParticle_2 = null;

    [SerializeField] private GameObject BackKeyPop;

    [SerializeField] private DynamicScrollList _stageScrollInfo;
    [SerializeField] private MainSceneScrollView _mainSceneScrollView;

    [SerializeField] private NotchHighlight[] notchHighlights = null;

    private int notchCount = 0;

    private bool _isFirst;

    private float BackKeyDelay = 0.5f;

    private readonly List<UserGold> golds = new List<UserGold>();
    private bool IsItemAnimation;

    public bool IsStageClear { get; set; }

    public DynamicScrollList GetStageScrollInfo => _stageScrollInfo;

    public bool SetisItemAnimation
    {
        set => IsItemAnimation = value;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (LoadingImageManager.CallLoadingPopup) CallLoadingTutorialPop();
        golds.AddRange(FindObjectsOfType<UserGold>());

        if (BlockManager.GetInstance != null)
            if (PlayerData.GetInstance != null && PlayerData.GetInstance.PresentLevel + 1 == StageManager.StageNumber)
                _isFirst = true;
    }

    private void Update()
    {
        BackKeyDelay -= Time.deltaTime;
        if (!IsStageClear)
            if (Application.platform == RuntimePlatform.Android)
                if (BackKeyDelay <= 0)
                    if (Input.GetKey(KeyCode.Escape))
                    {
                        OnBackKey();
                        BackKeyDelay = 0.5f;
                    }
    }

    private void OnApplicationPause(bool pause)
    {
#if UNITY_ANDROID
        if (BackKeyPop != null)
        {
            if (transform.childCount == 0)
            {
                if (BackKeyPop == PopupList.GetInstance.Popup_GamePause) Instantiate(BackKeyPop, transform);
                if (StageManager.GetInstance != null)
                {
                    if (StageManager.GetInstance.IsTutorialActive() == true) StageManager.GetInstance.TutorialEnd();
                    if (StageManager.GetInstance.cashItemTutorial != null)
                    {
                        StageManager.GetInstance.CashTutorialEnd();
                        StageManager.GetInstance.cashItemTutorial.gameObject.SetActiveSelf(false);
                    }
                }
            }
            else
            {
                DeleteExceptionPopup();
            }
        }
#endif
    }

    public void UnLockTouch()
    {
        for (var i = transform.childCount - 1; i > 0; i--)
            if (transform.GetChild(i).GetComponent<CongratulationClearPopup>() != null)
                transform.GetChild(i).GetComponent<CongratulationClearPopup>().OffPopupSetting();
        IsStageClear = false;
    }

    public void GoldAdd(UserGold gold)
    {
        golds.Add(gold);
    }

    public void SetNotchHighlight(bool value)
    {
        if (value)
        {
            notchCount++;
        }
        else
        {
            notchCount--;
            notchCount = Mathf.Max(notchCount, 0);
        }

        if (notchCount > 0)
        {
            for (int i = 0; i < notchHighlights.Length; i++)
            {
                notchHighlights[i].SetPopupActive(true);
            }
        }
        else
        {
            for (int i = 0; i < notchHighlights.Length; i++)
            {
                notchHighlights[i].SetPopupActive(false);
            }
        }
    }

    public void GoldRemove(UserGold gold)
    {
        UserGold obj = null;
        foreach (var item in golds)
            if (item == gold)
            {
                item.StopAllCoroutines();
                obj = item;
                break;
            }

        golds.Remove(obj);
    }

    public bool SerchTutorialStage(int num)
    {
        var returnValue = true;
        switch (num)
        {
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
            case 11:
            case 12:
            case 16:
            case 21:
            case 26:
            case 36:
            case 46:
            case 51:
            case 61:
            case 81:
            case 111:
            case 121:
            case 151:
            case 181:
            case 201:
            case 241:
            case 271:
            case 301:
            case 321:
            case 341:
            case 361:
            case 401:
            case 441:
            case 501:
            case 561:
            case 641:
            case 701:
            case 801:
            case 901:
            case 1001:
            case 1101:
            case 1201:
            case 10:
                returnValue = false;
                break;
        }

        return returnValue;
    }

    public void SerchActiveMiniTutorials()
    {
        var stage = StageManager.StageNumber;
        switch (stage)
        {
            case 1:
                OnClickMiniTutorial();
                break;

            case 2:
                OnClickMiniTutorial(1);
                break;

            case 3:
                OnClickMiniTutorial(2);
                break;

            case 4:
                OnClickMiniTutorial(3);
                break;

            case 5:
                OnClickMiniTutorial(4);
                break;

            case 6:
                OnClickMiniTutorial(5);
                break;

            case 7:
                OnClickMiniTutorial(6);
                break;

            case 11:
                OnClickMiniTutorial(7);
                break;

            case 16:
                OnClickMiniTutorial(8);
                break;

            case 21:
                OnClickMiniTutorial(9);
                break;

            case 26:
                OnClickMiniTutorial(10);
                break;

            case 31:
                OnClickMiniTutorial(11);
                break;

            case 36:
                OnClickMiniTutorial(12);
                break;

            case 41:
                OnClickMiniTutorial(13);
                break;

            case 46:
                OnClickMiniTutorial(14);
                break;
            //case 51:
            //OnClickMiniTutorial(15);
            //break;
            case 51:
                OnClickMiniTutorial(16);
                break;

            case 61:
                OnClickMiniTutorial(17);
                break;

            case 81:
                OnClickMiniTutorial(18);
                break;
            //case 91:
            //OnClickMiniTutorial(19);
            //break;
            case 111:
                OnClickMiniTutorial(20);
                break;

            case 121:
                OnClickMiniTutorial(21);
                break;

            case 151:
                OnClickMiniTutorial(22);
                break;

            case 181:
                OnClickMiniTutorial(23);
                break;

            case 201:
                OnClickMiniTutorial(24);
                break;

            case 241:
                OnClickMiniTutorial(25);
                break;

            case 271:
                OnClickMiniTutorial(26);
                break;

            case 301:
                OnClickMiniTutorial(27);
                break;

            case 321:
                OnClickMiniTutorial(28);
                break;

            case 341:
                OnClickMiniTutorial(29);
                break;

            case 361:
                OnClickMiniTutorial(30);
                break;

            case 401:
                OnClickMiniTutorial(31);
                break;

            case 441:
                OnClickMiniTutorial(32);
                break;

            case 501:
                OnClickMiniTutorial(33);
                break;

            case 561:
                OnClickMiniTutorial(34);
                break;

            case 641:
                OnClickMiniTutorial(35);
                break;

            case 701:
                OnClickMiniTutorial(36);
                break;
        }
    }

    public void GoldRefresh(bool isprimium = false)
    {
        if(isprimium) golds.AddRange(FindObjectsOfType<UserGold>());

        foreach (var item in golds)
            if (item != null && item.gameObject.activeSelf)
                item.Goldsynchronization();
    }

    public void GoldFix()
    {
        foreach (var item in golds) item.GoldFixed();
    }

    public GameObject GetCoin(int value = 0)
    {
        GameObject obj = Instantiate(PopupList.GetInstance.GetCoin, this.transform);
        obj.GetComponent<GetCoinPopup>().AnimationStart(value);
        return obj;
    }

    public void OnBackKey()
    {
        var isBackOkay = false;


        if (StageManager.GetInstance != null)
        {
            if (!StageManager.GetInstance.IsTutorialActive()) isBackOkay = true;
        }
        else
        {
            isBackOkay = true;
        }


        if (isBackOkay)
        {
            DeleteExceptionPopup();

            if (!IsStageClear && !IsItemAnimation)
                //if (BlockManager.GetInstance == null)
            {
                if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
                {
                    if (_mainSceneScrollView != null)
                    {
                        if (!_mainSceneScrollView.isCenter)
                        {
                            _mainSceneScrollView.ClickMain();
                            return;
                        }
                    }
                }


                if (BaseSystem.GetInstance.GetSystemList("AdventuerSystem"))
                {
                    if (transform.childCount == 1 && transform.GetChild(0).gameObject.activeSelf.Equals(false))
                    {
                        if (BackKeyPop == null) return;
                        Instantiate(BackKeyPop, transform);
                        return;
                    }
                }
                else
                {
                    if (transform.childCount == 1 && transform.GetChild(0).name == "StagePopup")
                    {
                        if (BackKeyPop == null) return;
                        Instantiate(BackKeyPop, transform);
                        return;
                    }
                }

               

                if (transform.childCount != 0)
                {
                    var obj = transform.GetChild(transform.childCount - 1).gameObject;
                    if (obj.GetComponent<PopupSetting>() != null) obj.GetComponent<PopupSetting>().PressedBackKey();
                }
                else
                {
                    if (BackKeyPop == null) return;
                    Instantiate(BackKeyPop, transform);
                }
            }
        }
        else
        {
            StageManager.GetInstance.TutorialEnd();
        }
    }

    public void OnComboPop(int ComboCount)
    {
        if (ComboCount < 6) return;

        if (ComboCount < 7)
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_Combo_Nice, transform.parent);
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("ComboMatch_Nice");
        }
        else if (ComboCount < 8)
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_Combo_Perfect, transform.parent);
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("ComboMatch_Perfect");
        }
        else
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_Combo_Fantastic, transform.parent);
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play("ComboMatch_Fantastic");
        }
    }

    public void OnShufflePop()
    {
        if (StageManager.GetInstance.IsTutorialActive() == true)
        {
            StageManager.GetInstance.TutorialEnd();
        }

        var obj = Instantiate(PopupList.GetInstance.Popup_Shuffle, transform);
    }

    public void OnBuyToRemainMove(int Count = 0)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_BuyToRemainMove, transform);
        obj.GetComponent<BuyToRemainMovePopup>().WhatCountToContinue(Count);
    }

    public void OnMoveWarring()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_MoveWarring, transform.parent);
    }

    public GameObject OnClickStarChestGameObject()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("StarChest");

        var obj = Instantiate(PopupList.GetInstance.Popup_StarChestOpen, transform);
        return obj;
    }

    public void OnClickStarChest()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("StarChest");

        if(GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_StarChestOpen, transform);
        }
    }

    public void OnCongratulationClear()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null &&
                transform.GetChild(i).GetComponent<PopupLoadingTutorial>() == null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();
        var obj = Instantiate(PopupList.GetInstance.Popup_CongratulationClear, transform);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.PauseBGM();
    }

    public void OnCongratulationClear_2(int star = 1, float fillAmount = 0.0f)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_CongratulationClear_2, transform);
        obj.GetComponent<CongratulationClearPopup_Fantasy>().Setting(star, fillAmount, _isFirst);
    }

    public void OnClickMovePlus(int type = 0)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_RemainMovePlus, transform);
        if (type == 0) obj.GetComponent<Animator>().SetTrigger("Continue");
        else if (type == 1) obj.GetComponent<Animator>().SetTrigger("Reward");
    }

    public void OnImpossibleMission()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_ImpossibleMission, transform);
    }

    public void OnMissionClear(int Score, int Stage)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_MissionClear, transform);
        obj.GetComponent<MissionClearPopup>().StageAndScore(Stage, Score);
    }

    public void OnMissionClear(int star = 1, float fillAmount = 0.0f)
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<MissionClearPopup>() != null)
                return;

        if (ChallengeSystem.GetInstance != null)
        {
            for (var j = transform.childCount - 1; j >= 0; j--)
                if (transform.GetChild(j).GetComponent<ChallengeStage_ReAskPopup>() != null)
                {
                    for (var i = transform.childCount - 1; i >= 0; i--)
                        if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                            transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();

                    var Cobj = Instantiate(PopupList.GetInstance.Popup_MissionClear, transform);
                    Cobj.GetComponent<MissionClearPopup>().GetStars(star, _isFirst);
                }

            for (var i = transform.childCount - 1; i >= 0; i--)
                if (transform.GetChild(i).GetComponent<ChallengeStage_StartPopup>() != null)
                    return;
        }

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<MissionClearPopup>() != null)
                return;

        UnLockTouch();

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();

        Debug.Log("게임 클리어 팝업 생성");
        var obj = Instantiate(PopupList.GetInstance.Popup_MissionClear, transform);
        obj.GetComponent<MissionClearPopup>().GetStars(star, _isFirst);
    }

    public GameObject OnMissionFail()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_MissionFail, transform);
        return obj;
    }

    public void OnGamePause()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Play_pause_button");
        if (IsItemAnimation) return;

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();

        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_ReAsk, transform);
            }
            else
            {
                var obj = Instantiate(PopupList.GetInstance.Popup_GamePause, transform);
            }
        }
        else
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_GamePause, transform);
        }
    }

    public void OnClickBuyItem(int WhatItem = 0)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_ItemBuy, transform);
        obj.GetComponent<BuyItemPopup>().ItemSetting(WhatItem);
    }

    public void OnClickUseItem(int WhatItem = 0)
    {
        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.ItemHammer > 0 && WhatItem == 0 || StageManager.GetInstance.IsTestMode)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item1_use");
                var obj = Instantiate(PopupList.GetInstance.Popup_ItemUse, transform);
                obj.GetComponent<UseItemPopup>().ItemSetting(WhatItem);
            }
            else if (PlayerData.GetInstance.ItemCross > 0 && WhatItem == 1 || StageManager.GetInstance.IsTestMode)
            {
                var obj = Instantiate(PopupList.GetInstance.Popup_ItemUse, transform);
                obj.GetComponent<UseItemPopup>().ItemSetting(WhatItem);
            }
            else if (PlayerData.GetInstance.ItemBomb > 0 && WhatItem == 2 || StageManager.GetInstance.IsTestMode)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item2_use");
                var obj = Instantiate(PopupList.GetInstance.Popup_ItemUse, transform);
                obj.GetComponent<UseItemPopup>().ItemSetting(WhatItem);
            }
            else if (PlayerData.GetInstance.ItemColor > 0 && WhatItem == 3 || StageManager.GetInstance.IsTestMode)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Item3_use");
                var obj = Instantiate(PopupList.GetInstance.Popup_ItemUse, transform);
                obj.GetComponent<UseItemPopup>().ItemSetting(WhatItem);
            }
            else if (WhatItem > 3)
            {
            }
            else
            {
                OnClickBuyItem(WhatItem);
            }
        }
    }

    public void OnClickSetup()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WorldMap_Setting_Button");
#if UNITY_ANDROID
        var obj = Instantiate(PopupList.GetInstance.Popup_Setup_AOS, transform);
#else
        GameObject obj = Instantiate(PopupList.GetInstance.Popup_Setup_IOS, this.transform);
#endif
    }

    public void OnClickShopMainButton(bool beforeLimited = false)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Shop_Package, transform);

        if (FireParticle_1 != null) FireParticle_1.SetActive(false);
        if (FireParticle_2 != null) FireParticle_2.SetActive(false);

        var botShop = obj.GetComponent<BotShopPopup>();
        if (botShop != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("Intro_packageshop_enter");
            //botShop.ChangeShopList(EShopKind.PACKAGE);
            botShop.Init(() =>
            {
                if (FireParticle_1 != null) FireParticle_1.SetActive(true);
                if (FireParticle_2 != null) FireParticle_2.SetActive(true);
            });
        }
    }

    public void OnClickShopCoinButton(bool beforeLimited = false)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Shop_Coin, transform);

        var botShop = obj.GetComponent<BotShopPopup>();
        if (botShop != null)
        {
            botShop.ChangeShopList(EShopKind.GOLD);
        }
    }

    public void OnClickEventLevelButton()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Event_Level, transform);
    }

    public void OnClickShopPackageButton(bool beforeLimited = false)
    {
        if (PlayerData.GetInstance != null)
        {
            if (!PlayerData.GetInstance.IsBuyLimitedPackage && !beforeLimited &&
                !PlayerData.GetInstance.IsSeeLimitedPackage)
            {
                var obj = CallLimitedShop();
                if (obj != null) obj.GetComponent<LimitedShopPopup>().IsClickShopButton = true;
            }
            else
            {
                var obj = Instantiate(PopupList.GetInstance.Popup_Shop_Package, transform);
            }
        }
    }

    public void OnClickShopPlayButton()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Shop_Play, transform);

        var botShop = obj.GetComponent<BotShopPopup>();
        if (botShop != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("Intro_coinshop_enter");
            botShop.ChangeShopList(EShopKind.PACKAGE);
        }
    }

    public void OnClickLanguage()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Language, transform);
    }

    public void OnClickSavingBox()
    {
        GameObject obj = Instantiate(PopupList.GetInstance.Popup_SavingBox, this.transform);
    }

    public void OnClickMoreGame()
    {
#if UNITY_ANDROID
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("MoreGame");
        Application.OpenURL("https://play.google.com/store/apps/dev?id=8069522235284918301");
#elif UNITY_IOS
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("MoreGame");
        Application.OpenURL("https://apps.apple.com/developer/hyejin-lee/id1643676334");
#endif
    }

    public void OnClickTutorial()
    {
        Instantiate(PopupList.GetInstance.Popup_Tutorial, transform);
    }

    public void CallTextLog(int num = 2)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_TextLog, transform);
        obj.GetComponent<TextLogPopup>().SetString(num);
    }

    public void OnClickMiniTutorial(int num = 0)
    {
        if (num >= PopupList.GetInstance.TutorialsPopupList.Count) return;
        var obj = Instantiate(PopupList.GetInstance.TutorialsPopupList[num], transform);
        obj.GetComponent<MiniTutorialPopup>().OnTutorial(num);
    }

    public void OnClickPrivacyPolicyURL()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLScpNb_0llMpvOhsWpmobtwGMjsluZtWkXh4PaAREZiM-nb2Iw/viewform");
#elif UNITY_IOS
        Application.OpenURL("");
#endif
    }

    public void OnClickTermsOfServiceURL()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://docs.google.com/forms/d/e/1FAIpQLSdC_iYOQ2nJPoCzOeKKTBnrIzRZajFCMYS1QqsnCvoXUXTMgg/viewform");
#elif UNITY_IOS
        Application.OpenURL("");
#endif
    }

    public void OnClickPrivacyPolicy()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_PrivacyPolicy, transform);
    }

    public void OnClickTermsOfService()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_TermsOfService, transform);
    }

    public void OnClickDailyBonus()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_DailyBonus, transform);
    }

    public void OnClickRateUs()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Rate_Us, transform);
    }

    public void OnClickDailyQuest()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_DailyQuest, transform);
    }

    public GameObject OnClickMissionGuid()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_MissionGuid, transform);
        return obj;
    }

    public void OnClickRoulette(bool _isDailySpin = false)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Roulette, transform);
        RoulettePopup rouletteScript = obj.GetComponent<RoulettePopup>();
        if (null != rouletteScript)
        {
            rouletteScript.CheckProgress(_isDailySpin);
        }
        else
        {
            RoulettePopup_Dotween rouletteScript_Dotween = obj.GetComponent<RoulettePopup_Dotween>();
            if (null != rouletteScript_Dotween)
            {
                rouletteScript_Dotween.CheckProgress();
            }
        }
    }

    public void OnClickAcorns()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Acorns, transform);
    }

    public GameObject OnClickAcornsGameClearPopUp()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Acorns, transform);
        return obj;
    }

    public void OnClickLoadQuestion()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_LoadQuestion, transform);
    }

    public void OnClickPrimiumTicket()
    {
        var obj = Instantiate(PopupList.GetInstance.Pop_PrimiumTicket, transform);
    }

    public GameObject GetCoin()
    {
        var obj = Instantiate(PopupList.GetInstance.GetCoin, transform);
        return obj;
    }

    public void InputStageNumber(int num)
    {
        FirebaseManager.GetInstance.DebugLog("icecube: Stage number is " + num);
        StageManager.StageNumber = num;
    }

    public void CallLoadingPop()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Loading, transform);
    }

    public void CallLoadingTutorialPop(string SceneName, int SceneImage = 0)
    {
        FirebaseManager.GetInstance.DebugLog("icecube: Scene name is " + SceneName);
        var ImageSelect = Random.Range(0, 10);
        if (SceneImage == 0) LoadingImageManager.LoadingImageNumber = ImageSelect;
        else LoadingImageManager.LoadingImageNumber = SceneImage;
        var Time = LoadingImageManager.SceneTransTime;
        var obj = Instantiate(PopupList.GetInstance.Popup_LoadingTutorial, transform);

        obj.GetComponent<PopupLoadingTutorial>().FadeIn(Time, LoadingImageManager.LoadingImageNumber, SceneName);
    }

    public void CallLoadingTutorialPop()
    {
        var ImageSelect = LoadingImageManager.LoadingImageNumber;
        var Time = LoadingImageManager.SceneTransTime;
        var obj = Instantiate(PopupList.GetInstance.Popup_LoadingTutorial, transform);
        obj.GetComponent<PopupLoadingTutorial>().FadeOut(Time, ImageSelect);
    }

    public GameObject CallWeeklyBonus()
    {
        var _isDaily = false;
        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.DailyYear == 0 && PlayerData.GetInstance.DailyMonth == 0 &&
                PlayerData.GetInstance.DailyDay == 0)
            {
                _isDaily = true;
                PlayerData.GetInstance.DailyYear = DateTime.Now.Year;
                PlayerData.GetInstance.DailyMonth = DateTime.Now.Month;
                PlayerData.GetInstance.DailyDay = DateTime.Now.Day;
            }
            else
            {
                var time = new DateTime(PlayerData.GetInstance.DailyYear, PlayerData.GetInstance.DailyMonth,
                    PlayerData.GetInstance.DailyDay);
                var resultTime = time - DateTime.Now;
                if (resultTime.Days < 0)
                {
                    DebugX.Log(resultTime.Days);
                    _isDaily = true;
                    PlayerData.GetInstance.DailyYear = DateTime.Now.Year;
                    PlayerData.GetInstance.DailyMonth = DateTime.Now.Month;
                    PlayerData.GetInstance.DailyDay = DateTime.Now.Day;
                    if (resultTime.Days < -1) PlayerData.GetInstance.WeeklyXDay = 0;
                }
            }
        }

        if (_isDaily)
        {
            WeeklyRetentionIndicator.showRewardAdCount = 0;
            WeeklyRetentionIndicator.showInsAdCount = 0;
            DebugX.Log(11);
            var obj = Instantiate(PopupList.GetInstance.Popup_WeeklyBonus, transform);
            return obj;
        }

        return null;
    }

    public void OnClickServiceCenter()
    {
        var mailto = "gamecsservice@gmail.com";
        var subject = EscapeURL(StaticGameSettings.CsReportTitle);
        var body = EscapeURL
        ("_____\n\n" +
         "Device Model : " + SystemInfo.deviceModel + "\n\n" +
         "Device OS : " + SystemInfo.operatingSystem + "\n\n" +
         "_____"
        );

        Application.OpenURL("mailto:" + mailto + "?subject=" + subject + "&body=" + body);
    }

    private string EscapeURL(string url)
    {
        return WWW.EscapeURL(url).Replace("+", "%20");
    }

    public void FaceBookLike()
    {
        Application.OpenURL("https://www.facebook.com/Icecube-109854981807528");
        //Application.OpenURL("https://www.youtube.com/channel/UCdf0vcVQufEkg5MtHl7gINg");
    }

    public void YoutubeFollow()
    {
        Application.OpenURL("https://www.youtube.com/channel/UC1QbnSqQvGRRpL384AxuyIA");
    }

    public void OpenAchievement()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Intro_achievement_enter");
        if (GpgsManager.GetInstance != null) GpgsManager.GetInstance.ShowAchievementUI();
    }

    public void ExceptionPopup()
    {
#if UNITY_ANDROID
        if (transform.childCount == 0) Instantiate(PopupList.GetInstance.Pop_RewardExcption, transform);
#endif
    }

    public void DeleteExceptionPopup()
    {
#if UNITY_ANDROID
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() == null &&
                transform.GetChild(i).GetComponent<GetCoinPopup>() == null &&
                transform.GetChild(i).name != "StagePopup")
                Destroy(transform.GetChild(i).gameObject);
#endif
    }

    public void ChestOpen()
    {
        for (var i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).GetComponent<MissionClearPopup>() != null)
                transform.GetChild(i).GetComponent<MissionClearPopup>().InStars();
    }

    public GameObject CallRewardLoadingPop(RewardAdsButton adsButton)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_NotRewardedAds, transform);
        obj.GetComponent<NotRewardedAdsPopup>().SetRequestAdsButton(adsButton);
        return obj;
    }

    /*public GameObject CallRewardLoadingPop(AdCommon common)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_NotRewardedAds, transform);
        obj.GetComponent<NotRewardedAdsPopup>().SetRequestAdsButton(common);
        return obj;
    }*/

    public GameObject CallRewardLoadingPop(RoulettePopup adsButton)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_NotRewardedAds, transform);
        obj.GetComponent<NotRewardedAdsPopup>().SetRequestAdsButton(adsButton);
        return obj;
    }

    public GameObject LoadPopup(GameObject _targetPopup)
    {
        var obj = Instantiate(_targetPopup, transform);
        return obj;
    }

    public GameObject CallLimitedShop()
    {
        if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
        {
            CallLimitedShop_TwoTime();
            return null;
        }

        if (PlayerData.GetInstance == null) return null;
        if (Application.internetReachability == NetworkReachability.NotReachable) return null;

        if (!PlayerData.GetInstance.IsBuyLimitedPackage)
        {
            var condition = false;
            if (!PlayerData.GetInstance.IsSeeLimitedPackage)
            {
                condition = true;
            }
            else
            {
                if (PlayerData.GetInstance.LimitedShopDay == 0 &&
                    PlayerData.GetInstance.LimitedShopMonth == 0) // && PlayerData.GetInstance.LimitedShopDay == 0
                {
                    condition = true;
                    PlayerData.GetInstance.LimitedShopYear = DateTime.Now.Year;
                    PlayerData.GetInstance.LimitedShopMonth = DateTime.Now.Month;
                    PlayerData.GetInstance.LimitedShopDay = DateTime.Now.Day;
                }
                else
                {
                    var time = new DateTime(PlayerData.GetInstance.LimitedShopYear,
                        PlayerData.GetInstance.LimitedShopMonth, PlayerData.GetInstance.LimitedShopDay);
                    var resultTime = time - DateTime.Now;
                    Debug.Log(
                        "time : " + time + "  resulttime : " + resultTime + "resulttime.days : " + resultTime.Days);
                    if (resultTime.Days >= 0)
                    {
                        condition = true;
                        PlayerData.GetInstance.LimitedShopYear = DateTime.Now.Year;
                        PlayerData.GetInstance.LimitedShopMonth = DateTime.Now.Month;
                        PlayerData.GetInstance.LimitedShopDay = DateTime.Now.Day;
                    }
                }

                condition = true;
            }

            if (condition)
            {
                var obj = Instantiate(PopupList.GetInstance.Popup_LimitedShop, transform);
                return obj;
            }
        }
        return null;
    }

    public void CallLimitedShop_TwoTime()
    {
        var nowHour = DateTime.Now.Hour;
        if (nowHour < 10) return;

        if (!PlayerData.GetInstance.IsBuyLimitedPackage)
        {
            PlayerData.GetInstance.LimitedShopDay = DateTime.Now.Day;

            bool condition;
            if (!PlayerData.GetInstance.IsSeeLimitedPackage)
            {
                condition = true;
            }
            else
            {
                condition = false;
            }

            if (condition)
            {
                Instantiate(PopupList.GetInstance.Popup_LimitedShop, transform);
            }
        }
    }

    public void RewardContinueStage()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<BuyToRemainMovePopup>() != null)
            {
                transform.GetChild(i).GetComponent<BuyToRemainMovePopup>().isrewardContinue = true;
                transform.GetChild(i).GetComponent<BuyToRemainMovePopup>().OffPopupSetting();
            }
    }

    public void WorldMapTopShop()
    {
        //AdmobManager.GetInstance.ShowAd(EInterstitialKind.NEXTSTAGE);
        // 2022 - 01 - 18 전완익.
        //AdmobManager.GetInstance.ShowAd(null, ERewardedKind.GETITEM);

        var paramater = new Dictionary<string, string>();
        paramater.Add("ShopKinds", "WorldMap_Top");
        if (FirebaseManager.GetInstance != null)  FirebaseManager.GetInstance.FirebaseLogEvent("WorldMap_coinshop_Top", paramater);
        OnClickShopCoinButton();
    }

    public void WorldMapBotShop()
    {
        var paramater = new Dictionary<string, string>();
        paramater.Add("ShopKinds", "WorldMap_Bot");
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("WorldMap_shop_Bottom", paramater);
        OnClickShopMainButton();
    }

    public void ShowGetItemPopup(bool isOk)
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_GetItem, transform);
        obj.GetComponent<GetItemPopup>().SetPopup(isOk);
    }

    public void ShowChallengeClearPopup()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<ChallengeStage_ClearPopup>() != null)
                return;

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_Clear, transform);
    }

    public void ShowChallengeFailPopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_Fail, transform);
    }

    public GameObject ShowChallengeRewardPopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_Reward, this.transform.position, Quaternion.identity);
        return obj;
    }

    public void ShowChallengeReAskPopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_ReAsk, transform);
    }

    public void ShowChallengeStartPopup()
    {
        //
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<ChallengeStage_StartPopup>() != null)
                return;
        //
        UnLockTouch();

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_Start, transform);
    }

    public void ShowChallengeTutorialPopup()
    {
        //
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<ChallengeStage_TutorialStartPopup>() != null)
                return;
        //

        // 챌린지 튜토리얼 팝업 Show
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();
        var obj = Instantiate(PopupList.GetInstance.Popup_ChallengeStage_Tutorial, transform);
    }

    public void ShowVipPopup()
    {
         if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Purchaser.GetInstance.Refresh(() =>
                    {
                        foreach (var productType in Purchaser.GetInstance.SubscriptionProductList.Keys)
                        {
                            var product = Purchaser.GetInstance.GetSubscriptionProduct(productType);

                            if (product != null)
                            {
                                SubscriptionManager subsManager = new SubscriptionManager(product, product.receipt);
                                if (product.receipt!=null)
                                {
                                    if (subsManager.getSubscriptionInfo().isExpired() == Result.True)
                                    {
                                        Debug.Log("구독이 중지되었습니다..");
                                        PlayerData.GetInstance._vipContinue = false;
                                        PlayerData.GetInstance._vipType = ESubsType.None;
                                    }
                                    //구독이 유지중일 경우
                                    else
                                    {
                                        PlayerData.GetInstance._vipContinue = true;
                                        foreach (var id in Purchaser.GetInstance.SubscriptionProductList)
                                        {
                                            if (subsManager.getSubscriptionInfo().getProductId() == id.Value)
                                            {
                                                PlayerData.GetInstance._vipType = id.Key;
                                                break;
                                            }
                                        }

                                        Debug.Log("구독을 유지중입니다.");
                                        break;
                                    }
                                }
                                else
                                {
                                    PlayerData.GetInstance._vipContinue = false;
                                    PlayerData.GetInstance._vipType = ESubsType.None;
                                    Debug.Log("정보를 확인할 수 없습니다.");
                                }
                            }
                            else
                            {
                                Debug.Log("product 정보가 없습니다.");
                            }
                        }
                        var obj = Instantiate(PopupList.GetInstance.Popup_Vip_Shop, transform);
                    },
                    (reason) => { Debug.Log("실패함"); });
            }
            else
            {
                CallTextLog(6);
            }
        }
    }

    public void ShowEventLevelOpen()
    {
        if (PopupList.GetInstance.Popup_EventLevel_Open != null)
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Open, transform);
            if (obj != null)
            {
                obj.SendMessage("SetCommingSoon", SendMessageOptions.DontRequireReceiver);
            }
        }
    }

    public void ShowEventLevelClear()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<EventLevelPopup>() != null)
                return;

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<EventLevelStage_Clear>() != null)
                return;

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();

        //var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Clear, transform);
        if (PlayerData.GetInstance != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"eventgame_{EventLevelSystem.GetInstance.EventLevelNum}level_clear"));
            PlayerData.GetInstance.SaveEventLevelNumber(++EventLevelSystem.GetInstance.EventLevelNum);
        }

        if (EventLevelSystem.GetInstance.EventLevelNum == 6)
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Clear, transform);
        }
        else
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_Event_Level, transform);
            EventLevelPopup popEventLevel = obj.GetComponent<EventLevelPopup>();
            if (popEventLevel != null)
            {
                popEventLevel.SetPopupType(PopupEventLevelType.Clear);
            }
        }
    }

    public void ShowEventLevelFail()
    {
        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<EventLevelStage_Fail>() != null)
                return;

        for (var i = transform.childCount - 1; i >= 0; i--)
            if (transform.GetChild(i).GetComponent<PopupSetting>() != null)
                transform.GetChild(i).GetComponent<PopupSetting>().OffPopupSetting();

        var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Fail, transform);
    }

    public void ShowEventLevelRetryPopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_EventLevel_Retry, transform);
    }

    public void ShowNoAdsPopup()
    {
        if (PopupList.GetInstance.Popup_No_Ads != null)
        {
            var obj = Instantiate(PopupList.GetInstance.Popup_No_Ads, transform);
        }
    }

    public void ShowEventLevelCommingSoon()
    {
        if (PopupList.GetInstance.Pop_Event_ComingSoon != null)
        {
            var obj = Instantiate(PopupList.GetInstance.Pop_Event_ComingSoon, transform);
        }
    }

    public void ShowNoticePopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Popup_Notice, transform);
    }

    public void ShowMainScenePackageShopPopup()
    {
        var obj = Instantiate(PopupList.GetInstance.Pop_MainScenePackage, transform);
    }

    public LimitedPackagePopup ShowLimitedPackage()
    {
        return Instantiate(PopupList.GetInstance.Pop_LimitedPackage, transform).GetComponent<LimitedPackagePopup>();
    }
}