using System;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class ADManager : Singleton<ADManager>
{
    private AdBase adBase = new MaxContainer();

    [HideInInspector] public bool noAdsPopup = false;

    [HideInInspector] public int noAdsCount = 0;
    
    [Tooltip("전면광고 쿨타임이 진행될지 여부를 나타내는 변수")]private bool isInterstitialTimer = false;
    public void SetInterstitialTimer(bool isOn)
    {
        isInterstitialTimer = isOn;
    }
    
    private DateTime timer=DateTime.Now;
    [Tooltip("현재 전면광고 쿨타임")]private double interstitialTimer;
    [Tooltip("전면광고 쿨타임 기준")] private double interstitialCycle = 120;

    [Tooltip("다음스테이지 전면광고가 나오기 시작하는 스테이지")]
    private int interstitialStage = 15;


    public void Start()
    {
        adBase.Init();
        SceneManager.activeSceneChanged += ((arg0, scene) =>
        {
            if (scene.name == "GameScene")
            {
                SetInterstitialTimer(true);
            }
            else
            {
                SetInterstitialTimer(false);
            }
        });
    }
    
    void Update()
    {
        if (isInterstitialTimer)
        {
            if (isInterstitialTimer)
            {
                var time = DateTime.Now - timer;
                if (time.TotalSeconds > 0)
                {
                    interstitialTimer += time.TotalSeconds;
                    timer = DateTime.Now;
                }
            }
        }
    }

    /// <summary>
    /// 일정시간마다 노출되는 전면광고를 위한 함수(다음 스테이지)
    /// </summary>
    public void ShowCycleInterstitial()
    {
        #if UNITY_EDITOR
        #else
        if (interstitialTimer > interstitialCycle && adBase.IsInterstitialAdLoaded(EInterstitialKind.INTERSTITIAL) && StageManager.StageNumber>interstitialStage
        && !IsAdsFree())
        {
            adBase.ShowInterstitialAd(EInterstitialKind.INTERSTITIAL);
            interstitialTimer = 0;
        }
#endif
    }

    /// <summary>
    /// 일반 전면광고를 위한 함수(메인으로, 다시하기, etc...)
    /// </summary>
    /// <param name="kind">전면광고 종류</param>
    public void ShowInterstitial(EInterstitialKind kind)
    {
        #if UNITY_EDITOR
        #else
        if(!IsAdsFree())
        {
            adBase.ShowInterstitialAd(kind);
        }
#endif
    }
    public void ShowInterstitialWithReward(EInterstitialKind kind)
    {
        #if UNITY_EDITOR
        #else
            adBase.ShowInterstitialAd(kind);
        #endif
    }

    /// <summary>
    /// 보상형 광고를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    public void ShowReward(ERewardedKind kind, Action reward)
    {
        adBase.ShowRewardAd(kind, reward);
    }

    /// <summary>
    /// 배너 광고 표시를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public void ShowBanner(EBannerKind kind)
    {
#if UNITY_EDITOR
#else
        if (!IsAdsFree())
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Banner_top");
            adBase.ShowBannerAd(kind);
        }
#endif
    }

    /// <summary>
    /// 배너 광고 숨김을 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public void HideBanner(EBannerKind kind)
    {
        if (!PlayerData.GetInstance.IsAdsFree)
        {
            adBase.HideBannerAd(kind);
        }
    }

    public bool IsinterstitialAdLoad(EInterstitialKind kind)
    {
        return adBase.IsInterstitialAdLoaded(kind);
    }

    public bool IsAdsFree()
    {
     if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                Purchaser.GetInstance.Refresh(() =>
                    {
                        foreach (var productType in Purchaser.GetInstance.SubscriptionProductList.Keys)
                        {
                            var product = Purchaser.GetInstance.GetSubscriptionProduct(productType);

                            if (product != null)
                            {
                                SubscriptionManager subsManager = new SubscriptionManager(product, product.receipt);
                                if (product.receipt!=null)
                                {
                                    if (subsManager.getSubscriptionInfo().isExpired() == Result.True)
                                    {
                                        Debug.Log("구독이 중지되었습니다..");
                                        PlayerData.GetInstance._vipContinue = false;
                                        PlayerData.GetInstance._vipType = ESubsType.None;
                                    }
                                    //구독이 유지중일 경우
                                    else
                                    {
                                        PlayerData.GetInstance._vipContinue = true;
                                        foreach (var id in Purchaser.GetInstance.SubscriptionProductList)
                                        {
                                            if (subsManager.getSubscriptionInfo().getProductId() == id.Value)
                                            {
                                                PlayerData.GetInstance._vipType = id.Key;
                                                break;
                                            }
                                        }

                                        Debug.Log("구독을 유지중입니다.");
                                        break;
                                    }
                                }
                                else
                                {
                                    Debug.Log("정보를 확인할 수 없습니다.");
                                    PlayerData.GetInstance._vipContinue = false;
                                    PlayerData.GetInstance._vipType = ESubsType.None;
                                }
                            }
                            else
                            {
                                Debug.Log("product 정보가 없습니다.");
                            }
                        }
                    },
                    (reason) => { Debug.Log("실패함"); });
            }
        }
        if (PlayerData.GetInstance.IsAdsFree || PlayerData.GetInstance._vipContinue)
        {
            return true;
        }

        return false;
    }
    /*
    public void OnApplicationFocus(bool focused)
    {
        noAdsPopup = false;
        noAdsCount = 0;
    }
    */
}
