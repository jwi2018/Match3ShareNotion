using DarkTonic.MasterAudio;
using System.Collections;
using System.Collections.Generic;
using CompleteProject;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class SetupPopup : PopupSetting
{
    [SerializeField] private Toggle soundBGM = null;
    [SerializeField] private Toggle soundSFX = null;
    [SerializeField] private Toggle push = null;
    [SerializeField] private Toggle vibrate = null;
    [SerializeField] private Sprite[] bgmImage;
    [SerializeField] private Sprite[] sfxImage;
    [SerializeField] private Sprite[] pushImage;
    [SerializeField] private Sprite[] vibImage;
    [SerializeField] private Image soundBGMImageobj = null;
    [SerializeField] private Image soundSFXImageobj = null;
    [SerializeField] private Image pushImageobj = null;
    [SerializeField] private Image vibrateImageobj = null;

    [SerializeField] private GameObject loginButton = null;
    [SerializeField] private GameObject logoutButton = null;
    [SerializeField] private Text versionText = null;

    [SerializeField] private Button button_Kind_Setting = null;
    [SerializeField] private GameObject button_Kind_Setting_Inactive = null;
    [SerializeField] private GameObject rect_Kind_Setting = null;

    [SerializeField] private Button button_Kind_Promotion = null;
    [SerializeField] private GameObject button_Kind_Promotion_Inactive = null;
    [SerializeField] private GameObject rect_Kind_Promotion = null;

    [SerializeField] private Button button_Kind_GPGS = null;
    [SerializeField] private GameObject button_Kind_GPGS_Inactive = null;
    [SerializeField] private GameObject rect_Kind_GPGS = null;

    [SerializeField] private Sprite[] buttonImages;

    [SerializeField] private GameObject gobPromotionAlram = null;
    
    protected ShopItemInfo weekItemInfo;

    private void Start()
    {
        gobPromotionAlram.SetActiveSelf(false);
        OnPopupSetting();
    }

    public void RefreshPromotionAlram()
    {
        bool facebookCoin = FaceBookCoin.IsCoinActive();
        bool youtubeCoin = YoutubeCoin.IsCoinActive();
        bool moreGameCoin = MoreGameCoin.IsCoinActive();
        if (facebookCoin || youtubeCoin || moreGameCoin)
        {
            gobPromotionAlram.SetActiveSelf(true);
        }
        else
        {
            gobPromotionAlram.SetActiveSelf(false);
        }
    }

    public override void OnPopupSetting()
    {
        if (PlayerData.GetInstance != null)
        {
            
            if (BaseSystem.GetInstance != null)
            {
                if (BaseSystem.GetInstance.GetSystemList("CircusSystem"))
                {
                    FirebaseManager.GetInstance.FirebaseLogEvent("setting_enter");
                }
            }
            else
            {
                FirebaseManager.GetInstance.FirebaseLogEvent("Intro_setting_enter");
            }
            
            if (!PlayerData.GetInstance.IsPlayBGM)
            {
                soundBGM.GetComponent<Toggle>().isOn = false;
            }
            else
            {
                soundBGM.GetComponent<Toggle>().isOn = true;
            }

            if (!PlayerData.GetInstance.IsPlaySFX)
            {
                soundSFX.GetComponent<Toggle>().isOn = false;
            }
            else
            {
                soundSFX.GetComponent<Toggle>().isOn = true;
            }

            if (vibrate != null)
            {
                if (!PlayerData.GetInstance.IsPlayVib)
                {
                    vibrate.GetComponent<Toggle>().isOn = false;
                }
                else
                {
                    vibrate.GetComponent<Toggle>().isOn = true;
                }
            }

            if (!PlayerData.GetInstance.IsPush)
            {
                push.GetComponent<Toggle>().isOn = false;
            }
            else
            {
                push.GetComponent<Toggle>().isOn = true;
            }
            if (PlayerData.GetInstance.IsGpgsLogin)
            {
                if (loginButton != null)
                {
                    loginButton.SetActive(false);
                }
                if (logoutButton != null)
                {
                    logoutButton.SetActive(true);
                }
            }
            else
            {
                if (loginButton != null)
                {
                    loginButton.SetActive(true);
                }
                if (logoutButton != null)
                {
                    logoutButton.SetActive(false);
                }
            }
        }
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("Popup");
        }
        if (versionText != null)
        {
            versionText.text = "v" + Application.version;
        }
        button_Kind_Setting?.onClick.AddListener(() => ChangedKind(ESetupKinds.SETTING));
        button_Kind_Promotion?.onClick.AddListener(() =>
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Channel");
            ChangedKind(ESetupKinds.PROMOTION);
        });
        button_Kind_GPGS?.onClick.AddListener(() =>
        {
            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_GoogleGameService");
            ChangedKind(ESetupKinds.GPGS);  
        });

        ChangedKind(ESetupKinds.SETTING);

