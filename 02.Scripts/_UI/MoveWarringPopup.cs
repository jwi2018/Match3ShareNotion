public class MoveWarringPopup : PopupSetting
{
    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play(SoundManager.GetInstance.ShowFiveCount);
    }

    public override void OffPopupSetting()
    {
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
    }
}