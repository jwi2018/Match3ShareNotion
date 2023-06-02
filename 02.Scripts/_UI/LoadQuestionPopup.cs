using UnityEngine;

public class LoadQuestionPopup : PopupSetting
{
    public override void OnPopupSetting()
    {
    }

    public override void OffPopupSetting()
    {
        Destroy(gameObject);
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public void OnClickLoadButton()
    {
#if UNITY_ANDROID
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            transform.parent.GetComponent<PopupManager>().CallTextLog(5);
        }
        else
        {
            transform.parent.GetComponent<PopupManager>().CallLoadingPop();
            GpgsSaveManager.GetInstance.LoadFromCloud("SaveData");
        }
#endif
        OffPopupSetting();
    }
}