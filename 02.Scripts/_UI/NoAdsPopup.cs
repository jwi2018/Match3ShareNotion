using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class NoAdsPopup : PopupSetting
{
    [SerializeField] private GameObject buyNoAdsButton;
    
    
    private void Start()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("noad_popup");
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        ADManager.GetInstance.noAdsPopup = false;
        
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DailyBonus");
    }

    /*
    public void ButtonChange()
    {
        if (BaseSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance!=null)
            {
                ChangeButton.SetActive(false);
            }
        }
        //ChangeButton.SetActive(false);
    }
    */

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");

        StopCoroutine(BuyNoAds());
        //GetComponent<Animator>().SetBool("Start_Star", false);
        //ChangeButton.SetActive(true);
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void ClickBuyButton()
    {
        StartCoroutine(BuyNoAds());
    }

    private IEnumerator BuyNoAds()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (PlayerData.GetInstance.IsAdsFree)
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("Click_RemoveAd_new");
                OffPopupSetting();
            }
        }
    }
}