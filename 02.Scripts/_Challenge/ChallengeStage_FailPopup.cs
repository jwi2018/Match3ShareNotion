using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class ChallengeStage_FailPopup : PopupSetting, ICoroutineAnimationController
{
    [SerializeField] private Text continueStageButtonText;
    [SerializeField] private Localize challengeText;
    [SerializeField] private Localize challengeText_NextButton;
    
    private bool GetButton;
    private int nextSceneValue;
    public bool IsNext { get; set; }

    void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PauseBGM();
            SoundManager.GetInstance.Play("FailPopup");
        }
        var NextStage = StageManager.StageNumber + 1;
        continueStageButtonText.text = "\" " + NextStage + " \"";
        challengeText.SetTerm("Challenge_Fail");
        challengeText_NextButton.SetTerm("Challenge_NextButton");
        BlockManager.GetInstance.IsSwapAble = false;
        if (EditorAutoModeControll._isAutoMode)
            FirebaseManager.GetInstance.FirebaseLogEvent("Auto_Stage_Fail", "StageNumber", StageManager.StageNumber.ToString());
        else FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{StageManager.StageNumber}_Hardmode_level_failed"));
    }

    public override void OffPopupSetting()
    {
        OnClickGoMain();
    }

    public override void PressedBackKey()
    {
        OnClickGoMain();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickGoMain()
    {
        var popupManager = transform.parent.GetComponent<PopupManager>();
        popupManager.CallLoadingTutorialPop("MainScene", 100);
    }

    public void OnClickNextStage()
    {
        if (!GetButton)
        {
            GetButton = true;

            ADManager.GetInstance.ShowCycleInterstitial();
            StageManager.StageNumber++;
            nextSceneValue = 2;
            StartCoroutine(MissionClearCoroutine());
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