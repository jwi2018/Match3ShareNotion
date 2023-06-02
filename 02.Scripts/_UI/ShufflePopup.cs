using UnityEngine;

public class ShufflePopup : PopupSetting
{
    public override void OnPopupSetting()
    {
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }
}