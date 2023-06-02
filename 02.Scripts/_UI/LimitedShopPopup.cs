using System;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;

public class LimitedShopPopup : PopupSetting
{
    [SerializeField] private PackageSetting limitedPackage;
    [SerializeField] private Text timeHourText;
    [SerializeField] private Text timeMinuteText;
    [SerializeField] private Text timeSecondText;
    [SerializeField] private Vector2Int FirstHourValue = new Vector2Int(12, 15);
    [SerializeField] private Vector2Int SecondHourValue = new Vector2Int(18, 20);
    [SerializeField] private bool IsTestMode;
    [SerializeField] private GameObject[] disableWhenNotTimerObjects;
    [SerializeField] private Animator animator;

    public bool IsClickShopButton;

    private ELimitedState limitedState = ELimitedState.NONE;

    private void Start()
    {
        OnPopupSetting();
    }

    private void Update()
    {
        if (PlayerData.GetInstance != null)
            if (PlayerData.GetInstance.IsBuyLimitedPackage)
                OffPopupSetting();
        if (timeHourText == null || timeMinuteText == null || timeSecondText == null) return;
        /* 한정판매 시간보여주는 부분
        DateTime myDt = DateTime.Now;

        int maxValue = 0;
        if (limitedState == ELimitedState.FIRST) maxValue = FirstHourValue.y;
        else if (limitedState == ELimitedState.SECOND) maxValue = SecondHourValue.y;
        else if (limitedState == ELimitedState.NONE) return;

        DateTime conditionDt = new DateTime(myDt.Year, myDt.Month, myDt.Day, maxValue, 0, 0, DateTimeKind.Local);
        TimeSpan resultTime = conditionDt.Subtract(myDt);

        timeHourText.text = resultTime.Hours.ToString("D2") + ":";
        timeMinuteText.text = resultTime.Minutes.ToString("D2") + ":";
        timeSecondText.text = resultTime.Seconds.ToString("D2");*/
    }

    public override void OnPopupSetting()
    {
        if (BaseSystem.GetInstance.GetSystemList("CircusSystem") || BaseSystem.GetInstance.GetSystemList("Fantasy"))
        {
            CheckLimitPopup();
            return;
        }
        
        
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        //임시 주석
        //limitedPackage.Init(7);
        if (animator != null) animator.SetTrigger("On");
        if (IsClickShopButton)
            if (disableWhenNotTimerObjects != null)
                for (var i = 0; i < disableWhenNotTimerObjects.Length; i++)
                    disableWhenNotTimerObjects[i].SetActive(false);
        if (IsTestMode)
        {
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play(SoundManager.GetInstance.MissionPopupOpen);
            return;
        }

        var myDt = DateTime.Now;
        if (myDt.Hour >= FirstHourValue.x && myDt.Hour < FirstHourValue.y)
        {
            limitedState = ELimitedState.FIRST;
            if (PlayerData.GetInstance != null)
            {
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.MissionPopupOpen);
                PlayerData.GetInstance.IsSeeLimitedPackage = true;
            }
        }
        else if (myDt.Hour >= SecondHourValue.x && myDt.Hour < SecondHourValue.y)
        {
            limitedState = ELimitedState.SECOND;
            if (PlayerData.GetInstance != null)
            {
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.MissionPopupOpen);
                PlayerData.GetInstance.IsSeeLimitedPackage = true;
            }
        }
        else if (IsClickShopButton && !PlayerData.GetInstance.IsSeeLimitedPackage)
        {
            limitedState = ELimitedState.NONE;
            if (PlayerData.GetInstance != null)
            {
                if (SoundManager.GetInstance != null)
                    SoundManager.GetInstance.Play(SoundManager.GetInstance.MissionPopupOpen);
                PlayerData.GetInstance.IsSeeLimitedPackage = true;
            }
        }
        else
        {
            OffPopupSetting();
        }
    }

    public override void OffPopupSetting()
    {
        var obj = transform.parent;
        for (var i = 0; i < obj.childCount; i++)
            if (obj.GetChild(i).GetComponent<ICoroutineAnimationController>() != null)
                obj.GetChild(i).GetComponent<ICoroutineAnimationController>().IsNext = true;

        GetComponent<Animator>().SetTrigger("Off");

        /*if (PopupList.GetInstance.Popup_Notice != null)
        {
            PopupManager.instance.ShowNoticePopup();
        }*/
        //Destroy(gameObject);
    }

    public void CheckLimitPopup()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        if (animator != null) animator.SetTrigger("On");
        
        if (PlayerData.GetInstance != null)
        {
            if (SoundManager.GetInstance != null)
                SoundManager.GetInstance.Play(SoundManager.GetInstance.MissionPopupOpen);
            PlayerData.GetInstance.IsSeeLimitedPackage = true;
        }
    }
    
    public override void OnButtonClick()
    {
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public void PurchaseItem()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("limitedpackage_purchase");

        if (Purchaser.GetInstance != null)
            Purchaser.GetInstance.BuyProductID(Purchaser.GetInstance.GetPackageItems(7).PurchaserID);
    }

    private enum ELimitedState
    {
        FIRST,
        SECOND,
        NONE
    }

    public void ActiveFalse()
    {

    }
}