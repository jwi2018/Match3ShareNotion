using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public struct RoulletInfo
{
    public int ItemList;
    public int Persent;
}

public class RoulettePopup : PopupSetting
{
    [SerializeField] private GameObject MoreAdsButton;

    [SerializeField] private GameObject MoreNoAdsButton;

    [SerializeField] private GameObject SpinButton;

    [SerializeField] private GameObject SpinCloseButton;

    [SerializeField] private GameObject GetItemButton;

    [SerializeField] private GameObject RouletteBase;

    [SerializeField] private GameObject X_Button;

    [SerializeField] private List<float> ItemList;

    [SerializeField] private List<RouletteItemStatus> RouletteItems_Info;

    [SerializeField] private AnimationCurve SpinSpeedController;

    [SerializeField] private AnimationGetItem animationGetItem;

    [SerializeField] private AnimationListener _listener;

    [SerializeField] private GameObject getItemParticle;

    [SerializeField] private GameObject gobPIN;

    private bool _ShowAdsSpin;
    private float AfterRotate;
    private float BeforeRotate;

    private float deg;
    private float DegCheck = 0.0f;
    private bool isSpining;
    private float Range;

    private void Awake()
    {
        gobPIN.SetActive(false);
    }

    private void Start()
    {
        OnPopupSetting();
        StartCoroutine(CoInit(1f));
    }

    private IEnumerator CoInit(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        gobPIN.SetActive(true);
    }

    public override void OnPopupSetting()
    {
        FirebaseManager.GetInstance.FirebaseLogEvent("Intro_roulette_enter");
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
        deg = 360 / ItemList.Count;
    }

    public override void OffPopupSetting()
    {
        if (isSpining) return;
        GetComponent<Animator>().SetTrigger("Off");
    }

    public override void PressedBackKey()
    {
        OffPopupSetting();
    }

    public override void OnButtonClick()
    {
        if (SoundManager.GetInstance != null) SoundManager.GetInstance.Play("Popup");
    }

    public void CheckProgress(bool _isDailyRoulette)
    {
        if (_isDailyRoulette)
        {
            SpinButton.SetActive(false);

            if (Application.internetReachability == NetworkReachability.NotReachable)
                MoreNoAdsButton.SetActive(true);
            else
                MoreAdsButton.SetActive(true);
        }
    }

    public void OnClickButtonSpin()
    {

        if (FirebaseManager.GetInstance != null)
            FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_Daily");
        if (isSpining) return;
        SpinButton.SetActive(false);
        //FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_Daily");
        StartCoroutine(SpiningRoulette());
        if (PlayerData.GetInstance != null)
        {
            PlayerData.GetInstance.IsDailyRoulette = true;
            PlayerData.GetInstance.RouletteYear = DateTime.Now.Year;
            PlayerData.GetInstance.RouletteMonth = DateTime.Now.Month;
            PlayerData.GetInstance.RouletteDay = DateTime.Now.Day;
            PlayerData.GetInstance.RouletteHour = DateTime.Now.Hour;
            PlayerData.GetInstance.RouletteMinute = DateTime.Now.Minute;
            PlayerData.GetInstance.RouletteSecond = DateTime.Now.Second;
        }
    }

    public void TestSpin()
    {
        StartCoroutine(SpiningRoulette());
    }

    public void OnClickAdsOneMore()
    {
        ADManager.GetInstance.ShowReward(ERewardedKind.REWARD, () =>
        {
            var paramater = new Dictionary<string, string>();
            paramater.Add("AdsResult", true.ToString());

            if (FirebaseManager.GetInstance != null) FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_reward", paramater);
            if (PlayerData.GetInstance != null)
            {
                PlayerData.GetInstance.IsAdsRoulette = true;

                PlayerData.GetInstance.RouletteYear = DateTime.Now.Year;
                PlayerData.GetInstance.RouletteMonth = DateTime.Now.Month;
                PlayerData.GetInstance.RouletteDay = DateTime.Now.Day;
                PlayerData.GetInstance.RouletteHour = DateTime.Now.Hour;
                PlayerData.GetInstance.RouletteMinute = DateTime.Now.Minute;
                PlayerData.GetInstance.RouletteSecond = DateTime.Now.Second;
            }

            _ShowAdsSpin = true;
            MoreAdsButton.SetActive(false);
            StartCoroutine(SpiningRoulette());
            if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsAdsRoulette = true;
        });
        /*AD.ShowAd(ERewardedKind.GETITEM, () =>
        {
            var paramater = new Dictionary<string, object>();
            paramater.Add("AdsResult", true.ToString());

            FirebaseManager.GetInstance.FirebaseLogEvent("Roulette_Reward", paramater);
            if (PlayerData.GetInstance != null)
            {
                PlayerData.GetInstance.IsAdsRoulette = true;

                PlayerData.GetInstance.RouletteYear = DateTime.Now.Year;
                PlayerData.GetInstance.RouletteMonth = DateTime.Now.Month;
                PlayerData.GetInstance.RouletteDay = DateTime.Now.Day;
                PlayerData.GetInstance.RouletteHour = DateTime.Now.Hour;
                PlayerData.GetInstance.RouletteMinute = DateTime.Now.Minute;
                PlayerData.GetInstance.RouletteSecond = DateTime.Now.Second;
            }

            _ShowAdsSpin = true;
            MoreAdsButton.SetActive(false);
            StartCoroutine(SpiningRoulette());
            if (PlayerData.GetInstance != null) PlayerData.GetInstance.IsAdsRoulette = true;
        });*/
    }

