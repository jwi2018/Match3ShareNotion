using UnityEngine;

public class DailyBonusPopup : PopupSetting
{
    [SerializeField] private Animator CoinAnimator;

    private bool GetTouch;

    private void Start()
    {
        GetTouch = false;
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DailyBonus");
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OnClickDailyBonus();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void OnClickDailyBonus()
    {
        if (GetTouch) return;
        CoinAnimator.SetTrigger("Normal");
        if (PlayerData.GetInstance != null)
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play("GetCoin");
                PlayerData.GetInstance.Gold += 200;
            }

        GetTouch = true;
        OffPopupSetting();
    }

    public void OnCilckRewardDailyBonus()
    {
        //if (AdsManager.GetInstance != null)
        //{
        //    AdsManager.GetInstance.ShowRewardVideo((isDone) => {
        //        if (isDone)
        //        {
        //            GetCoinTwice();
        //        }
        //    });
        //}
        GetCoinTwice();
    }

    public void GetCoinTwice()
    {
        if (GetTouch) return;
        CoinAnimator.SetTrigger("Twice");
        if (PlayerData.GetInstance != null)
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play("GetCoin");
                PlayerData.GetInstance.Gold += 400;
            }

        GetTouch = true;
        OffPopupSetting();
    }
}