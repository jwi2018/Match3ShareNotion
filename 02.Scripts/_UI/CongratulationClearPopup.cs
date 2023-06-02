using LogicStates;
using UnityEngine;

public class CongratulationClearPopup : PopupSetting
{
    private bool isSkipAble;
    private bool isSkipButtonTouch;

    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        transform.parent.GetComponent<PopupManager>().IsStageClear = true;
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("F3_StageClear");
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("End");
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnSfxFirePlay()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ClearFirework");
    }

    public void OnBraboPlay()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("ClearBravo");
            SoundManager.GetInstance.Play("ClearStar");
        }
    }

    public void StartCeremony()
    {
        isSkipAble = true;
        StageManager.GetInstance.SetSkipText(true);
        LogicManager.GetInstance.ChangeLogicState(new WaitUserInputLogic());
        BlockManager.GetInstance.ShowClearBomb();
    }

    public void TouchSkipScreen()
    {
        if (isSkipAble && !isSkipButtonTouch)
        {
            isSkipButtonTouch = true;
            StageManager.GetInstance.SkipClearBomb();
        }
    }
}