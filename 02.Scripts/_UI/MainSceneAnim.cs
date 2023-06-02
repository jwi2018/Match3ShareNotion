using UnityEngine;

public class MainSceneAnim : MonoBehaviour
{
    /*
    [SerializeField]
    private GameObject PopupManager;
    [SerializeField]
    private GameObject AppleStore;
    [SerializeField]
    private GameObject GoogleStore;

    [SerializeField]
    private Image[] _gameBanner;
    [SerializeField]
    private Sprite[] _gameBannerImage;

    bool isPlay = false;

    private void Start()
    {
        Debug.Log(PlayerData.GetInstance.NumLanguage);
        //
        if (TitleManager.SetisTitleAnimActive == true)
        {
            TitleManager.SetisTitleAnimActive = false;
            this.GetComponent<Animator>().SetTrigger("First");

            if (PlayerData.GetInstance != null)
            {
                if (PlayerData.GetInstance.NumLanguage != 0)
                {
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        if (!PlayerData.GetInstance.IsTermsOfUse_KR)
                        {
                            Instantiate(PopupList.GetInstance.Popup_PrivacyPolicy_KR, PopupManager.transform);
                        }
                    }
                    else
                    {
                        if (Application.systemLanguage != SystemLanguage.Japanese &&
                            Application.systemLanguage != SystemLanguage.ChineseSimplified &&
                            Application.systemLanguage != SystemLanguage.ChineseTraditional)
                        {
                            if (!PlayerData.GetInstance.IsTermsOfUse_EU)
                            {
                                Instantiate(PopupList.GetInstance.Popup_PrivacyPolicy_EU, PopupManager.transform);
                            }
                        }
                        else
                        {
                            if (Application.systemLanguage == SystemLanguage.Japanese)
                            {
                                PlayerData.GetInstance.NumLanguage = 10;
                            }
                            else if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
                            {
                                PlayerData.GetInstance.NumLanguage = 11;
                            }
                            else if (Application.systemLanguage != SystemLanguage.ChineseTraditional)
                            {
                                PlayerData.GetInstance.NumLanguage = 12;
                            }
                        }
                    }

                }
                else
                {
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        if (!PlayerData.GetInstance.IsTermsOfUse_KR)
                        {
                            if (PopupList.GetInstance != null)
                            {
                                Instantiate(PopupList.GetInstance.Popup_PrivacyPolicy_KR, PopupManager.transform);
                                PlayerData.GetInstance.NumLanguage = 9;
                            }
                        }
                    }
                    else
                    {
                        if (Application.systemLanguage != SystemLanguage.Japanese &&
                            Application.systemLanguage != SystemLanguage.ChineseSimplified &&
                            Application.systemLanguage != SystemLanguage.ChineseTraditional)
                        {
                            if (!PlayerData.GetInstance.IsTermsOfUse_EU)
                            {
                                Instantiate(PopupList.GetInstance.Popup_PrivacyPolicy_EU, PopupManager.transform);
                            }
                        }
                        else
                        {
                            if(Application.systemLanguage == SystemLanguage.Japanese)
                            {
                                PlayerData.GetInstance.NumLanguage = 10;
                            }
                            else if(Application.systemLanguage == SystemLanguage.ChineseSimplified)
                            {
                                PlayerData.GetInstance.NumLanguage = 11;
                            }
                            else if(Application.systemLanguage == SystemLanguage.ChineseTraditional)
                            {
                                PlayerData.GetInstance.NumLanguage = 12;
                            }
                        }
                    }
                }
            }
            if(PlayerData.GetInstance.NumLanguage == 10)
            {
                foreach(Image nCnt in _gameBanner)
                {
                    nCnt.sprite = _gameBannerImage[1];
                }
            }
            else if (PlayerData.GetInstance.NumLanguage == 11)
            {
                foreach (Image nCnt in _gameBanner)
                {
                    nCnt.sprite = _gameBannerImage[2];
                }
            }
            else if (PlayerData.GetInstance.NumLanguage == 12)
            {
                foreach (Image nCnt in _gameBanner)
                {
                    nCnt.sprite = _gameBannerImage[3];
                }
            }
            else
            {
                foreach (Image nCnt in _gameBanner)
                {
                    nCnt.sprite = _gameBannerImage[0];
                }
            }


            this.GetComponent<Animator>().SetTrigger("First");
        }
        if (Application.platform == RuntimePlatform.Android)
        {
            GoogleStore.SetActive(true);
            AppleStore.SetActive(false);

        }
        else
        {

            AppleStore.SetActive(true);
            GoogleStore.SetActive(false);
        }

    }

    public void IsGameStart()
    {
        bool isRewardGet = false;

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.DailyYear == 0 && PlayerData.GetInstance.DailyMonth == 0 && PlayerData.GetInstance.DailyDay == 0)
            {
                isRewardGet = true;
                PlayerData.GetInstance.DailyYear = System.DateTime.Now.Year;
                PlayerData.GetInstance.DailyMonth = System.DateTime.Now.Month;
                PlayerData.GetInstance.DailyDay = System.DateTime.Now.Day;
            }
            else
            {
                System.DateTime time = new System.DateTime(PlayerData.GetInstance.DailyYear, PlayerData.GetInstance.DailyMonth, PlayerData.GetInstance.DailyDay);
                System.TimeSpan resultTime = time - System.DateTime.Now;
                if (resultTime.Days < 0)
                {
                    isRewardGet = true;
                    PlayerData.GetInstance.DailyYear = System.DateTime.Now.Year;
                    PlayerData.GetInstance.DailyMonth = System.DateTime.Now.Month;
                    PlayerData.GetInstance.DailyDay = System.DateTime.Now.Day;
                }
            }

        }
        if (isRewardGet)
        {
            if (SoundManager.GetInstance != null)
            {
                SoundManager.GetInstance.Play(SoundManager.GetInstance.DailyBonus);
            }
            PopupManager.GetComponent<PopupManager>().OnClickDailyBonus();
        }
    }

    public void IsRateUs()
    {
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PlayBGM(0);
        }

        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.PresentLevel >= 15)
            {
                if (!PlayerData.GetInstance.IsRateUs)
                {
                    if (PlayerData.GetInstance.RateUsYear == 0 && PlayerData.GetInstance.RateUsMonth == 0 && PlayerData.GetInstance.RateUsDay == 0)
                    {

                            PopupManager.GetComponent<PopupManager>().OnClickRateUs();

                        TitleManager.SetisRateUs = false;
                        PlayerData.GetInstance.RateUsYear = System.DateTime.Now.Year;
                        PlayerData.GetInstance.RateUsMonth = System.DateTime.Now.Month;
                        PlayerData.GetInstance.RateUsDay = System.DateTime.Now.Day;
                    }
                    else
                    {
                        System.DateTime time = new System.DateTime(PlayerData.GetInstance.RateUsYear, PlayerData.GetInstance.RateUsMonth, PlayerData.GetInstance.RateUsDay);
                        System.TimeSpan resultTime = time - System.DateTime.Now;
                        if (resultTime.Days < 0)
                        {

                            PopupManager.GetComponent<PopupManager>().OnClickRateUs();

                            TitleManager.SetisRateUs = false;
                            PlayerData.GetInstance.RateUsYear = System.DateTime.Now.Year;
                            PlayerData.GetInstance.RateUsMonth = System.DateTime.Now.Month;
                            PlayerData.GetInstance.RateUsDay = System.DateTime.Now.Day;
                        }
                    }
                }
            }
        }
    }

    public void TitleAndGameStart()
    {
        if(isPlay)
        {
            return;
        }
        else
        {
            isPlay = true;
        }
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            StartCoroutine("GameStart");
            return;
        }
        if (AdsManager.GetInstance != null)
        {
            if (AdsManager.GetInstance._adMobInterstitial != null)
            {
                if (AdsManager.GetInstance._adMobInterstitial.IsLoaded() == true)
                {
                    AdsManager.GetInstance.ShowInterstitialAds(AdsManager.GetInstance._adMobInterstitial);
                    StartCoroutine("GameStart");
                }
                else
                {
                    StartCoroutine(InterstitialAdsWait());
                }
            }
        }
    }

    IEnumerator GameStart()
    {
        //광고
        //광고 출력 기다림

        yield return new WaitForEndOfFrame();
        this.GetComponent<Animator>().SetTrigger("Off");
        if (SoundManager.GetInstance != null)
        {
            SoundManager.GetInstance.PlayBGM(0);
        }
    }

    IEnumerator InterstitialAdsWait()
    {
        PopupManager.GetComponent<PopupManager>().CallLoadingPop();
        GameObject obj = null;

        while(true)
        {
            for (int i = 0; i < PopupManager.transform.childCount; i++)
            {
                if (PopupManager.transform.GetChild(i).GetComponent<LoadingPopup>() != null)
                {
                    obj = PopupManager.transform.GetChild(i).gameObject;
                }
            }

            if(obj == null)
            {
                StartCoroutine("GameStart");
                break;
            }

            if(AdsManager.GetInstance._adMobInterstitial != null)
            {
                if (AdsManager.GetInstance._adMobInterstitial.IsLoaded() == true)
                {
                    AdsManager.GetInstance.ShowInterstitialAds(AdsManager.GetInstance._adMobInterstitial);

                    for (int i = 0; i < PopupManager.transform.childCount; i++)
                    {
                        if (PopupManager.transform.GetChild(i).GetComponent<LoadingPopup>() != null)
                        {
                            PopupManager.transform.GetChild(i).GetComponent<LoadingPopup>().OffPopupSetting();
                            break;
                        }
                    }
                    StartCoroutine("GameStart");
                    break;
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }*/
}