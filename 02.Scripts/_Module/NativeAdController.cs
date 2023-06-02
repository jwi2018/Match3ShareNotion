using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NativeAdController : MonoBehaviour
{
    [SerializeField] private Image iconImage = null;
    [SerializeField] private Image mainImage = null;
    [SerializeField] private Text adTitleText = null;
    [SerializeField] private Text adInfomationText = null;
    [SerializeField] private GameObject adBoxCollider = null;
    [SerializeField] private Button adbutton = null;
    [SerializeField] private EUnifiedNativeKind nativeKind;

    [SerializeField] private string ourGamesAdress_AOS = "";
    [SerializeField] private string ourGamesAdress_IOS = "";

    private bool isAdLoaded = false;

    public void Start()
    {
        adbutton?.onClick.AddListener(() => OpenOurGameURL());
        if (PlayerData.GetInstance == null)
        {
            Debug.Log("[광고 확인] 플레이어 데이터 NUll");
            return;
        }
        /*
        if (!isAdLoaded)
        {
            if (AdmobNativeManager.GetInstance.IsNativeLoaded(nativeKind))
            {
                ADManager.GetInstance.HideBanner(EBannerKind.BANNER);
                isAdLoaded = true;
                var item = AdmobNativeManager.GetInstance.GetNativeAd(nativeKind);

                Texture2D IconTexture = null;
                Texture2D ImageTexture = null;

                string BodyText = null;
                string HeadLineText = null;

                if (item.GetIconTexture() != null)
                {
                    IconTexture = item.GetIconTexture();
                    Rect IconRect = new Rect(0, 0, IconTexture.width, IconTexture.height);
                    iconImage.overrideSprite = Sprite.Create(IconTexture, IconRect, new Vector2(0.5f, 0.5f), 100);
                }
                else
                    iconImage.gameObject.SetActive(false);

                if (item.GetHeadlineText() != null)
                {
                    HeadLineText = item.GetHeadlineText();
                    adTitleText.text = HeadLineText;
                }
                else
                    adTitleText.gameObject.SetActive(false);

                if (item.GetImageTextures().Count > 0)
                {
                    ImageTexture = item.GetImageTextures()[0];
                    Rect ImageRect = new Rect(0, 0, ImageTexture.width, ImageTexture.height);
                    mainImage.overrideSprite = Sprite.Create(ImageTexture, ImageRect, new Vector2(0.5f, 0.5f), 100);
                }
                else
                    mainImage.gameObject.SetActive(false);

                if (item.GetBodyText() != null)
                {
                    BodyText = item.GetBodyText();
                    adInfomationText.text = BodyText;
                }
                else
                    adInfomationText.gameObject.SetActive(false);

                if (!item.RegisterIconImageGameObject(adBoxCollider))
                {
                    Debug.Log("Handle Failure");
                    // Handle failure to register ad asset.
                }

                if (!item.RegisterBodyTextGameObject(adbutton.gameObject))
                {
                    Debug.Log("Handle Failure");
                    // Handle failure to register ad asset.
                }

                AdmobNativeManager.GetInstance.RequestNativeAd(nativeKind);
            }
            else if (!AdmobNativeManager.GetInstance.IsNativeLoaded(nativeKind))
            {
                AdmobNativeManager.GetInstance.RequestNativeAd(nativeKind);
            }
        }*/
    }

    private void Update()
    {
    }

    public void OpenOurGameURL()
    {
        if (isAdLoaded) return;
#if UNITY_ANDROID
        Application.OpenURL(ourGamesAdress_AOS);
#elif UNITY_IOS
        Application.OpenURL(ourGamesAdress_IOS);
#endif
        switch (nativeKind)
        {
            case EUnifiedNativeKind.Native:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("NativeBanner_Button");
                break;
            case EUnifiedNativeKind.MissionClear:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("NativeBanner_Button");
                Debug.Log("result_banner_01");
                break;
            case EUnifiedNativeKind.MissionFail:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("NativeBanner_Button");
                Debug.Log("result_banner_02");
                break;
            case EUnifiedNativeKind.GameExit:
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("NativeBanner_Button");
                Debug.Log("gameover_banner_03");
                break;
        }
    }
}