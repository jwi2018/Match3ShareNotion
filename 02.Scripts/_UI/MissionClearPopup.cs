using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

public class MissionClearPopup : PopupSetting, ICoroutineAnimationController
{
    [SerializeField] private Text StageText;

    [SerializeField] private Text ScoreText;

    [SerializeField] private GameObject NextButton;

    [SerializeField] private Animator starAnimator;

    [SerializeField] private Animator rewardBoxAnimator;

    [SerializeField] private Animator buttonListAnimator;

    [SerializeField] private Text rewardText;

    [SerializeField] private Text SpecialrewardText;

    [SerializeField] private Image rewardImage;

    [SerializeField] private Image boxImage_Top;

    [SerializeField] private List<Sprite> boxSprites_Top = new List<Sprite>();

    [SerializeField] private Image boxImage_Bot;

    [SerializeField] private List<Sprite> boxSprites_Bot = new List<Sprite>();

    [SerializeField] private List<RewardItem> _ClearRewardItem;

    [SerializeField] private List<Sprite> _RewardItemSprite;

    [SerializeField] private List<int> Persent;

    [SerializeField] private int SpecialStageGold = 50;
    [SerializeField] private Slider sliderTresureBox = null;
    [SerializeField] private Text textTresureBox;
    [SerializeField] private GameObject tresurebox_alram;

    [SerializeField] private Slider acronValueSlider;
    [SerializeField] private Text acronValueText;
    [SerializeField] private GameObject FullchestImg = null;

    [SerializeField] private Text savingCoin;

    [SerializeField]
    private SavingAnim savingAnim = null;

    private int getStar = 0;

    public bool _isRateUsProcess = true;

    private int _boxColumnCount;
    private bool _isFirst;
    private int _totalStar;

    private bool GetButton;
    private int nextSceneValue;
    private bool rewardStage;

    public bool MissionClear = false;

    
    // 클리어 팝업 하단의 리워드 광고 버튼 추가
    // 수정 필요
    [SerializeField] private int coolTime;
    [SerializeField] private Text timeText;
    private int missionClearGold = 0;
    private int MissionClearSpecial = 0;
    private int specialRewardInt = 0;
    [SerializeField] private GameObject specialRewardObj;
    [SerializeField] private Image specialRewardSpr;
    [SerializeField] private GameObject specialRewardObjLight;
    [SerializeField] private Text specialRewardText;
    [SerializeField] private List<GameObject> ButtonSetObj = new List<GameObject>();
    [SerializeField] private List<GameObject> RewardObj = new List<GameObject>();
    [SerializeField] private List<GameObject> CheckObj = new List<GameObject>();
    //

