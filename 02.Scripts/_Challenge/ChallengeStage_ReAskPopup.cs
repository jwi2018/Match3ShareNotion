using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class ChallengeStage_ReAskPopup : PopupSetting, ICoroutineAnimationController
{
    private bool GetButton;
    private int nextSceneValue;
    
    [SerializeField] private Localize challengeText;
    [SerializeField] private Localize challengeText_FailButton;
    [SerializeField] private Localize challengeText_Continue;
    
    public bool IsNext { get; set; }
    
    
    void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        challengeText.SetTerm("Challenge_Pause");
        challengeText_FailButton.SetTerm("Challenge_FailButton");
        challengeText_Continue.SetTerm("Continue");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = false;
    }

    public override void OffPopupSetting()
    {
        //ChallengeSystem.GetInstance.isChallengeStage = false;
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = true;
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
        ChallengeSystem.GetInstance.IsChallengeStage = false;
        var popupManager = transform.parent.GetComponent<PopupManager>();
        popupManager.CallLoadingTutorialPop("MainScene", 100);
    }
    
    public void OnClickStartNormalStage()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Challenge_next_button");
        if (ChallengeSystem.GetInstance.IsChallengeStage)
        {
            StageManager.GetInstance.ShowFailPopup();
            ChallengeSystem.GetInstance.IsChallengeStage = false;
        }
        else
        {
            ChallengeSystem.GetInstance.IsChallengeStage = false;
            //StageManager.GetInstance.ShowClearPopup();
            StageManager.StageNumber++;
            nextSceneValue = 2;
            StartCoroutine(MissionClearCoroutine());
        }
        //ChallengeSystem.GetInstance.isChallengeStage = false;
        //StageManager.GetInstance.ShowClearPopup();
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