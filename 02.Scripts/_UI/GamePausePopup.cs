using UnityEngine;
using UnityEngine.UI;

public class GamePausePopup : PopupSetting
{
    [SerializeField] private Toggle _soundBGM;

    [SerializeField] private Toggle _soundSFX;

    [SerializeField] private Toggle _vibrate;

    [SerializeField] private Image _soundBGMImage;

    [SerializeField] private Image _soundSFXImage;

    [SerializeField] private Image _vibrateImage;

    [SerializeField] private Sprite[] _bgmImage;

    [SerializeField] private Sprite[] _sfxImage;

    [SerializeField] private Sprite[] _vibImage;

    private void Start()
    {
        OnPopupSetting();
        ADManager.GetInstance.SetInterstitialTimer(false);
    }

    private void OnDisable()
    {
        if (null != ADManager.GetInstance)
        {
            ADManager.GetInstance.SetInterstitialTimer(true);
        }
    }

    public override void OnPopupSetting()
    {
        if (PlayerData.GetInstance != null)
        {
            if (!PlayerData.GetInstance.IsPlayBGM) _soundBGM.isOn = false;
            if (!PlayerData.GetInstance.IsPlaySFX) _soundSFX.isOn = false;
            if (!PlayerData.GetInstance.IsPlayVib) _vibrate.isOn = false;
        }
        //FirebaseManager.GetInstance.FirebaseLogEvent("Play_pause_button");

        ADManager.GetInstance.noAdsPopup = false;
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = false;
    }

    public override void OffPopupSetting()
    {
        GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        BlockManager.GetInstance.IsSwapAble = true;

        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_Continue");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("ButtonPush");
    }

    public void ToggleCheck()
    {
        if (PlayerData.GetInstance != null)
        {
            if (!PlayerData.GetInstance.IsPlayBGM) _soundBGM.isOn = false;
            if (!PlayerData.GetInstance.IsPlaySFX) _soundSFX.isOn = false;
            if (!PlayerData.GetInstance.IsPlayVib) _vibrate.isOn = false;
        }
    }

    public void ChangeToggle(Toggle toggle)
    {
        if (toggle == _soundBGM.GetComponent<Toggle>())
        {

            if (_soundBGM.GetComponent<Toggle>().isOn)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_BG_On");
                _soundBGMImage.sprite = _bgmImage[0];
                SoundManager.GetInstance.BGMSource.volume = 0.8f;
                SoundManager.BGMOnOff(true);
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayBGM = true;
            }
            else
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_BG_Off");
                _soundBGMImage.sprite = _bgmImage[1];
                SoundManager.GetInstance.BGMSource.volume = 0;
                SoundManager.BGMOnOff(false);
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayBGM = false;
            }
        }
        else if (toggle == _soundSFX.GetComponent<Toggle>())
        {

            if (_soundSFX.GetComponent<Toggle>().isOn)
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_Ef_On");
                _soundSFXImage.sprite = _sfxImage[0];
                SoundManager.SFXOnOff(true);
                foreach (var nCnt in SoundManager.GetInstance.SFXSource) nCnt.volume = 1;
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlaySFX = true;
            }
            else
            {
                if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_Ef_Off");
                _soundSFXImage.sprite = _sfxImage[1];
                SoundManager.SFXOnOff(false);
                foreach (var nCnt in SoundManager.GetInstance.SFXSource) nCnt.volume = 0;
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlaySFX = false;
            }
        }
        else if (toggle == _vibrate.GetComponent<Toggle>())
        {
            FirebaseManager.GetInstance.FirebaseLogEvent("Pause_vibration_button");
            if (_vibrate.GetComponent<Toggle>().isOn)
            {
                _vibrateImage.sprite = _vibImage[0];
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayVib = true;
                Debug.Log("true");
            }
            else
            {
                _vibrateImage.sprite = _vibImage[1];
                if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsPlayVib = false;
                Debug.Log("false");
            }
        }
    }

    public void OnClickGoMain()
    {
        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                PopupManager.instance.ShowChallengeReAskPopup();
                return;
            }
        }

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                PopupManager.instance.ShowEventLevelRetryPopup();
                return;
            }
        }

        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_Home");
        ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);
        var obj = transform.parent.gameObject;
        obj.GetComponent<PopupManager>().CallLoadingTutorialPop("MainScene", 100);
    }

    public void OnClickRePlay()
    {
        if (ChallengeSystem.GetInstance != null)
        {
            if (ChallengeSystem.GetInstance.IsChallengeStage)
            {
                PopupManager.instance.ShowChallengeReAskPopup();
                return;
            }
        }

        if (EventLevelSystem.GetInstance != null)
        {
            if (EventLevelSystem.GetInstance.IsEventLevel)
            {
                EventLevelSystem.GetInstance.isRetry = true;
                PopupManager.instance.ShowEventLevelRetryPopup();
                return;
            }
        }

        if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Pause_Replay");
        ADManager.GetInstance.ShowInterstitial(EInterstitialKind.INTERSTITIAL);
        var obj = transform.parent.gameObject;
        obj.GetComponent<PopupManager>().CallLoadingTutorialPop("GameScene");
    }
}