    private Vector3 vecTargetPosition;
    [SerializeField] private GameObject target;
    private void Start()
    {
        if (FirebaseManager.GetInstance != null)
            FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"Stage_{StageManager.StageNumber}_Clear"));

        if (PlayerData.GetInstance.SpecialRewardYear == 0)
        {
            DateTime rewardTime = DateTime.Now;
            
            PlayerData.GetInstance.SpecialRewardYear = rewardTime.AddSeconds(-coolTime).Year;
            PlayerData.GetInstance.SpecialRewardMonth = rewardTime.AddSeconds(-coolTime).Month;
            PlayerData.GetInstance.SpecialRewardDay = rewardTime.AddSeconds(-coolTime).Day;
            PlayerData.GetInstance.SpecialRewardHour = rewardTime.AddSeconds(-coolTime).Hour;
            PlayerData.GetInstance.SpecialRewardMinute = rewardTime.AddSeconds(-coolTime).Minute;
            PlayerData.GetInstance.SpecialRewardSecond = rewardTime.AddSeconds(-coolTime).Second;
        }

        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                StartCoroutine(SpecialRewardCoroutine());
            }
           
        }

        //OnPopupSetting();
        ADManager.GetInstance.SetInterstitialTimer(false);
        //AdsManager.GetInstance.HideBanner();
    }

    private void OnDisable()
    {
        if (null != ADManager.GetInstance)
        {
            ADManager.GetInstance.SetInterstitialTimer(true);
        }
    }

    public override void OnEnable()
    {
        OnPopupSetting();
    }

    public bool IsNext { set; get; }

    public override void OnPopupSetting()
    {
        missionClearGold = 0;
        MissionClearSpecial = 0;
        specialRewardInt = 0;
        
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                StartCoroutine(COSliderTresureBoxValue());
            }
           
        }

        //ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
        if (!StageStatus.isFirstClear)
        {
            StageStatus.isFirstClear = true;
            //AdmobManager.GetInstance.ShowInterstitial(120);
        }

        if (GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
        {
            //if(FullchestImg != null) FullchestImg.SetActive(true);
        }
        
        Debug.Log("미션 클리어!");
        //AdmobManager.GetInstance.ShowAd(EInterstitialKind.NEXTSTAGE);
        StageManager.GetInstance.SetSkipText(false);
        StageManager.GetInstance.IsSkipAble = false;

        transform.SetAsFirstSibling();
        GetButton = false;
        
        if (BaseSystem.GetInstance != null && StageText != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                StageText.text = "Level " + StageManager.StageNumber.ToString();
            }
            else
            {
                StageText.text = StageManager.StageNumber.ToString();
            }
        }
        else
        {
            if(StageText != null) StageText.text = StageManager.StageNumber.ToString();
        }

        //ScoreText.text = StageManager.GetInstance.PreScore.ToString();
        DOTweenEx.DOTextInt(ScoreText, 0, StageManager.GetInstance.PreScore, 1, it => it.ToString("#,##0"));
        if (StageManager.StageNumber == StaticGameSettings.TotalStage) NextButton.SetActive(false);
        BlockManager.GetInstance.IsSwapAble = false;
        BlockManager.GetInstance.SetBlockCollider(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearPopup");

        if (AcornSystem.GetInstance != null)
        {
            if (acronValueSlider != null)
            {
                acronValueSlider.maxValue = AcornSystem.GetInstance.RewardedStandardValue;
                acronValueSlider.value = PlayerData.GetInstance.Acorn;
                acronValueText.text = string.Format("{0} / {1}", PlayerData.GetInstance.Acorn,
                    AcornSystem.GetInstance.RewardedStandardValue);
            }
        }
    }

    public override void OffPopupSetting()
    {
        if (BaseSystem.GetInstance != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            {
                StopCoroutine(COSliderTresureBoxValue());
            }
        }
        
        OnClickGoMain();
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
            GetButton = true;
            ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);

            if (ChallengeSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance.ConfirmChallengeStage(StageManager.StageNumber))
                {
                    FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_popup"));

                    if (StaticGameSettings.TotalStage != StageManager.StageNumber)
                    {
                        PopupManager.instance.ShowChallengeStartPopup();
                    }
                    else
                    {
                        ADManager.GetInstance.noAdsPopup = true;
                        nextSceneValue = 0;
                        StartCoroutine(MissionClearCoroutine());
                    }
                }
                else
                {
                    ADManager.GetInstance.noAdsPopup = true;
                    nextSceneValue = 0;
                    StartCoroutine(MissionClearCoroutine());
                }
            }
            else
            {
                ADManager.GetInstance.noAdsPopup = true;
                nextSceneValue = 0;
                StartCoroutine(MissionClearCoroutine());
            }
        }
    }

    public void OnClickRePlay()
    {
        if (!GetButton)
        {
            GetButton = true;
            nextSceneValue = 1;
            StartCoroutine(MissionClearCoroutine());
            ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);
        }
    }

    public void OnClickNextStage()
    {
        if (!GetButton)
        {
            GetButton = true;

            ADManager.GetInstance.ShowCycleInterstitial();

            if (ChallengeSystem.GetInstance != null)
            {
                if (ChallengeSystem.GetInstance.ConfirmChallengeStage(StageManager.StageNumber))
                {
                    FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_hardmode_popup"));
                    
                    if (StaticGameSettings.TotalStage != StageManager.StageNumber)
                    {
                        PopupManager.instance.ShowChallengeStartPopup();
                    }
                    else
                    {
                        StageManager.StageNumber++;
                        nextSceneValue = 2;
                        StartCoroutine(MissionClearCoroutine());
                    }
                }
                else
                {
                    StageManager.StageNumber++;
                    nextSceneValue = 2;
                    StartCoroutine(MissionClearCoroutine());
                }
            }
            else
            {
                StageManager.StageNumber++;
                nextSceneValue = 2;
                StartCoroutine(MissionClearCoroutine());
            }
        }
    }

    public void SetUIInfo()
    {
    }

    public void GetStars(int num, bool First)
    {
        getStar = num;
        var intValue = 0;
        _isFirst = First;

        int beforeStar = PlayerData.GetInstance.GetLevelStartCount(StageManager.StageNumber);

        if (PrimiumTicketSystem.GetInstance != null) PrimiumTicketSystem.GetInstance.GETGAMECLEARSTARCOUNT += num;
            
        SetSavingCoin(num, beforeStar);
        if (_isFirst)
        {
            if (StageManager.StageNumber % 15 == 0)
            {
                rewardStage = true;
                var MaxPersent = 0;
                for (var i = 0; i < Persent.Count; i++) MaxPersent += Persent[i];
                var Temp = Random.Range(1, MaxPersent);
                for (var i = 0; i < Persent.Count; i++)
                {
                    Temp -= Persent[i];
                    if (Temp <= 0)
                    {
                        Temp = i;
                        break;
                    }
                }

                rewardImage.sprite = _RewardItemSprite[Temp];
                MissionClearSpecial = _ClearRewardItem[Temp].intValue;
                SpecialrewardText.text = "+" + MissionClearSpecial;
                switch (_ClearRewardItem[Temp].useItem)
                {
                    case EUseItem.NONE:
                        PlayerData.GetInstance.Gold += _ClearRewardItem[Temp].intValue;
                        specialRewardInt = 0;
                        break;

                    case EUseItem.HAMMER:
                        PlayerData.GetInstance.ItemHammer += _ClearRewardItem[Temp].intValue;
                        specialRewardInt = 1;
                        break;

                    case EUseItem.CROSS:
                        PlayerData.GetInstance.ItemCross += _ClearRewardItem[Temp].intValue;
                        specialRewardInt = 2;
                        break;

                    case EUseItem.BOMB:
                        PlayerData.GetInstance.ItemBomb += _ClearRewardItem[Temp].intValue;
                        specialRewardInt = 3;
                        break;

                    case EUseItem.COLOR:
                        PlayerData.GetInstance.ItemColor += _ClearRewardItem[Temp].intValue;
                        specialRewardInt = 4;
                        break;
                }
            }
            //rewardText.text = "+20";
            missionClearGold = 20;
            rewardText.text = "+" + missionClearGold.ToString();
            PlayerData.GetInstance.Gold += 20;
            //GetReward(missionClearGold);
        }
        else
        {
            //rewardText.text = "+1";
            missionClearGold = 1;
            specialRewardInt = Random.Range(0, 5);
            rewardText.text = "+" + missionClearGold.ToString();
            PlayerData.GetInstance.Gold += 1;
            //GetReward(missionClearGold);
        }

        int iBeforeTotalStars = GameVariable.GetRemainStarCount();

        if (num == 3)
        {
            PlayerData.GetInstance.SetLevelStartCount(StageManager.StageNumber, num);
            starAnimator.SetTrigger("Star_3");
        }
        else if (num == 2)
        {
            PlayerData.GetInstance.SetLevelStartCount(StageManager.StageNumber, num);
            starAnimator.SetTrigger("Star_2");
        }
        else
        {
            PlayerData.GetInstance.SetLevelStartCount(StageManager.StageNumber, num);
            starAnimator.SetTrigger("Star_1");
        }

        int iAfterTotalStars = GameVariable.GetRemainStarCount();

        if (sliderTresureBox != null)
        {
            int updateSkip = -1;
            int valueToTween = iBeforeTotalStars;
            DOTween.To(() => valueToTween, x => valueToTween = x, iAfterTotalStars, 1.5f).OnUpdate(() =>
            {
                if (updateSkip != valueToTween)
                {
                    int sliderValue = valueToTween > GameVariable.GetNeedOpenStarCount() ? GameVariable.GetNeedOpenStarCount() : valueToTween;
                    Debug.LogWarningFormat("KKI sliderValue {0}", sliderValue);
                    sliderTresureBox.maxValue = GameVariable.GetNeedOpenStarCount();
                    sliderTresureBox.value = sliderValue;
                    textTresureBox.text = string.Format("{0} / {1}", valueToTween, GameVariable.GetNeedOpenStarCount());
                    updateSkip = valueToTween;

                    //Debug.LogWarningFormat("KKI{0}", valueToTween);
                }
            });
        }
        
        if (GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
        {
            //if(FullchestImg != null) FullchestImg.SetActive(true);
        }
    }

    public void onClickGameClearStarChest()
    {
        int iBeforeTotalStars = PlayerData.GetInstance.GetTotalStarCount();

        if (GameVariable.GetRemainStarCount() >= GameVariable.GetNeedOpenStarCount())
        {
            
            if (!GetButton)
            {
                var gameObject = transform.parent.GetComponent<PopupManager>().OnClickStarChestGameObject();
                StarChestPopup starChest = gameObject.GetComponent<StarChestPopup>();
                if (starChest != null)
                {
                    starChest.SetOffAction(SliderTresureBoxValue);
                }
            }
        }
    }

    public void SliderTresureBoxValue()
    {
        if (sliderTresureBox != null)
        {
            sliderTresureBox.maxValue = GameVariable.GetNeedOpenStarCount();
            sliderTresureBox.value = GameVariable.GetRemainStarCount();
            textTresureBox.text = string.Format("{0} / {1}", GameVariable.GetRemainStarCount(), GameVariable.GetNeedOpenStarCount());
        }

        if (savingCoin != null)
        {
            savingCoin.text = string.Format("{0:+#,0;-#,0;0}", PlayerData.GetInstance.SavingCoin);
        }
        
        if (GameVariable.GetRemainStarCount() < GameVariable.GetNeedOpenStarCount())
        {
            //if(FullchestImg != null) FullchestImg.SetActive(false);
            //tresurebox_alram.SetActive(false);
            tresurebox_alram.SetActiveSelf(false);
        }
        else
        {
            //tresurebox_alram.SetActive(true);
            tresurebox_alram.SetActiveSelf(true);
        }
    }

    private IEnumerator COSliderTresureBoxValue()
    {
        while (true)
        {
            SliderTresureBoxValue();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void onClickAcornButton()
    {
        var gameObject = transform.parent.GetComponent<PopupManager>().OnClickAcornsGameClearPopUp();
        AcornsPopup acornsPopup = gameObject.GetComponent<AcornsPopup>();
        if (acornsPopup != null)
        {
            acornsPopup.SetOffAction(SliderAcornValue);
        }
        
        //PopupManager.instance.OnClickAcorns();
    }
    
    public void SliderAcornValue()
    {
        if (sliderTresureBox != null)
        {
            acronValueSlider.maxValue = 100;
            acronValueSlider.value = PlayerData.GetInstance.Acorn;
            acronValueText.text = string.Format("{0} / {1}", PlayerData.GetInstance.Acorn, 100);
        }
    }
    
    private void SetSavingCoin(int getStar, int beforeStar)
    {
        int getCoinValue = PlayerData.GetInstance.SavingCoin;
        if (getStar > beforeStar)
        {
            for (int i = beforeStar + 1; i <= getStar; i++)
            {
                if (i == 1)
                {
                    getCoinValue += 6;
                }
                else if (i == 2)
                {
                    getCoinValue += 7;
                }
                else if (i == 3)
                {
                    getCoinValue += 7;
                }
            }

            if (getCoinValue > SavingInfomation.isTotalSavingCoin)
            {
                getCoinValue = SavingInfomation.isTotalSavingCoin;
            }

            PlayerData.GetInstance.SavingCoin = getCoinValue;
        }
    }

    public void StageAndScore(int Stage, int Score)
    {
        ScoreText.text = Score.ToString();
    }

    public void OnClickStarChest()
    {
        if (!GetButton) transform.parent.GetComponent<PopupManager>().OnClickStarChest();
    }

    public void InStars()
    {
    }

    public void StarAnimationEnd()
    {
        if (rewardStage)
            rewardBoxAnimator.SetTrigger("Open2");
        else
            rewardBoxAnimator.SetTrigger("Open");

        savingAnim.StartAnim(StageManager.GetInstance.BeforeStars, getStar);
    }

    public void RewardAnimationEnd()
    {
        buttonListAnimator.SetTrigger("Start");
    }

    public void ButtonAnimationEnd()
    {
        transform.parent.GetComponent<PopupManager>().UnLockTouch();
    }

    public void StarInGaugeStart(int num = 0)
    {
        if (num == 0)
        {
            transform.parent.GetComponent<PopupManager>().UnLockTouch();
        }
    }

    public void StarShining()
    {
        GetComponent<ShiningItem>().StartShine();
    }

    public void PlaySFX()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearPopup");
    }

    private IEnumerator RateUsCorutine(int num)
    {
        var obj = transform.parent.gameObject;
        yield return new WaitWhile(() => _isRateUsProcess);

        if (num == 0)
        {
            obj.GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene", 100);
            GetButton = true;
        }
        else if (num == 1)
        {
            obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
            GetButton = true;
        }
    }

    public void OnClick()
    {
#if UNITY_ANDROID
        Application.OpenURL("https://play.google.com/store/apps/dev?id=5914057434208552564");
#elif UNITY_IOS
        Application.OpenURL("https://itunes.apple.com/developer/myungjin-jang/id1394999371");
#endif
    }

    public void ClearPopupAnimationEnd()
    {
        starAnimator.SetBool("Next", true);
    }

    private IEnumerator MissionClearCoroutine()

    {
        var popupManager = transform.parent.GetComponent<PopupManager>();

        if (popupManager == null) yield return null;
        IsNext = true;
        var isRateUs = false;

        if (StageManager.StageNumber > 14)
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

    public void ShowCoinReward()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play(SoundManager.GetInstance.PopupShop);
            }
            PlayerData.GetInstance.Gold += 150;
            missionClearGold += 150;
            rewardText.text = "+" + missionClearGold.ToString();
            
            CheckObj[0].SetActive(true);
            RewardCoolTimeObj(false, 0);
            
            // 1. 해당 버튼 비활성화 ex) ButtonSetObj[List].GetComponent<Button>().interactable = false;
            // 2. 체크 이미지 활성화 && 보상 골드 이미지 비활성화
        });
    }
    
    public void ShowSpecialReward()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            PlayerData.GetInstance.SpecialRewardYear = System.DateTime.Now.Year;
            PlayerData.GetInstance.SpecialRewardMonth = System.DateTime.Now.Month;
            PlayerData.GetInstance.SpecialRewardDay = System.DateTime.Now.Day;
            PlayerData.GetInstance.SpecialRewardHour = System.DateTime.Now.Hour;
            PlayerData.GetInstance.SpecialRewardMinute = System.DateTime.Now.Minute;
            PlayerData.GetInstance.SpecialRewardSecond = System.DateTime.Now.Second;
            
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play(SoundManager.GetInstance.StarBoxGetItem);
            }
            
            StartCoroutine(SpecialRewardCoroutine());

            if (StageManager.StageNumber % 15 == 0)
            {
                int temp = 0;
                switch (specialRewardInt)
                {
                    case 0 :
                        PlayerData.GetInstance.Gold += 200;
                        missionClearGold += 200;
                        rewardText.text = "+" + missionClearGold.ToString();
                        break;
                    case 1 :
                        PlayerData.GetInstance.ItemHammer += 1;
                        break;
                    case 2 :
                        PlayerData.GetInstance.ItemCross += 1;
                        break;
                    case 3 :
                        PlayerData.GetInstance.ItemBomb += 1;
                        break;
                    case 4 :
                        PlayerData.GetInstance.ItemColor += 1;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                int tempSpecialRewardInt = Random.Range(0, 4);
                int temp = 0;
                switch (tempSpecialRewardInt)
                {
                    case 0 :
                        PlayerData.GetInstance.Gold += 200;
                        missionClearGold += 200;
                        rewardText.text = "+" + missionClearGold.ToString();
                        break;
                    case 1 :
                        PlayerData.GetInstance.ItemHammer += 1;
                        ShowAdSpecialSwitch(temp, tempSpecialRewardInt);
                        break;
                    case 2 :
                        PlayerData.GetInstance.ItemBomb += 1;
                        ShowAdSpecialSwitch(temp, tempSpecialRewardInt);
                        break;
                    case 3 :
                        PlayerData.GetInstance.ItemColor += 1;
                        ShowAdSpecialSwitch(temp, tempSpecialRewardInt);
                        break;
                    default:
                        break;
                }
            }
        });
    }

    public void ShowAdSpecialSwitch(int tempInt, int tempSpecialRewardInt)
    {
        specialRewardObj.SetActive(true);
        specialRewardSpr.sprite = _RewardItemSprite[tempSpecialRewardInt];
        tempInt = ++MissionClearSpecial;
        specialRewardText.text = "+" + tempInt.ToString();
    }

    public void ShowSavingBox()
    {
        PopupManager.instance.OnClickSavingBox();
    }

    public void ShowStarChest()
    {
        if (GameVariable.GetRemainStarCount() > GameVariable.GetNeedOpenStarCount())
        {
            PopupManager.instance.OnClickStarChest();
        }
    }
    
    IEnumerator SpecialRewardCoroutine()
    {
        RewardCoolTimeObj(false, 1);
        
        DateTime time =
            new DateTime(PlayerData.GetInstance.SpecialRewardYear, PlayerData.GetInstance.SpecialRewardMonth, PlayerData.GetInstance.SpecialRewardDay,
                PlayerData.GetInstance.SpecialRewardHour, PlayerData.GetInstance.SpecialRewardMinute, PlayerData.GetInstance.SpecialRewardSecond);
        TimeSpan resultTime = time - DateTime.Now;
        
        float CoolTime = (float) resultTime.TotalSeconds + coolTime;

        int minute = 0;
        int second = 0;
        while (CoolTime > 0)
        {
            CoolTime -= Time.deltaTime;

            minute = (int)CoolTime / 60;
            second = (int) CoolTime % 60;
            
            timeText.gameObject.SetActive(true);

            timeText.text = minute + ":" + second.ToString("D2");
            
            yield return new WaitForEndOfFrame();
        }
        
        timeText.gameObject.SetActive(false);
        RewardCoolTimeObj(true, 1);
    }

    public void RewardCoolTimeObj(bool cool, int buttonNum)
    {
        specialRewardObjLight.SetActive(cool);
        RewardObj[buttonNum].SetActive(cool);
        ButtonSetObj[buttonNum].GetComponent<Button>().interactable = cool;
    }

    public void GetReward(int getCoin)
    {
        vecTargetPosition = target.transform.position;
        GameObject gobPopup = PopupManager.instance.LoadPopup(PopupList.GetInstance.Pop_GetReward);
        Pop_GetReward popReward = gobPopup.GetComponent<Pop_GetReward>();
        popReward.ShowReward(EDailyQuestRewardType.COIN, 1, getCoin, "GetCoin", vecTargetPosition);
    }

}