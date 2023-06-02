using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventLevelStage_CommingSoon : PopupSetting
{
    private PopupManager popupManager;

    [SerializeField] private Text txtExplain;

    private void Start()
    {
        popupManager = transform.parent.GetComponent<PopupManager>();
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        //EventLevelCommingSoon
        txtExplain.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("EventLevelCommingSoon"), StaticGameSettings.iLimitStageEventLevel);
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
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
            EventLevelSystem.GetInstance.IsEventLevel = false;

            popupManager.CallLoadingTutorialPop("MainScene", 100);
        }
    }

    public void OnClickRestartEventStage()
    {
        if (EventLevelSystem.GetInstance != null)
        {
            EventLevelSystem.GetInstance.IsEventLevel = true;

            popupManager.CallLoadingTutorialPop("GameScene");
        }
    }
}