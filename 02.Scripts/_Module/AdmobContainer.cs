using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
using GoogleMobileAds.Api;

public class AdmobBannerAdClass
{
    public string AdKey;
    public BannerView BannerAd;
    public bool IsLoad;

    public AdmobBannerAdClass(string key)
    {
        AdKey = key;
        BannerAd = new BannerView(AdKey, AdSize.Banner, AdPosition.Top);
    }
}

public class AdmobInterstitialAdClass
{
    public string AdKey;
    public InterstitialAd InterstitialAd;
    public bool IsLoad;

    public AdmobInterstitialAdClass(string key)
    {
        AdKey = key;
        InterstitialAd = new InterstitialAd(AdKey);
    }
}

public class AdmobRewardAdClass
{
    public string AdKey;
    public RewardedAd RewardedAd;
    public bool IsLoad;

    public AdmobRewardAdClass(string key)
    {
        AdKey = key;
        RewardedAd = new RewardedAd(AdKey);
    }
}

public class AdmobContainer : AdBase
{
    private Dictionary<EBannerKind, AdmobBannerAdClass> bannerAdList = new Dictionary<EBannerKind, AdmobBannerAdClass>();
    private Dictionary<EInterstitialKind, AdmobInterstitialAdClass> interstitialAdList = new Dictionary<EInterstitialKind, AdmobInterstitialAdClass>();
    private Dictionary<ERewardedKind, AdmobRewardAdClass> rewardAdList = new Dictionary<ERewardedKind, AdmobRewardAdClass>();

    private Action reward;

    public override void Init()
    {
        base.Init();

#if UNITY_ANDROID
        bannerAdList.Add(EBannerKind.BANNER, new AdmobBannerAdClass(StaticGameSettings.AOSBannerKey));

        interstitialAdList.Add(EInterstitialKind.INTERSTITIAL, new AdmobInterstitialAdClass(StaticGameSettings.AOSInterstitialKey));

        rewardAdList.Add(ERewardedKind.REWARD, new AdmobRewardAdClass(StaticGameSettings.AOSRewardKey));

#elif UNITY_IOS
         bannerAdList.Add(EBannerKind.BANNER, new AdmobBannerAdClass(StaticGameSettings.IOSBannerKey));

         interstitialAdList.Add(EInterstitialKind.INTERSTITIAL, new AdmobInterstitialAdClass(StaticGameSettings.IOSInterstitialKey));

         rewardAdList.Add(ERewardedKind.REWARD, new AdmobRewardAdClass(StaticGameSettings.IOSRewardKey));
#endif
        MobileAds.Initialize(initStatus =>
        {
            foreach (EBannerKind kind in Enum.GetValues(typeof(EBannerKind)))
            {
                 //배너 광고의 로드 및 로드 실패 콜백 함수 설정
                 bannerAdList[kind].BannerAd.OnAdLoaded += ((sender, args) =>
                {
                    BannerOnAdLoaded(kind);
                    bannerAdList[kind].IsLoad = true;
                });
                bannerAdList[kind].BannerAd.OnAdFailedToLoad += ((sender, args) =>
                {
                     //BannerOnAdLoadFail(kind, args.Message);
                 });
            }

            foreach (EInterstitialKind kind in Enum.GetValues(typeof(EInterstitialKind)))
            {
                 //전면 광고의 로드, 로드실패, 광고 종료 및 오프닝 콜백 함수 설정
                 interstitialAdList[kind].InterstitialAd.OnAdLoaded += ((sender, args) =>
                {
                    InterstitialOnAdLoaded(kind);
                    interstitialAdList[kind].IsLoad = true;
                });
                interstitialAdList[kind].InterstitialAd.OnAdFailedToLoad += ((sender, args) =>
                {
                     //InterstitialOnAdLoadFail(kind, args.Message);
                 });
                interstitialAdList[kind].InterstitialAd.OnAdClosed += ((sender, args) =>
                {
                    InterstitialOnAdClose(kind);
                });
                interstitialAdList[kind].InterstitialAd.OnAdOpening += ((sender, args) =>
                {
                    interstitialAdList[kind].IsLoad = false;
                });

                RequestInterstitialAd(kind);
            }

            foreach (ERewardedKind kind in Enum.GetValues(typeof(ERewardedKind)))
            {
                 //보상형 광고의 로드, 로드실패, 광고 종료 및 오프닝 콜백 함수 설정
                 rewardAdList[kind].RewardedAd.OnAdLoaded += ((sender, args) =>
                {
                    RewardOnAdLoaded(kind);
                    rewardAdList[kind].IsLoad = true;
                });
                rewardAdList[kind].RewardedAd.OnAdFailedToLoad += ((sender, args) =>
                {
                     //RewardOnAdLoadFail(kind, args.Message);
                 });
                rewardAdList[kind].RewardedAd.OnAdClosed += ((sender, args) => { RewardOnAdClose(kind); });
                rewardAdList[kind].RewardedAd.OnAdOpening += ((sender, args) => { rewardAdList[kind].IsLoad = false; });
                rewardAdList[kind].RewardedAd.OnUserEarnedReward += (sender, args) =>
                {
                    reward.Invoke();

                DailyQuestManager.CollectMission(EDailyQuestType.WATCHAD, 1);
                    DailyQuestManager.Save();
                };

                RequestRewardAd(kind);
            }
        });
    }

    public override void RequestBannerAd(EBannerKind kind)
    {
        base.RequestBannerAd(kind);
        if (bannerAdList.ContainsKey(kind))
        {
            //배너 광고 로드
            AdRequest request = new AdRequest.Builder().Build();
            bannerAdList[kind].BannerAd.LoadAd(request);
        }
    }

    public override void RequestInterstitialAd(EInterstitialKind kind)
    {
        base.RequestInterstitialAd(kind);
        if (interstitialAdList.ContainsKey(kind))
        {
            //전면 광고 로드
            AdRequest request = new AdRequest.Builder().Build();
            interstitialAdList[kind].InterstitialAd.LoadAd(request);
        }
    }

    public override void RequestRewardAd(ERewardedKind kind)
    {
        base.RequestRewardAd(kind);
        if (rewardAdList.ContainsKey(kind))
        {
            //전면 광고 로드
            AdRequest request = new AdRequest.Builder().Build();
            rewardAdList[kind].RewardedAd.LoadAd(request);
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
                bannerAdList[kind].BannerAd.Show();
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
                bannerAdList[kind].BannerAd.Hide();
            }
            else
            {
                base.FailHideBannerAd(kind, "광고가 로드되지 않음.");
            }
        }
        base.FailHideBannerAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
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
                interstitialAdList[kind].InterstitialAd.Show();
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
                rewardAdList[kind].RewardedAd.Show();
            }
            else
            {
                base.FailShowRewardAd(kind, "광고가 로드되지 않음.");
                RequestRewardAd(kind);
            }
        }
        else base.FailShowRewardAd(kind, "해당 광고 종류가 리스트에 추가되지 않음.");
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
}*/