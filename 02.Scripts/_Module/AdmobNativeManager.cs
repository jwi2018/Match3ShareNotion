using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeAdClass
{
    public string AdKey;
    //public NativeAd NativeAd;
    public bool IsRequestEnd;
    public bool IsLoad;
    
    public NativeAdClass(string key)
    {
        AdKey = key;
        IsRequestEnd = true;
    }
}
public class AdmobNativeManager : Singleton<AdmobNativeManager>
{
    private Dictionary<EUnifiedNativeKind, NativeAdClass> nativeAdList = new Dictionary<EUnifiedNativeKind, NativeAdClass>();
    
    
    public void Start()
    {
        /*Debug.Log("네이티브 매니저 INIT");
        #if UNITY_ANDROID
        nativeAdList.Add(EUnifiedNativeKind.Native, new NativeAdClass(StaticGameSettings.AOSNativeKey));
        
        #elif UNITY_IOS
        nativeAdList.Add(EUnifiedNativeKind.Native, new NativeAdClass(StaticGameSettings.IOSNativeKey));
#endif
        
        MobileAds.Initialize(initStatus =>
        {
            foreach (EUnifiedNativeKind item in Enum.GetValues(typeof(EUnifiedNativeKind)))
            {
                RequestNativeAd(item);
            }
        });*/
    }

    /// <summary>
    /// 네이티브 광고를 요청하는 함수
    /// </summary>
    /// <param name="kind">네이티브 광고 타입</param>
    public void RequestNativeAd(EUnifiedNativeKind kind)
    {
        if (nativeAdList.ContainsKey(kind))
        {/*
            if (nativeAdList[kind].IsRequestEnd)
            {
                var nativeKey = nativeAdList[kind].AdKey;
                AdLoader adLoader = new AdLoader.Builder(nativeKey)
                    .ForNativeAd()
                    .Build();

                adLoader.OnNativeAdLoaded += ((sender, args) =>
                {
                    Debug.Log("[광고 알림] 네이티브 " + kind + " 광고 로드 완료");
                    //nativeAdList[kind].NativeAd = args.nativeAd;
                    nativeAdList[kind].IsLoad = true;
                    nativeAdList[kind].IsRequestEnd = true;
                });
                adLoader.OnAdFailedToLoad += ((sender, args) =>
                {
                    Debug.Log("[광고 알림] 네이티브 " + kind + " 광고 로드 실패");
                    nativeAdList[kind].IsRequestEnd = true;
                });
                adLoader.LoadAd(new AdRequest.Builder().Build());
                nativeAdList[kind].IsRequestEnd = false;
            }*/
        }
    }

    /// <summary>
    /// 네이티브 광고를 가져오기 위한 함수
    /// </summary>
    /// <param name="kind">네이티브 광고 종류</param>
    /// <returns>네이티브 광고</returns>
    /*
    public NativeAd GetNativeAd(EUnifiedNativeKind kind)
    {
        if (nativeAdList.ContainsKey(kind))
        {
            return nativeAdList[kind].NativeAd;
        }

        return null;
    }
    */

    /// <summary>
    /// 네이티브 광고가 로드 되었는지 확인하는 함수
    /// </summary>
    /// <param name="kind">네이티브 광고 종류</param>
    /// <returns>네이티브 광고 로드 여부</returns>
    public bool IsNativeLoaded(EUnifiedNativeKind kind)
    {
        if (nativeAdList.ContainsKey(kind))
        {
            Debug.Log("[광고 알림] 네이티브 " + kind + " 광고 로드 여부 : " + nativeAdList[kind].IsLoad);
            return nativeAdList[kind].IsLoad;
        }
        else
        {
            Debug.Log("[광고 알림] 네이티브 " + kind + "키값이 존재하지 않음.");
            return false;
        }

        return false;
    }
}
