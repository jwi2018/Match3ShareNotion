using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum PopupEventLevelType
{
    MainScene,
    Clear,
}

public class EventLevelPopup : PopupSetting
{
    private PopupManager popupManager;
    [SerializeField] private GameObject AllClear;
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject NObtn = null;
    [SerializeField] private GameObject OKbtn = null;
    [SerializeField] private List<GameObject> eventLevelEntitys = new List<GameObject>();

    [SerializeField] private Text txtExplain;
    [SerializeField] private GameObject gobRightBtn;

    [SerializeField] private Text txtBtnLeft;
    [SerializeField] private Text txtBtnRight;

    private PopupEventLevelType typePopup = PopupEventLevelType.MainScene;
    private bool isMainScene;

    private void Start()
    {
        popupManager = transform.parent.GetComponent<PopupManager>();

        if (animator != null)
        {
            if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
            {
                CheckScene();
            }
            else
            {
                animator.SetInteger("ClearStage", PlayerData.GetInstance.GetEventStageNum() - 1);
                animator.SetInteger("BeforeClearStage", PlayerData.GetInstance.BeforeEventLevelNum);
                animator.SetBool("On", true);
            }
        }

        FirebaseManager.GetInstance.FirebaseLogEvent("eventgame_enter");
        PlayerData.GetInstance.BeforeEventLevelNum = PlayerData.GetInstance.GetEventStageNum() - 1;
        if (txtBtnLeft != null) txtBtnLeft.text = I2.Loc.LocalizationManager.GetTermTranslation("Challenge_FailButton");
        if (txtBtnRight != null) txtBtnRight.text = I2.Loc.LocalizationManager.GetTermTranslation("Challenge_StartButton");

        SetPopupUI();
    }

