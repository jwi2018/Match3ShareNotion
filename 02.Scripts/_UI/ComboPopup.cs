public class ComboPopup : PopupSetting
{
    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        //StageManager.GetInstance.ComboCount = 0;
    }

    public override void OffPopupSetting()
    {
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }
}