using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FreeGoldStatus : MonoBehaviour
{
    [SerializeField] private GameObject enableRewardButton;
    [SerializeField] private GameObject disableRewardButton;
    [SerializeField] private PopupManager popupManager;

    [SerializeField] private Text timeText;

    [HideInInspector] public float CoolTime;
    [SerializeField] private int RewardedCoin;
    [HideInInspector] public float realtimecoolTime;

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

        CoolTime = PlayerData.GetInstance.FreeGoldCooltime;
        if (CoolTime == 0)
        {
            CoolTime = 60;
        }

        StartCoroutine(WorldMapRewardCoroutine());
    }

    public void ShowPopup()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("intro_coinreward_enter");
        var obj = Instantiate(PopupList.GetInstance.Pop_FreeGold, PopupManager.instance.transform);
        Pop_FreeGold popFreegold = obj.GetComponent<Pop_FreeGold>();
        popFreegold.SetPopup(this);
        //AD.ShowAd(ERewardedKind.WORLDMAPCOIN, () =>
        //{
        //    PlayerData.GetInstance.WorldMapYear = System.DateTime.Now.Year;
        //    PlayerData.GetInstance.WorldMapMonth = System.DateTime.Now.Month;
        //    PlayerData.GetInstance.WorldMapDay = System.DateTime.Now.Day;
        //    PlayerData.GetInstance.WorldMapHour = System.DateTime.Now.Hour;
        //    PlayerData.GetInstance.WorldMapMinute = System.DateTime.Now.Minute;
        //    PlayerData.GetInstance.WorldMapSecond = System.DateTime.Now.Second;
        //    PlayerData.GetInstance.Gold += RewardedCoin;

        //    GameObject coin = popupManager.GetCoin();
        //    coin.GetComponent<Animator>().SetTrigger("Normal");

        //    StartCoroutine(WorldMapRewardCoroutine());
        //});
    }

    private void CooltimeRefresh()
    {
    }

    private IEnumerator WorldMapRewardCoroutine()
    {
        enableRewardButton.SetActive(false);
        disableRewardButton.SetActive(true);

        int minute = 0;
        int second = 0;
        while (true)
        {
            System.DateTime time = new System.DateTime(PlayerData.GetInstance.WorldMapYear, PlayerData.GetInstance.WorldMapMonth, PlayerData.GetInstance.WorldMapDay,
        PlayerData.GetInstance.WorldMapHour, PlayerData.GetInstance.WorldMapMinute, PlayerData.GetInstance.WorldMapSecond);

            System.TimeSpan resultTime = time - System.DateTime.Now;

            realtimecoolTime = (float)resultTime.TotalSeconds + CoolTime;

            realtimecoolTime -= Time.deltaTime;

            minute = (int)realtimecoolTime / 60;
            second = (int)realtimecoolTime % 60;

            timeText.text = minute + ":" + second.ToString("D2");

            
            if (PlayerData.GetInstance.FreeGoldCount.Equals(4) && realtimecoolTime <= 0)
            {
                PlayerData.GetInstance.FreeGoldCount = 0;
            }

            if (realtimecoolTime <= 0)
            {
                ButtonStateChange(true);
            }
            else
            {
                ButtonStateChange(false);
            }

           
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void ButtonStateChange(bool isEnable)
    {
        if (enableRewardButton.activeSelf != isEnable)
        {
            enableRewardButton.SetActive(isEnable);
            disableRewardButton.SetActive(!isEnable);
        }
    }
}