    public void CheckScene()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainScene"))
        {
            animator.SetInteger("ClearStage", PlayerData.GetInstance.GetEventStageNum() - 1);
            animator.SetInteger("BeforeClearStage", PlayerData.GetInstance.BeforeEventLevelNum);
            animator.SetBool("On", false);
            isMainScene = true;
        }
        else
        {
            animator.SetInteger("ClearStage", PlayerData.GetInstance.GetEventStageNum() - 1);
            animator.SetInteger("BeforeClearStage", PlayerData.GetInstance.BeforeEventLevelNum);
            animator.SetBool("On", true);
        }
    }

    public void SetPopupType(PopupEventLevelType _popupType)
    {
        switch (_popupType)
        {
            case PopupEventLevelType.Clear:

                break;
        }
        typePopup = _popupType;
    }

    public void SetPopupUI()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            switch (typePopup)
            {
                case PopupEventLevelType.MainScene:
#if UNITY_ANDROID
                    ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
#elif UNITY_IOS
                    ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
#endif
                    SetEClearventStage(false);
                    break;

                case PopupEventLevelType.Clear:
#if UNITY_ANDROID
                    ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
#elif UNITY_IOS
                    ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
#endif
                    if (SoundManager.GetInstance != null)
                    {
                        SoundManager.GetInstance.PauseBGM();
                        SoundManager.GetInstance.Play("ClearPopup");
                    }
                    SetEClearventStage(true);
                    if (txtBtnLeft != null) txtBtnLeft.text = I2.Loc.LocalizationManager.GetTermTranslation("Home");
                    if (txtBtnRight != null) txtBtnRight.text = I2.Loc.LocalizationManager.GetTermTranslation("Next");
                    if (StageManager.GetInstance != null)
                    {
                        StageManager.GetInstance.SetSkipText(false);
                    }
                    break;
            }

            if (txtExplain != null)
            {
                if (EventLevelSystem.GetInstance.EventLevelNum == 1)
                {
                    txtExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("EventLevelOpen");
                }
                else
                {
                    string i2Key = string.Format("EventLevelClear{0}", EventLevelSystem.GetInstance.EventLevelNum - 1);
                    txtExplain.text = I2.Loc.LocalizationManager.GetTermTranslation(i2Key);
                }

                if (isMainScene)
                {
                    txtExplain.text = I2.Loc.LocalizationManager.GetTermTranslation("EventLevelOpen");
                }
            }

            StaticScript.SetActiveCheckNULL(gobRightBtn, true);
            if (PlayerData.GetInstance.GetIsEventMapAllClear() == true)
            {
                StaticScript.SetActiveCheckNULL(gobRightBtn, false);
            }
        }
    }

    // public override void OnPopupSetting()
    // {
    //
    // }

    public override void OffPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("eventgame_out");
        switch (typePopup)
        {
            case PopupEventLevelType.MainScene:
#if UNITY_ANDROID
                ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
#elif UNITY_IOS
                ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
#endif
                break;

            case PopupEventLevelType.Clear:
#if UNITY_ANDROID
                ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
#elif UNITY_IOS
                ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
#endif
                break;
        }
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
        GetComponent<Animator>().SetTrigger("Off");

        if (SceneManager.GetActiveScene().name == "GameScene")
            GameObject.Find("PopupManager").GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene", 100);
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void SetCommingSoon()
    {
        if (txtBtnLeft != null) txtBtnLeft.text = I2.Loc.LocalizationManager.GetTermTranslation("Challenge_FailButton");
        if (txtBtnRight != null) txtBtnRight.text = I2.Loc.LocalizationManager.GetTermTranslation("Challenge_StartButton");
    }

    public void StartEventLevel()
    {
        if (popupManager != null)
        {
            if (EventLevelSystem.GetInstance != null)
            {
                Scene scene = SceneManager.GetActiveScene();

                if (PlayerData.GetInstance.GetIsEventMapAllClear())
                {
                    StaticScript.SetActiveCheckNULL(AllClear, true);
                    StaticScript.SetActiveCheckNULL(NObtn, false);
                    StaticScript.SetActiveCheckNULL(OKbtn, false);
                }
                else
                {
                    EventLevelSystem.GetInstance.EventMapOpenPickNumber();

                    EventLevelSystem.GetInstance.IsEventLevel = true;
                    popupManager.CallLoadingTutorialPop("GameScene");

                    if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{EventLevelSystem.GetInstance.EventLevelNum}_event_game_enter"));
                }
            }
        }
    }

    public void PressBackKey_AllClear()
    {
        // StaticScript.SetActiveCheckNULL(AllClear, false);
        // StaticScript.SetActiveCheckNULL(NObtn, true);
        // StaticScript.SetActiveCheckNULL(OKbtn, true);
        GetComponent<Animator>().SetTrigger("Off");
    }

    public void SetEClearventStage(bool showEffect)
    {
        int clearCount = PlayerData.GetInstance.GetEventStageNum();
        if (PlayerData.GetInstance.GetIsEventMapAllClear())
        {
            clearCount = 5;
        }
        else
        {
            clearCount = PlayerData.GetInstance.GetEventStageNum() - 2;
        }
        for (int i = 0; i < eventLevelEntitys.Count; i++)
        {
            EventLevelMoleController elEntity = eventLevelEntitys[i].GetComponent<EventLevelMoleController>();
            if (elEntity != null)
            {
                if (i <= clearCount)
                {
                    if (i == clearCount)
                    {
                        elEntity.SetComplete(showEffect, showEffect);
                    }
                    else
                    {
                        elEntity.SetComplete(false);
                    }
                }
                else
                {
                    elEntity.SetNotComplete();
                }
            }
        }
    }

    public void AllclearCheck()
    {
        if (AllClear != null)
        {
            if (PlayerData.GetInstance.GetIsEventMapAllClear())
            {
                StaticScript.SetActiveCheckNULL(AllClear, true);
                StaticScript.SetActiveCheckNULL(NObtn, false);
                StaticScript.SetActiveCheckNULL(OKbtn, false);
            }
        }
    }
}