    public void GetItemAnimationEnd()
    {
        GetItemButton.SetActive(true);
    }

    public void OnClickGetItem()
    {
        animationGetItem.gameObject.GetComponent<Animator>().SetTrigger("Off");
        GetItemButton.SetActive(false);
        X_Button.SetActive(true);
        transform.parent.GetComponent<PopupManager>().GoldRefresh();
        getItemParticle.SetActive(true);
        GetComponent<AnimationListener>().Play(1);
    }

    public void GetItemAnimaion_Off_End()
    {
        isSpining = false;
        animationGetItem.gameObject.SetActive(false);
    }

    private IEnumerator SpiningRoulette()
    {
        isSpining = true;
        X_Button.SetActive(false);
        var Setcuved = new AnimationCurve();
        Setcuved.AddKey(SpinSpeedController.keys[0]);
        float MaxinumPersent = 0;

        foreach (var item in ItemList) MaxinumPersent += item;
        var mydiceCount = Random.Range(0, MaxinumPersent);

        var myItem = 0;

        for (var i = 0; i < ItemList.Count; i++)
        {
            mydiceCount -= ItemList[i];
            if (mydiceCount < 0)
            {
                myItem = i + 1;
                break;
            }
        }

        var TrunCount = Random.Range(4, 5);
        TrunCount *= 360;
        var Range_Deg = deg / 2; // Random.Range(1, deg - 1);
        var _AddDeg = deg * myItem + Range_Deg;
        var value_1_key = new Keyframe();
        value_1_key.value = TrunCount + _AddDeg;
        var endTime = SpinSpeedController.keys[SpinSpeedController.keys.Length - 1].time;
        value_1_key.time = endTime;
        Setcuved.AddKey(value_1_key);
        SpinSpeedController.keys = Setcuved.keys;
        if (PlayerData.GetInstance != null)
        {
            DailyQuestManager.CollectMission(EDailyQuestType.USEROULETTE, 1);
            switch (myItem)
            {
                case 1:
                    PlayerData.GetInstance.ItemHammer += 1;
                    break;

                case 2:
                    PlayerData.GetInstance.Gold += 100;
                    break;

                case 3:
                    PlayerData.GetInstance.ItemColor += 1;
                    break;

                case 4:
                    PlayerData.GetInstance.Gold += 150;
                    break;

                case 5:
                    PlayerData.GetInstance.ItemBomb += 1;
                    break;

                case 6:
                    PlayerData.GetInstance.Gold += 50;
                    break;

                case 7:
                    PlayerData.GetInstance.ItemHammer += 1;
                    break;

                case 8:
                    PlayerData.GetInstance.Gold += 200;
                    break;
            }
        }

        var TotalTime = 0.0f;
        _listener.Play(64);
        while (TotalTime < endTime)
        {
            TotalTime += Time.deltaTime;
            Debug.LogWarningFormat("KKI :: {0}, {1}", SpinSpeedController.Evaluate(TotalTime), TotalTime);
            RouletteBase.transform.rotation =
                Quaternion.Euler(new Vector3(0, 0, SpinSpeedController.Evaluate(TotalTime)));
            yield return new WaitForEndOfFrame();
        }

        RouletteBase.transform.rotation = Quaternion.Euler(new Vector3(0, 0, SpinSpeedController.Evaluate(TotalTime)));

        var sprite = RouletteItems_Info[myItem - 1].GetImage.sprite;
        var intValue = RouletteItems_Info[myItem - 1].GetIntValue;
        var rect = RouletteItems_Info[myItem - 1].GetImage.rectTransform;
        animationGetItem.ChangeItem(sprite, intValue, rect);
        _listener.Play(65);
        yield return new WaitWhile(() => isSpining);

        if (!_ShowAdsSpin)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                //MoreNoAdsButton.SetActive(true);
                //SpinCloseButton.SetActive(true);
                OffPopupSetting();
            }
            else
            {
                GetItemButton.SetActive(false);
                MoreAdsButton.SetActive(true);
            }
        }
        else
        {
            //SpinCloseButton.SetActive(true);
            OffPopupSetting();
        }

        yield return new WaitForEndOfFrame();
    }

    public void CancelReward()
    {
        StopAllCoroutines();
    }

    private IEnumerator LoadReardAds(GameObject popupManager)
    {
        var WaitTime = 0.0f;
        yield return new WaitForEndOfFrame();
        //while (WaitTime < 5.0f)
        //{
        //    WaitTime += Time.deltaTime;

        //    if (AdsManager.GetInstance.IsNewVideoLoaded(ERewardType.ROULETTE))
        //    {
        //        for (int i = 0; i < popupManager.transform.childCount; i++)
        //        {
        //            if (popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>() != null)
        //            {
        //                popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>().OffPopupSetting();
        //            }
        //        }
        //        break;
        //    }
        //    yield return new WaitForEndOfFrame();
        //}
        //yield return new WaitForEndOfFrame();
        //if (!AdsManager.GetInstance.IsNewVideoLoaded(ERewardType.ROULETTE))
        //{
        //    for (int i = 0; i < popupManager.transform.childCount; i++)
        //    {
        //        if (popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>() != null)
        //        {
        //            popupManager.transform.GetChild(i).GetComponent<NotRewardedAdsPopup>().LoadFail();
        //        }
        //    }
        //}
        //else
        //{
        //    OnClickAdsOneMore();
        //}
        yield return new WaitForEndOfFrame();
    }
}