using UnityEngine;

public class MoreGamePopup : PopupSetting
{
    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        OnButtonClick();
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickWoodBlock()
    {
        Application.OpenURL("http://onelink.to/woodblock");
        OnButtonClick();
    }
}