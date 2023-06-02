using System;
using DG.Tweening;
using UnityEngine;

public class RateUsPopup : PopupSetting
{
    [SerializeField] private GameObject ChangeButton;
    
    private void Start()
    {
        OnPopupSetting();
    }

    public override void OnPopupSetting()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("DailyBonus");
        if (BaseSystem.GetInstance != null)
        {
            /*
            if (ChallengeSystem.GetInstance!=null)
            {
                GetComponent<Animator>().SetBool("Start_Star", true);
            }
            */
            GetComponent<Animator>().SetBool("Start_Star", true);
        }
    }

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

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        var obj = transform.parent;
        for (var i = 0; i < obj.childCount; i++)
            if (obj.GetChild(i).GetComponent<ICoroutineAnimationController>() != null)
                obj.GetChild(i).GetComponent<ICoroutineAnimationController>().IsNext = true;
        
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

    public void OnClickReview()
    {
#if UNITY_ANDROID
        Application.OpenURL(StaticGameSettings.ReviewAOSURL);
#elif UNITY_IOS
        Application.OpenURL(StaticGameSettings.ReviewIOSURL);
#endif
        if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsRateUs = true;
        OffPopupSetting();
    }
}