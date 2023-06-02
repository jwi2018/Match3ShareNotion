using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLevelStage_Pause : PopupSetting
{
    [SerializeField] private Text title_text = null;
    [SerializeField] private Text btn_text = null;

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = false;
    }

    private void Start()
    {
        if (EventLevelSystem.GetInstance.isRetry)
        {
            if (title_text != null) title_text.text = I2.Loc.LocalizationManager.GetTermTranslation("Replay");
            if (btn_text != null) btn_text.text = I2.Loc.LocalizationManager.GetTermTranslation("Replay");
        }
        else
        {
            if (title_text != null) title_text.text = I2.Loc.LocalizationManager.GetTermTranslation("Pause");
            if (btn_text != null) btn_text.text = I2.Loc.LocalizationManager.GetTermTranslation("Quit");
        }
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = true;
        
        if(EventLevelSystem.GetInstance != null) EventLevelSystem.GetInstance.isRetry = false;

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
        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.isRetry)
            {
                EventLevelSystem.GetInstance.IsEventLevel = true;
                StageManager.StageNumber = EventLevelSystem.GetInstance.OriginLevel;

                var popupManager = transform.parent.GetComponent<PopupManager>();
                popupManager.CallLoadingTutorialPop("GameScene");

                EventLevelSystem.GetInstance.isRetry = false;
            }
            else
            {
                EventLevelSystem.GetInstance.IsEventLevel = false;
                StageManager.StageNumber = EventLevelSystem.GetInstance.OriginLevel;
                var popupManager = transform.parent.GetComponent<PopupManager>();
                popupManager.CallLoadingTutorialPop("MainScene", 100);
            }
        }
    }
}