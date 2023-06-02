using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdBase
{
    public virtual void Init()
    {
        
    }

    /// <summary>
    /// 배너 광고를 요청하는 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void RequestBannerAd(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 배너 광고 요청");
    }
    
    /// <summary>
    /// 전면 광고를 요청하는 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    public virtual void RequestInterstitialAd(EInterstitialKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 전면 광고 요청");
    }

    /// <summary>
    /// 보상형 광로를 요청하는 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    public virtual void RequestRewardAd(ERewardedKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 보상형 광고 요청");
    }

    /// <summary>
    /// 배너 광고를 표시하는 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void ShowBannerAd(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 배너 광고 표시");
    }
    
    /// <summary>
    /// 배너 광고를 숨기는 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void HideBannerAd(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 배너 광고 숨김");
    }

    /// <summary>
    /// 전면 광고를 재생하는 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    public virtual void ShowInterstitialAd(EInterstitialKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 전면 광고 재생");
    }
    
    /// <summary>
    /// 보상형 광고를 재생하는 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    /// <param name="reward">광고 시청 보상</param>
    public virtual void ShowRewardAd(ERewardedKind kind, Action reward)
    {
        Debug.Log("[광고 알림] " + kind +" 보상형 광고 재생");
    }

    /// <summary>
    /// 배너 광고 로드 완료 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void BannerOnAdLoaded(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 배너 광고 로드 완료");
    }

    /// <summary>
    /// 전면 광고 로드 완료 로그를 위한 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    public virtual void InterstitialOnAdLoaded(EInterstitialKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 전면 광고 로드 완료");
    }

    /// <summary>
    /// 보상형 광고 로드 완료 로그를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    public virtual void RewardOnAdLoaded(ERewardedKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 보상형 광고 로드 완료");
    }

    /// <summary>
    /// 배너 광고 로드 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    /// <param name="msg">실패 이유</param>
    public virtual void BannerOnAdLoadFail(EBannerKind kind, string msg = "")
    {
        Debug.Log("[광고 알림] " + kind +" 배너 광고 로드 실패 : " + msg);
    }

    /// <summary>
    /// 전면 광고 로드 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    /// <param name="msg">실패 이유</param>
    public virtual void InterstitialOnAdLoadFail(EInterstitialKind kind, string msg)
    {
        Debug.Log("[광고 알림] " + kind +" 전면 광고 로드 실패 : " + msg);
    }

    /// <summary>
    /// 보상형 광고 로드 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    /// <param name="msg">실패 이유</param>
    public virtual void RewardOnAdLoadFail(ERewardedKind kind, string msg)
    {
        Debug.Log("[광고 알림] " + kind +" 보상형 광고 로드 실패 : " + msg);
    }

    /// <summary>
    /// 전면 광고가 종료되었을 때를 위한 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    public virtual void InterstitialOnAdClose(EInterstitialKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 전면 광고 종료");
        RequestInterstitialAd(kind);
    }

    /// <summary>
    /// 보상형 광고 종료되었을 때를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    public virtual void RewardOnAdClose(ERewardedKind kind)
    {
        Debug.Log("[광고 알림] " + kind +" 보상형 광고 종료");
        RequestRewardAd(kind);
    }
    
    /// <summary>
    /// 전면 광고가 로드 되었는지 확인하는 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    /// <returns>로드 여부</returns>
    public virtual bool IsInterstitialAdLoaded(EInterstitialKind kind)
    {
        return false;
    }

    /// <summary>
    /// 보상형 광고가 로드 되었는지 확인하는 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    /// <returns>로드 여부</returns>
    public virtual bool IsRewardAdLoaded(ERewardedKind kind)
    {
        return false;
    }

    /// <summary>
    /// 배너 광고 표시 성공 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void SuccessShowBannerAd(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind + " 배너 광고 표시 성공");
    }
    /// <summary>
    /// 배너 광고 숨김 성공 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    public virtual void SuccessHideBannerAd(EBannerKind kind)
    {
        Debug.Log("[광고 알림] " + kind + " 배너 광고 숨김 성공");
    }
    /// <summary>
    /// 전면 광고 재생 성공 로그를 위한 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    public virtual void SuccessShowInterstitialAd(EInterstitialKind kind)
    {
        Debug.Log("[광고 알림] " + kind + " 전면 광고 표시 성공");
    }
    /// <summary>
    /// 보상형 광고 재생 성공 로그를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    public virtual void SuccessShowRewardAd(ERewardedKind kind)
    {
        Debug.Log("[광고 알림] " + kind + " 보상형 광고 표시 성공");
    }
    /// <summary>
    /// 배너 광고 표시 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    /// <param name="reason">실패 이유</param>
    public virtual void FailShowBannerAd(EBannerKind kind, string reason = "")
    {
        Debug.Log("[광고 알림] " + kind + " 배너 광고 표시 실패 : " + reason);
    }
    /// <summary>
    /// 배너 광고 숨김 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">배너 광고 종류</param>
    /// <param name="reason">실패 이유</param>
    public virtual void FailHideBannerAd(EBannerKind kind, string reason = "")
    {
        Debug.Log("[광고 알림] " + kind + " 배너 광고 숨김 실패 : " + reason);
    }
    /// <summary>
    /// 전면 광고 재생 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">전면 광고 종류</param>
    /// <param name="reason">실패 이유</param>
    public virtual void FailShowInterstitialAd(EInterstitialKind kind, string reason = "")
    {
        Debug.Log("[광고 알림] " + kind + " 전면 광고 표시 실패 : " + reason);
    }
    /// <summary>
    /// 보상형 광고 재생 실패 로그를 위한 함수
    /// </summary>
    /// <param name="kind">보상형 광고 종류</param>
    /// <param name="reason">실패 이유</param>
    public virtual void FailShowRewardAd(ERewardedKind kind, string reason = "")
    {
        Debug.Log("[광고 알림] " + kind + " 보상형 광고 표시 실패 : " + reason);
    }
}
