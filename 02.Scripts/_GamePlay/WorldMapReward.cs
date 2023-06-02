using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapReward : MonoBehaviour
{
    [SerializeField] private GameObject enableRewardButton;
    [SerializeField] private GameObject disableRewardButton;
    [SerializeField] private PopupManager popupManager;

    [SerializeField] private Text timeText;

    [SerializeField] private float CoolTime;
    [SerializeField] private int RewardedCoin;

    private void Start()
    {
        if (PlayerData.GetInstance.WorldMapYear == 0)
        {
            System.DateTime rewardTime = System.DateTime.Now;

            PlayerData.GetInstance.WorldMapYear = rewardTime.AddSeconds(-CoolTime).Year;
            PlayerData.GetInstance.WorldMapMonth = rewardTime.AddSeconds(-CoolTime).Month;
            PlayerData.GetInstance.WorldMapDay = rewardTime.AddSeconds(-CoolTime).Day;
            PlayerData.GetInstance.WorldMapHour = rewardTime.AddSeconds(-CoolTime).Hour;
            PlayerData.GetInstance.WorldMapMinute = rewardTime.AddSeconds(-CoolTime).Minute;
            PlayerData.GetInstance.WorldMapSecond = rewardTime.AddSeconds(-CoolTime).Second;
        }

        StartCoroutine(WorldMapRewardCoroutine());
    }

    public void ShowAd()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            PlayerData.GetInstance.WorldMapYear = System.DateTime.Now.Year;
            PlayerData.GetInstance.WorldMapMonth = System.DateTime.Now.Month;
            PlayerData.GetInstance.WorldMapDay = System.DateTime.Now.Day;
            PlayerData.GetInstance.WorldMapHour = System.DateTime.Now.Hour;
            PlayerData.GetInstance.WorldMapMinute = System.DateTime.Now.Minute;
            PlayerData.GetInstance.WorldMapSecond = System.DateTime.Now.Second;
            PlayerData.GetInstance.Gold += RewardedCoin;

            GameObject coin = popupManager.GetCoin();
            coin.GetComponent<Animator>().SetTrigger("Normal");

            StartCoroutine(WorldMapRewardCoroutine());
        });
        /*AD.ShowAd(ERewardedKind.WORLDMAPCOIN, () =>
        {
            PlayerData.GetInstance.WorldMapYear = System.DateTime.Now.Year;
            PlayerData.GetInstance.WorldMapMonth = System.DateTime.Now.Month;
            PlayerData.GetInstance.WorldMapDay = System.DateTime.Now.Day;
            PlayerData.GetInstance.WorldMapHour = System.DateTime.Now.Hour;
            PlayerData.GetInstance.WorldMapMinute = System.DateTime.Now.Minute;
            PlayerData.GetInstance.WorldMapSecond = System.DateTime.Now.Second;
            PlayerData.GetInstance.Gold += RewardedCoin;

            GameObject coin = popupManager.GetCoin();
            coin.GetComponent<Animator>().SetTrigger("Normal");

            StartCoroutine(WorldMapRewardCoroutine());
        });*/
    }

    private IEnumerator WorldMapRewardCoroutine()
    {
        enableRewardButton.SetActive(false);
        disableRewardButton.SetActive(true);
        System.DateTime time =
        new System.DateTime(PlayerData.GetInstance.WorldMapYear, PlayerData.GetInstance.WorldMapMonth, PlayerData.GetInstance.WorldMapDay,
        PlayerData.GetInstance.WorldMapHour, PlayerData.GetInstance.WorldMapMinute, PlayerData.GetInstance.WorldMapSecond);
        System.TimeSpan resultTime = time - System.DateTime.Now;

        float coolTime = (float)resultTime.TotalSeconds + CoolTime;

        int minute = 0;
        int second = 0;
        while (coolTime > 0)
        {
            coolTime -= Time.deltaTime;

            minute = (int)coolTime / 60;
            second = (int)coolTime % 60;

            timeText.text = minute + ":" + second.ToString("D2");

            yield return new WaitForSeconds(0.5f);
            //yield return new WaitForEndOfFrame();
        }

        enableRewardButton.SetActive(true);
        disableRewardButton.SetActive(false);
    }
}