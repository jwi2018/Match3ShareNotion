using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RouletteStatus : MonoBehaviour
{
    [SerializeField] private GameObject enableRewardButton;
    [SerializeField] private GameObject disableRewardButton;
    [SerializeField] private PopupManager popupManager;
    [SerializeField] private GameObject noAdsButton;

    [SerializeField] private Text timeText;

    [SerializeField] private float CoolTime;
    private bool _isAdsRoulette;

    private bool _isDailayRoulette = false;

    private void Start()
    {
        if (PlayerData.GetInstance != null)
            if (PlayerData.GetInstance.RouletteYear == 0)
            {
                var rewardTime = DateTime.Now;

                PlayerData.GetInstance.RouletteYear = rewardTime.AddSeconds(-CoolTime).Year;
                PlayerData.GetInstance.RouletteMonth = rewardTime.AddSeconds(-CoolTime).Month;
                PlayerData.GetInstance.RouletteDay = rewardTime.AddSeconds(-CoolTime).Day;
                PlayerData.GetInstance.RouletteHour = rewardTime.AddSeconds(-CoolTime).Hour;
                PlayerData.GetInstance.RouletteMinute = rewardTime.AddSeconds(-CoolTime).Minute;
                PlayerData.GetInstance.RouletteSecond = rewardTime.AddSeconds(-CoolTime).Second;
            }

        StartCoroutine(RouletteCoroutine());
        StartCoroutine(DelayUpdate());
        if (noAdsButton != null)
        {
            StartCoroutine(NoAds());
        }
    }

    private IEnumerator DelayUpdate()
    {
        yield return new WaitForSeconds(0.2f);
        while (true)
        {
            if (PlayerData.GetInstance != null)
            {
                if (PlayerData.GetInstance.IsAdsRoulette != _isAdsRoulette)
                {
                    _isAdsRoulette = PlayerData.GetInstance.IsAdsRoulette;
                    StartCoroutine(RouletteCoroutine());
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public void OnClickButton()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("intro_roullet_enter");
        popupManager.OnClickRoulette(PlayerData.GetInstance.IsDailyRoulette);
    }

    private IEnumerator RouletteCoroutine()
    {
        enableRewardButton.SetActive(false);
        disableRewardButton.SetActive(true);

        var DailyTime =
            new DateTime(PlayerData.GetInstance.RouletteYear, PlayerData.GetInstance.RouletteMonth,
                PlayerData.GetInstance.RouletteDay);
        var DailyResultTime = DailyTime - DateTime.Now;

        var time =
            new DateTime(PlayerData.GetInstance.RouletteYear, PlayerData.GetInstance.RouletteMonth,
                PlayerData.GetInstance.RouletteDay,
                PlayerData.GetInstance.RouletteHour, PlayerData.GetInstance.RouletteMinute,
                PlayerData.GetInstance.RouletteSecond);
        var resultTime = time - DateTime.Now;

        if (DailyResultTime.Days < 0)
        {
            PlayerData.GetInstance.IsDailyRoulette = false;
            PlayerData.GetInstance.IsAdsRoulette = false;
            _isAdsRoulette = false;
        }

        if (!PlayerData.GetInstance.IsAdsRoulette)
        {
            enableRewardButton.SetActive(true);
            disableRewardButton.SetActive(false);
            yield break;
        }

        var coolTime = (float)resultTime.TotalSeconds + CoolTime;

        var minute = 0;
        var second = 0;

        while (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            minute = (int)coolTime / 60;
            second = (int)coolTime % 60;

            timeText.text = string.Format($"{minute}:{second.ToString("D2")}");

            yield return new WaitForEndOfFrame();
        }

        enableRewardButton.SetActive(true);
        disableRewardButton.SetActive(false);

        _isAdsRoulette = false;
        PlayerData.GetInstance.IsDailyRoulette = false;
        PlayerData.GetInstance.IsAdsRoulette = false;
    }

    private IEnumerator NoAds()
    {
        while (true)
        {
            if (PlayerData.GetInstance.IsAdsFree)
            {
                noAdsButton.SetActive(false);
            }
            else
            {
                noAdsButton.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}