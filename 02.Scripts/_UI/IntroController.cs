using System;
using System.Collections;
using UnityEngine;

public class IntroController : MonoBehaviour
{
    [SerializeField] private PopupManager _popupManager;

    [SerializeField] private GameObject _activeParnetObject;

    [SerializeField] private GameObject _weeklyPop;

    private bool _isFirst;
    private bool _onClickButton;
    private bool _openStart;

    private void Start()
    {
        if (!LoadingImageManager._isGameStart)
        {
            SoundManager.GetInstance.PlayBGM("Main");
            Destroy(_weeklyPop);
            gameObject.SetActive(false);
            if (ADManager.GetInstance.noAdsPopup)
            {
                if (!PlayerData.GetInstance.IsAdsFree && ADManager.GetInstance.noAdsCount.Equals(0))
                {
                    if (!BaseSystem.GetInstance.GetSystemList("Fantasy")) _popupManager.ShowNoAdsPopup();
                    ADManager.GetInstance.noAdsCount++;
                }
                else
                {
                    ADManager.GetInstance.noAdsCount++;
                    if (ADManager.GetInstance.noAdsCount > 4)
                    {
                        ADManager.GetInstance.noAdsCount = 0;
                    }
                }
                Debug.Log("ADManager.GetInstance.noAdsCount    :    " + ADManager.GetInstance.noAdsCount);
            }
        }
        else
        {
            if (PlayerData.GetInstance != null)
            {
                if (PlayerData.GetInstance.NumLanguage == 9)
                {
                    _isFirst = true;
                }
                else if (PlayerData.GetInstance.NumLanguage == 0)
                {
                    if (Application.systemLanguage == SystemLanguage.Korean)
                    {
                        if (PopupList.GetInstance != null)
                        {
                            _isFirst = true;
                            PlayerData.GetInstance.NumLanguage = 9;
                        }
                    }
                    else
                    {
                        if (Application.systemLanguage != SystemLanguage.Japanese &&
                            Application.systemLanguage != SystemLanguage.ChineseSimplified &&
                            Application.systemLanguage != SystemLanguage.ChineseTraditional)
                        {
                            if (Application.systemLanguage == SystemLanguage.French)
                                PlayerData.GetInstance.NumLanguage = 2;
                            else if (Application.systemLanguage == SystemLanguage.German)
                                PlayerData.GetInstance.NumLanguage = 3;
                            else if (Application.systemLanguage == SystemLanguage.Spanish)
                                PlayerData.GetInstance.NumLanguage = 4;
                            else if (Application.systemLanguage == SystemLanguage.Italian)
                                PlayerData.GetInstance.NumLanguage = 5;
                            else if (Application.systemLanguage == SystemLanguage.Russian)
                                PlayerData.GetInstance.NumLanguage = 6;
                            else if (Application.systemLanguage == SystemLanguage.Portuguese)
                                PlayerData.GetInstance.NumLanguage = 7;
                            else if (Application.systemLanguage == SystemLanguage.Turkish)
                                PlayerData.GetInstance.NumLanguage = 8;
                            else if (Application.systemLanguage == SystemLanguage.Indonesian)
                                PlayerData.GetInstance.NumLanguage = 13;
                            else if (Application.systemLanguage == SystemLanguage.Thai)
                                PlayerData.GetInstance.NumLanguage = 15;
                            else if (Application.systemLanguage == SystemLanguage.English)
                                PlayerData.GetInstance.NumLanguage = 1;
                        }
                        else
                        {
                            if (Application.systemLanguage == SystemLanguage.Japanese)
                                PlayerData.GetInstance.NumLanguage = 10;
                            else if (Application.systemLanguage == SystemLanguage.ChineseSimplified)
                                PlayerData.GetInstance.NumLanguage = 11;
                            else if (Application.systemLanguage == SystemLanguage.ChineseTraditional)
                                PlayerData.GetInstance.NumLanguage = 12;
                        }
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (!_openStart)
            if (PlayerData.GetInstance != null)
            {
                if (_isFirst)
                {
                    if (PlayerData.GetInstance.NumLanguage == 9)
                    {
                        _openStart = true;
                        GetComponent<Animator>().SetTrigger("First");
                    }
                }
                else
                {
                    _openStart = true;
                    GetComponent<Animator>().SetTrigger("Open");
                }
            }
    }

    public void OnClickPlay()
    {
        if (!_onClickButton)
        {
            _onClickButton = true;
            StartCoroutine(FrontBannerShow());
        }
    }

    public void OffActive()
    {
        SoundManager.GetInstance.PlayBGM("Main");
        CheckWeelyBonus();
    }

    public void CheckWeelyBonus()
    {
        _activeParnetObject.SetActive(false);
        LoadingImageManager._isGameStart = false;
        /*//this.GetComponent<Animator>().SetTrigger("Close");
        _weeklyPop = _popupManager.CallWeeklyBonus();
        if (_weeklyPop != null) _weeklyPop.SetActive(true);
        Debug.Log(_weeklyPop);
        Debug.Log("55");*/
        var _isDaily = false;
        if (PlayerData.GetInstance != null)
        {
            if (PlayerData.GetInstance.DailyYear == 0 && PlayerData.GetInstance.DailyMonth == 0 &&
                PlayerData.GetInstance.DailyDay == 0)
            {
                _isDaily = true;
                PlayerData.GetInstance.DailyYear = DateTime.Now.Year;
                PlayerData.GetInstance.DailyMonth = DateTime.Now.Month;
                PlayerData.GetInstance.DailyDay = DateTime.Now.Day;
            }
            else
            {
                var time = new DateTime(PlayerData.GetInstance.DailyYear, PlayerData.GetInstance.DailyMonth,
                    PlayerData.GetInstance.DailyDay);
                var resultTime = time - DateTime.Now;
                if (resultTime.Days < 0)
                {
                    _isDaily = true;
                    PlayerData.GetInstance.DailyYear = DateTime.Now.Year;
                    PlayerData.GetInstance.DailyMonth = DateTime.Now.Month;
                    PlayerData.GetInstance.DailyDay = DateTime.Now.Day;
                    if (resultTime.Days < -1) PlayerData.GetInstance.WeeklyXDay = 0;
                }
            }
        }

        if (_isDaily)
            _weeklyPop.SetActive(true);
        else
        {
            //_weeklyPop.SetActive(true);
            Destroy(_weeklyPop);
        }
            
    }

    private IEnumerator FrontBannerShow()
    {
        yield return new WaitForSeconds(0.1f);
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {

            yield return new WaitForEndOfFrame();
        }

        LoadingImageManager._isGameStart = false;
        GetComponent<Animator>().SetTrigger("Close");
        _weeklyPop = _popupManager.CallWeeklyBonus();

        if (SoundManager.GetInstance != null) SoundManager.GetInstance.PlayBGM("Main");
    }
}