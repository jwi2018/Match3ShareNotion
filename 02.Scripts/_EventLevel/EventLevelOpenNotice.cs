using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLevelOpenNotice : PopupSetting
{
    private PopupManager popupManager;

    private void Start()
    {
        popupManager = transform.parent.GetComponent<PopupManager>();
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void StartEventLevel()
    {
        if (popupManager != null)
        {
            if (EventLevelSystem.GetInstance != null)
            {
                EventLevelSystem.GetInstance.EventMapOpenPickNumber();
                
                EventLevelSystem.GetInstance.IsEventLevel = true;
                EventLevelSystem.GetInstance.EventLevelNum = 1;
                popupManager.CallLoadingTutorialPop("GameScene");
            }
        }
    }
}