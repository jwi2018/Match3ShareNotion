using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using AppLovinMax;

public class MaxBannerAdClass
{
    public string AdKey;
    public bool IsLoad;

    public MaxBannerAdClass(string key)
    {
        AdKey = key;
    }
}

public class MaxInterstitialAdClass
{
    public string AdKey;
    public bool IsLoad;

    public MaxInterstitialAdClass(string key)
    {
        AdKey = key;
    }
}

public class MaxRewardAdClass
{
    public string AdKey;
    public bool IsLoad;

    public MaxRewardAdClass(string key)
    {
        AdKey = key;
    }
}

public class MaxContainer : AdBase
{
    private Dictionary<EBannerKind, MaxBannerAdClass> bannerAdList = new Dictionary<EBannerKind, MaxBannerAdClass>();
    private Dictionary<EInterstitialKind, MaxInterstitialAdClass> interstitialAdList = new Dictionary<EInterstitialKind, MaxInterstitialAdClass>();
    private Dictionary<ERewardedKind, MaxRewardAdClass> rewardAdList = new Dictionary<ERewardedKind, MaxRewardAdClass>();

    private Action reward;

    public override void Init()
    {
        base.Init();

#if UNITY_ANDROID
        bannerAdList.Add(EBannerKind.BANNER, new MaxBannerAdClass(StaticGameSettings.AOSBannerKey));

        interstitialAdList.Add(EInterstitialKind.INTERSTITIAL, new MaxInterstitialAdClass(StaticGameSettings.AOSInterstitialKey));

        rewardAdList.Add(ERewardedKind.REWARD, new MaxRewardAdClass(StaticGameSettings.AOSRewardKey));

#elif UNITY_IOS
        bannerAdList.Add(EBannerKind.BANNER, new MaxBannerAdClass(StaticGameSettings.IOSBannerKey));

        interstitialAdList.Add(EInterstitialKind.INTERSTITIAL, new MaxInterstitialAdClass(StaticGameSettings.IOSInterstitialKey));

        rewardAdList.Add(ERewardedKind.REWARD, new MaxRewardAdClass(StaticGameSettings.IOSRewardKey));
#endif

        MaxSdkCallbacks.OnSdkInitializedEvent += (configuration =>
        {
            //배너 광고의 로드 및 로드실패 콜백 함수 설정
            foreach (EBannerKind kind in Enum.GetValues(typeof(EBannerKind)))
            {
                MaxSdkCallbacks.Banner.OnAdLoadedEvent += ((s, info) =>
                {
                    BannerOnAdLoaded(kind);
                    bannerAdList[kind].IsLoad = true;
                });
                MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += ((s, info) =>
                {
                    BannerOnAdLoadFail(kind, info.Message);
                });
                MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += ((s, info) =>
                {
                    double revenue = info.Revenue;

                    SingularAdData data = new SingularAdData("AppLovin", "USD", revenue);

                    data.WithAdUnitId(info.AdUnitIdentifier)
                        .WithNetworkName(info.NetworkName)
                        .WithAdPlacmentName(info.Placement);

                    SingularSDK.AdRevenue(data);
                });

                RequestBannerAd(kind);
            }
            foreach (EInterstitialKind kind in Enum.GetValues(typeof(EInterstitialKind)))
            {
                //전면 광고의 로드, 로드실패, 광고 종료 및 오프닝 콜백 함수 설정
                MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += ((s, info) =>
                {
                    InterstitialOnAdLoaded(kind);
                    interstitialAdList[kind].IsLoad = true;
                });
                MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += ((s, info) =>
                {
                    InterstitialOnAdLoadFail(kind, info.Message);
                });
                MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += ((s, info) =>
                {
                    InterstitialOnAdClose(kind);
                });
                MaxSdkCallbacks.Interstitial.OnAdClickedEvent += ((s, info) =>
                {
                    interstitialAdList[kind].IsLoad = false;
                });
                MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += ((s, info) =>
                {
                    double revenue = info.Revenue;

                    SingularAdData data = new SingularAdData("AppLovin", "USD", revenue);

                    data.WithAdUnitId(info.AdUnitIdentifier)
                        .WithNetworkName(info.NetworkName)
                        .WithAdPlacmentName(info.Placement);

                    SingularSDK.AdRevenue(data);
                });

                RequestInterstitialAd(kind);
            }
            foreach (ERewardedKind kind in Enum.GetValues(typeof(ERewardedKind)))
            {
                //보상형 광고의 로드, 로드실패, 광고 종료 및 오프닝 콜백 함수 설정
                MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += ((s, reward1, arg3) =>
                {
                    Debug.LogWarningFormat("KKI EDailyQuestType.WATCHAD");
                    DailyQuestManager.CollectMission(EDailyQuestType.WATCHAD, 1);
                    DailyQuestManager.Save();
                    if (reward != null)
                    {
                        reward.Invoke();
                        reward = null;
                    }
                });
                MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += ((s, info) =>
                {
                    RewardOnAdLoaded(kind);
                    rewardAdList[kind].IsLoad = true;
                });
                MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += ((s, info) =>
                {
                    RewardOnAdLoadFail(kind, info.Message);
                });
                MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += ((s, info) =>
                {
                    RewardOnAdClose(kind);
                });
                MaxSdkCallbacks.Rewarded.OnAdClickedEvent += ((s, info) =>
                {
                    Debug.Log("클릭한 순간");
                    rewardAdList[kind].IsLoad = false;
                });
                MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += ((s, info) =>
                {
                    Debug.Log("광고가 시작되는 순간");
                });
                MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += ((s, info) =>
                {
                    double revenue = info.Revenue;

                    SingularAdData data = new SingularAdData("AppLovin", "USD", revenue);

                    data.WithAdUnitId(info.AdUnitIdentifier)
                        .WithNetworkName(info.NetworkName)
                        .WithAdPlacmentName(info.Placement);

                    SingularSDK.AdRevenue(data);
                });

                RequestRewardAd(kind);
            }
        });
        MaxSdk.SetSdkKey("fnJ_dkuT94SrB_uXUzr6QbTrUQRM2N5i8Phm8ZJHdm6pm_LEzAKNnpCGDecG0iz97fvFDDXszeeke_zbAIzx59");
#if UNITY_IOS && !UNITY_EDITOR
        AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true); // 22.03.28 ATE
#endif
        MaxSdk.InitializeSdk();
    }

    public override void RequestBannerAd(EBannerKind kind)
    {
        base.RequestBannerAd(kind);
        if (bannerAdList.ContainsKey(kind))
        {
            //배너 광고 로드
            MaxSdk.CreateBanner(bannerAdList[kind].AdKey, MaxSdkBase.BannerPosition.TopCenter);
        }
    }

    public override void RequestInterstitialAd(EInterstitialKind kind)
    {
        base.RequestInterstitialAd(kind);
        if (interstitialAdList.ContainsKey(kind))
        {
            //전면 광고 로드
            MaxSdk.LoadInterstitial(interstitialAdList[kind].AdKey);
        }
    }

    public override void RequestRewardAd(ERewardedKind kind)
    {
        base.RequestRewardAd(kind);
        if (rewardAdList.ContainsKey(kind))
        {
            //보상형 광고 로드
            MaxSdk.LoadRewardedAd(rewardAdList[kind].AdKey);
        }
    }

    public override void ShowBannerAd(EBannerKind kind)
    {
        base.ShowBannerAd(kind);
        if (bannerAdList.ContainsKey(kind))
        {
            //배너 광고가 로드 되었다면 표시, 그렇지 않다면 광고 로드 요청
            if (bannerAdList[kind].IsLoad)
            {
                base.SuccessShowBannerAd(kind);
                MaxSdk.ShowBanner(bannerAdList[kind].AdKey);
            }
            else
            {
                base.FailShowBannerAd(kind, "광고가 로드되지 않음.");
                RequestBannerAd(kind);
            }
        }
        else
        {
            base.FailShowBannerAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
        }
    }

    public override void HideBannerAd(EBannerKind kind)
    {
        base.HideBannerAd(kind);
        if (bannerAdList.ContainsKey(kind))
        {
            if (bannerAdList[kind].IsLoad)
            {
                base.SuccessHideBannerAd(kind);
                MaxSdk.HideBanner(bannerAdList[kind].AdKey);
            }
            else
            {
                base.FailHideBannerAd(kind, "광고가 로드되지 않음.");
            }
        }
        else
        {
            base.FailHideBannerAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
        }
    }

    public override void ShowInterstitialAd(EInterstitialKind kind)
    {
        base.ShowInterstitialAd(kind);
        if (interstitialAdList.ContainsKey(kind))
        {
            //전면 광고가 로드 되었다면 광고 재생, 그렇지 않다면 광고 로드 요청
            if (interstitialAdList[kind].IsLoad)
            {
                base.SuccessShowInterstitialAd(kind);
                MaxSdk.ShowInterstitial(interstitialAdList[kind].AdKey);
            }
            else
            {
                base.FailShowInterstitialAd(kind, "광고가 로드되지 않음.");
                RequestInterstitialAd(kind);
            }
        }
        else
        {
            base.FailShowInterstitialAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
        }
    }

    public void SetReward(Action reward)
    {
        this.reward = reward;
    }

    public override void ShowRewardAd(ERewardedKind kind, Action reward)
    {
        base.ShowRewardAd(kind, reward);
        if (rewardAdList.ContainsKey(kind))
        {
            //보상형 광고가 로드 되었다면 광고 재생, 그렇지 않다면 광고 로드 요청
            if (rewardAdList[kind].IsLoad)
            {
                base.SuccessShowRewardAd(kind);
                SetReward(reward);
                MaxSdk.ShowRewardedAd(rewardAdList[kind].AdKey);
            }
            else
            {
                base.FailShowRewardAd(kind, "광고가 로드되지 않음.");
                RequestRewardAd(kind);
            }
        }
        else
        {
            base.FailShowRewardAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
        }
    }

    public override bool IsInterstitialAdLoaded(EInterstitialKind kind)
    {
        if (interstitialAdList.ContainsKey(kind))
        {
            return interstitialAdList[kind].IsLoad;
        }

        return false;
    }

    public override bool IsRewardAdLoaded(ERewardedKind kind)
    {
        if (rewardAdList.ContainsKey(kind))
        {
            return rewardAdList[kind].IsLoad;
        }

        return false;
    }
}