#if UNITY_IOS
        button_Kind_GPGS.gameObject.SetActive(false);
#endif
    }

    public override void OffPopupSetting()
    {
        this.GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("Popup");
        }
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.Play("ButtonPush");
        }

        StartCoroutine(CheckPromotionAlram(0.73f));
    }

    private IEnumerator CheckPromotionAlram(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        RefreshPromotionAlram();
    }

    private void Update()
    {
        if (PlayerData.GetInstance != null)
        {
            if (Social.localUser.authenticated)
            {
                if (loginButton != null)
                {
                    loginButton.SetActive(false);
                }
                if (logoutButton != null)
                {
                    logoutButton.SetActive(true);
                }
            }
            else
            {
                if (loginButton != null)
                {
                    loginButton.SetActive(true);
                }
                if (logoutButton != null)
                {
                    logoutButton.SetActive(false);
                }
            }
        }
    }

    public void ChangeToggle(Toggle toggle)
    {
        if (toggle == soundBGM.GetComponent<Toggle>())
        {
            
            if (soundBGM.GetComponent<Toggle>().isOn)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_BG_On");
                soundBGMImageobj.sprite = bgmImage[0];
                SoundManager.GetInstance.BGMSource.volume = 0.8f;
                MasterAudio.PlaylistMasterVolume = 1f;
                PlayerData.GetInstance.IsPlayBGM = true;
            }
            else
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_BG_Off");
                soundBGMImageobj.sprite = bgmImage[1];
                SoundManager.GetInstance.BGMSource.volume = 0;
                MasterAudio.PlaylistMasterVolume = 0f;
                PlayerData.GetInstance.IsPlayBGM = false;
            }
        }
        else if (toggle == soundSFX.GetComponent<Toggle>())
        {
            
            if (soundSFX.GetComponent<Toggle>().isOn)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Ef_On");
                soundSFXImageobj.sprite = sfxImage[0];
                foreach (AudioSource nCnt in SoundManager.GetInstance.SFXSource)
                {
                    nCnt.volume = 1;
                }
                MasterAudio.SetBusVolumeByName("SFX", 1f);
                MasterAudio.SetBusVolumeByName("Loop", 1f);
                PlayerData.GetInstance.IsPlaySFX = true;
            }
            else
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Ef_Off");
                soundSFXImageobj.sprite = sfxImage[1];
                foreach (AudioSource nCnt in SoundManager.GetInstance.SFXSource)
                {
                    nCnt.volume = 0;
                }
                MasterAudio.SetBusVolumeByName("SFX", 0f);
                MasterAudio.SetBusVolumeByName("Loop", 0f);
                PlayerData.GetInstance.IsPlaySFX = false;
            }
        }
        else if (toggle == push.GetComponent<Toggle>())
        {
            if (push.GetComponent<Toggle>().isOn)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Push_On");
                pushImageobj.sprite = pushImage[0];
                PlayerData.GetInstance.IsPush = true;
            }
            else
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Push_Off");
                pushImageobj.sprite = pushImage[1];
                PlayerData.GetInstance.IsPush = false;
            }
        }
        else if (toggle == vibrate.GetComponent<Toggle>())
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("Setting_01vibration_button");
            if (vibrate.GetComponent<Toggle>().isOn)
            {
                vibrateImageobj.sprite = vibImage[0];
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayVib = true;
                Debug.Log("true");
            }
            else
            {
                vibrateImageobj.sprite = vibImage[1];
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayVib = false;
                Debug.Log("false");
            }
        }
    }

    public void OnClickPrivacyPoilicy()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Privacy");

        if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            transform.parent.GetComponent<PopupManager>().OnClickPrivacyPolicyURL();
        else
            transform.parent.GetComponent<PopupManager>().OnClickPrivacyPolicy();
    }

    public void OnClickTermsOfService()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Gameservice");

        if (BaseSystem.GetInstance.GetSystemList("Fantasy"))
            transform.parent.GetComponent<PopupManager>().OnClickTermsOfServiceURL();
        else
            transform.parent.GetComponent<PopupManager>().OnClickTermsOfService();
    }

    public void OnClickServiceCenter()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_CSEMAIL");
        transform.parent.GetComponent<PopupManager>().OnClickServiceCenter();
    }

    public void OnClickLanguage()
    {
        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Setting_Local");
        transform.parent.GetComponent<PopupManager>().OnClickLanguage();
    }

    public void OnClickTutorial()
    {
        transform.parent.GetComponent<PopupManager>().OnClickTutorial();
    }

    public void OnClickSetup()
    {
        transform.parent.GetComponent<PopupManager>().OnClickLanguage();
    }

    public void OnClickAchievements()
    {
        if (GpgsManager.GetInstance != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("achievement_enter");
            GpgsManager.GetInstance.ShowAchievementUI();
        }
    }

    public PopupManager GetPopupManager()
    {
        return transform.parent.GetComponent<PopupManager>();
    }

    public void OnClickLogin()
    {
        if (GpgsManager.GetInstance != null)
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("login");
            GpgsManager.GetInstance.LoginGPGS(null);
        }
    }

    public void OnClickLogout()
    {
        if (GpgsManager.GetInstance != null)
        {
            GpgsManager.GetInstance.LogoutGPGS();
        }
    }

    public void OnClickSave()
    {
#if UNITY_ANDROID
        FirebaseManager.GetInstance.FirebaseLogEvent("Setting_03save_button");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            transform.parent.GetComponent<PopupManager>().CallTextLog(5);
        }
        else
        {
            this.transform.parent.GetComponent<PopupManager>().CallLoadingPop();
            GpgsSaveManager.GetInstance.SaveToCloud("SaveData");
        }
