using System;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class VipShopPopup : PopupSetting
{
    [SerializeField] private Button weekButton;
    [SerializeField] private Text weekButtonText;
    [SerializeField] private Button monthButton;
    [SerializeField] private Text monthButtonText;
    [SerializeField] private Button takeButton;

    [SerializeField] private GameObject buy;
    [SerializeField] private GameObject take;
    [SerializeField] private Text VIP_INFO;

    public Text coinInfo;
    protected ShopItemInfo weekItemInfo;
    protected ShopItemInfo monthItemInfo;
    private readonly int PackageListCount = 0;

    private DateTime dateTime;

    public void Start()
    {
        ButtonInit();


        #region AOS / IOS 텍스트 변경


#if UNITY_ANDROID
        VIP_INFO.text = I2.Loc.LocalizationManager.GetTermTranslation("VipInfoText_AOS");
#elif UNITY_IOS
        
        VIP_INFO.text = I2.Loc.LocalizationManager.GetTermTranslation("VipInfoText_IOS");
#endif

        #endregion

        #region 1주일 상품 버튼 설정

        weekItemInfo = Purchaser.GetInstance.GetPackageItems(8);
        if (weekItemInfo != null)
        {
            if (weekButtonText != null)
            {
                weekButtonText.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("VIP1Week"), weekItemInfo.PurchaserPrice);
                //weekButtonText.text = string.Format("1주일 구독하기 ({0})", weekItemInfo.PurchaserPrice);
            }
        }

        weekButton.onClick.AddListener(() =>
        {
            if (weekItemInfo != null)
            {
                Purchaser.GetInstance.BuyProductID(weekItemInfo.PurchaserID, ButtonInit);
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("vip_weekly_purchase");
            }
        });

        #endregion

        #region 1개월 상품 버튼 설정

        monthItemInfo = Purchaser.GetInstance.GetPackageItems(9);
        if (monthItemInfo != null)
        {
            if (monthButtonText != null)
            {
                monthButtonText.text = string.Format(I2.Loc.LocalizationManager.GetTermTranslation("VIP1Month"), monthItemInfo.PurchaserPrice);
                //monthButtonText.text = string.Format("1개월 구독하기 ({0})", monthItemInfo.PurchaserPrice);
            }
        }

        monthButton.onClick.AddListener(() =>
        {
            if (monthItemInfo != null)
            {
                Purchaser.GetInstance.BuyProductID(monthItemInfo.PurchaserID, ButtonInit);
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("vip_monthly_purchase");  
            }
        });

        #endregion
    }

    IEnumerator GetServerTime()
    {
        var obj = GameObject.Find("PopupManager");
        GameObject coin = null;
        
        if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                dateTime=DateTime.MaxValue;
                UnityWebRequest request = new UnityWebRequest();
                using (request = UnityWebRequest.Get("https://www.google.co.kr"))
                {
                    yield return request.SendWebRequest();
                    
                    if (request.isNetworkError)
                    {
                        Debug.Log(request.error);
                    }
                    else {
                        string date = request.GetResponseHeader("date"); //이곳에서 반송된 데이터에 시간 데이터가 존재
                        dateTime = DateTime.Parse(date).ToLocalTime(); // ToLocalTime() 메소드로 한국시간으로 변환시켜 준다.
                    }

                    if (dateTime != DateTime.MaxValue)
                    {
                        if (!(dateTime > Convert.ToDateTime(PlayerData.GetInstance.GetLastVipGetDay()).AddDays(1) && PlayerData.GetInstance._vipContinue)) // 인터넷 끊겼을 때도 보상 안받아져야함.
                        {
                            takeButton.enabled = false;
                            takeButton.interactable = false;
                        }
                    }
                    
                    #region 받기 버튼 설정
                    
                    takeButton.onClick.RemoveAllListeners();
                    takeButton.onClick.AddListener(() =>
                    {
                        if (dateTime != DateTime.MaxValue)
                        {
                            if (dateTime >
                                Convert.ToDateTime(PlayerData.GetInstance.GetLastVipGetDay())
                                    .AddDays(1)) // 현재 클릭 한 시간 > 이전에 아이템을 받은 시간 + 1일
                            {
                                PlayerData.GetInstance.LastVipGetDay =
                                    dateTime.ToString("yyyy/MM/dd 00:00:00"); // 클릭 시 현재 시간을 LastVipGetDay에 입력
                                PlayerData.GetInstance.Gold += 500;
                                PlayerData.GetInstance.ItemHammer += 1;
                                PlayerData.GetInstance.ItemBomb += 1;
                                PlayerData.GetInstance.ItemColor += 1;
                                takeButton.enabled = false;
                                takeButton.interactable = false;

                                coin = obj.GetComponent<PopupManager>().GetCoin();
                                coin.GetComponent<Animator>().SetTrigger("Normal");
                            }
                            else
                            {
                                Debug.Log("현재 시간 : "+dateTime);
                                Debug.Log("보상 받은 날짜 : " + PlayerData.GetInstance.GetLastVipGetDay());
                                Debug.Log("아직 Vip 보상을 받은지 하루가 지나지 않았습니다.");
                            }
                        }
                    });

                    #endregion
                }
            }
    }

    public override void OnPopupSetting()
    {
        // 구독 아직 안했음
        // 1. 주 / 월 구독 구매 버튼 활성화


        // 구독 했음
        // 1. 주 / 월 구분해서 받기 버튼 활성화 
    }

    public override void OffPopupSetting()
    {
        //this.transform.parent.GetComponent<PopupManager>().SetNotchHighlight(false);
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("PopupShop");
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void ButtonInit()
    {
        if (PlayerData.GetInstance.isVipContinue())
        {
            buy.SetActive(false);
            take.SetActive(true);

            StartCoroutine(GetServerTime());
        }
        //구독중이 아닐 때
        else
        {
            buy.SetActive(true);
            take.SetActive(false);
        }
    }
}