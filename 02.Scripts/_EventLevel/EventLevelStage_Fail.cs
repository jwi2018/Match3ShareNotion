using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLevelStage_Fail : PopupSetting
{
    private PopupManager popupManager;

    private void Start()
    {
        popupManager = transform.parent.GetComponent<PopupManager>();
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PauseBGM();
            SoundManager.GetInstance.Play("FailPopup");
        }

        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent(string.Format($"{EventLevelSystem.GetInstance.EventLevelNum}_event_game_failed"));
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