#endif
        //PlayerData.GetInstance.ToJsonData();
    }

    public void OnClickLoad()
    {
#if UNITY_ANDROID
        FirebaseManager.GetInstance.FirebaseLogEvent("Setting_03load_button");
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            transform.parent.GetComponent<PopupManager>().CallTextLog(5);
            return;
        }
        else if (!Social.localUser.authenticated)
        {
            transform.parent.GetComponent<PopupManager>().CallTextLog(3);
            return;
        }
        if (((GooglePlayGames.PlayGamesPlatform)Social.Active).SavedGame == null)
        {
            transform.parent.GetComponent<PopupManager>().CallTextLog(3);
            return;
        }
        transform.parent.GetComponent<PopupManager>().OnClickLoadQuestion();
#endif
    }

    public void ChangeToggleState(Button btn, GameObject gobInactive, bool isActive)
    {
        if (gobInactive == null)
        {
            // OLD 시스템
            Sprite spTarget = buttonImages[0];
            if (isActive == false)
            {
                spTarget = buttonImages[1];
            }
            btn.GetComponent<Image>().sprite = spTarget;
        }
        else
        {
            gobInactive.SetActiveSelf(!isActive);
        }
    }

    public void ChangedKind(ESetupKinds kinds)
    {
        switch (kinds)
        {
            case ESetupKinds.SETTING:
                FirebaseManager.GetInstance.FirebaseLogEvent("Setting_01menu_button");
                rect_Kind_Setting.SetActive(true);
                ChangeToggleState(button_Kind_Setting, button_Kind_Setting_Inactive, true);

                rect_Kind_Promotion.SetActive(false);
                ChangeToggleState(button_Kind_Promotion, button_Kind_Promotion_Inactive, false);

                rect_Kind_GPGS.SetActive(false);
                ChangeToggleState(button_Kind_GPGS, button_Kind_GPGS_Inactive, false);
                break;

            case ESetupKinds.PROMOTION:
                FirebaseManager.GetInstance.FirebaseLogEvent("Setting_02menu_button");
                rect_Kind_Setting.SetActive(false);
                ChangeToggleState(button_Kind_Setting, button_Kind_Setting_Inactive, false);
                rect_Kind_Promotion.SetActive(true);
                ChangeToggleState(button_Kind_Promotion, button_Kind_Promotion_Inactive, true);
                rect_Kind_GPGS.SetActive(false);
                ChangeToggleState(button_Kind_GPGS, button_Kind_GPGS_Inactive, false);
                break;

            case ESetupKinds.GPGS:
                FirebaseManager.GetInstance.FirebaseLogEvent("Setting_03menu_button");
                rect_Kind_Setting.SetActive(false);
                ChangeToggleState(button_Kind_Setting, button_Kind_Setting_Inactive, false);
                rect_Kind_Promotion.SetActive(false);
                ChangeToggleState(button_Kind_Promotion, button_Kind_Promotion_Inactive, false);
                rect_Kind_GPGS.SetActive(true);
                ChangeToggleState(button_Kind_GPGS, button_Kind_GPGS_Inactive, true);
                break;

            default:
                break;
        }
        OnButtonClick();
    }
}