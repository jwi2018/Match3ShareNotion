using UnityEngine;

public class GameExitPopup : PopupSetting
{
    public void OnDisable()
    {
        ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
    }
    
    private void Start()
    {
        PlayerPrefs.Save();
        OnPopupSetting();
        ADManager.GetInstance.ShowBanner(EBannerKind.BANNER);
    }

    public override void OnPopupSetting()
    {
        //ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        //AdsManager.GetInstance.ShowEndBanner();
    }

    public override void OffPopupSetting()
    {
        //this.GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        Destroy(gameObject);
        //EndBannerExit();
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickGameExit()
    {
        //var ajc = new AndroidJavaClass("com.lancekun.quit_helper.AN_QuitHelper");
        //var UnityInstance = ajc.CallStatic<AndroidJavaObject>("Instance");
        //UnityInstance.Call("AN_Exit");

        Application.Quit();
    }

    public void OnClickMoreGame()
    {
        transform.parent.GetComponent<PopupManager>().OnClickMoreGame();
        OffPopupSetting();
    }

    //public void EndBannerExit()
    //{
    //    AdsManager.GetInstance.HideEndBanner();
    //}
}